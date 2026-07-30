// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ToolResilience.cs
// Created          : 2026-07-30  15:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="ToolResilience.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Thrown by a tool executor to signal a failure that must not be retried (e.g. a validation or business error).
/// </summary>
public sealed class NonRetryableToolException : Exception
{
    public NonRetryableToolException(string message) : base(message)
    {
    }

    public NonRetryableToolException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
///     Process-wide resilience state shared by all (scoped) gateways: circuit breakers and concurrency limiters
///     are keyed by tool name so a failing downstream is isolated instead of saturating the pipeline.
/// </summary>
public sealed class ToolResilienceCoordinator(IClock? clock = null) : IDisposable
{
    private readonly ConcurrentDictionary<string, CircuitState> _circuits = new(StringComparer.OrdinalIgnoreCase);
    private readonly IClock _clock = clock ?? new SystemClock();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _limiters = new(StringComparer.OrdinalIgnoreCase);

    #region IDisposable Members

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (SemaphoreSlim limiter in this._limiters.Values)
        {
            limiter.Dispose();
        }

        this._limiters.Clear();
    }

    #endregion

    /// <summary>
    ///     Returns a failure when the tool's circuit is currently open.
    /// </summary>
    public Result EnsureCircuitClosed(string toolName, ToolExecutionPolicy policy)
    {
        if (policy.CircuitBreakerFailureThreshold <= 0)
        {
            return Result.Success();
        }

        if (!this._circuits.TryGetValue(toolName, out CircuitState? state) || state.OpenedUntil is null)
        {
            return Result.Success();
        }

        if (state.OpenedUntil > this._clock.UtcNow)
        {
            return Result.Failure(new Error(
                AiErrorCodes.ToolCircuitOpen,
                $"Tool '{toolName}' is temporarily unavailable (circuit open until {state.OpenedUntil:O})."));
        }

        this._circuits[toolName] = new CircuitState(0, null);
        return Result.Success();
    }

    /// <summary>
    ///     Records a successful execution, closing the circuit.
    /// </summary>
    public void RecordSuccess(string toolName) => this._circuits[toolName] = new CircuitState(0, null);

    /// <summary>
    ///     Records a failed execution, opening the circuit once the threshold is reached.
    /// </summary>
    public void RecordFailure(string toolName, ToolExecutionPolicy policy)
    {
        if (policy.CircuitBreakerFailureThreshold <= 0)
        {
            return;
        }

        CircuitState updated = this._circuits.AddOrUpdate(
            toolName,
            _ => new CircuitState(1, null),
            (_, existing) => new CircuitState(existing.ConsecutiveFailures + 1, existing.OpenedUntil));

        if (updated.ConsecutiveFailures >= policy.CircuitBreakerFailureThreshold)
        {
            TimeSpan duration = policy.CircuitBreakerDuration ?? TimeSpan.FromSeconds(30);
            this._circuits[toolName] = new CircuitState(updated.ConsecutiveFailures, this._clock.UtcNow.Add(duration));
        }
    }

    /// <summary>
    ///     Acquires a concurrency slot for the tool. Returns null when the policy imposes no limit.
    /// </summary>
    public async ValueTask<IDisposable?> AcquireSlotAsync(string toolName, ToolExecutionPolicy policy, CancellationToken cancellationToken)
    {
        if (policy.MaxConcurrency <= 0)
        {
            return null;
        }

        SemaphoreSlim limiter = this._limiters.GetOrAdd(toolName, _ => new SemaphoreSlim(policy.MaxConcurrency, policy.MaxConcurrency));
        await limiter.WaitAsync(cancellationToken).ConfigureAwait(false);
        return new Slot(limiter);
    }

    #region Nested type: CircuitState

    private sealed record CircuitState(int ConsecutiveFailures, DateTimeOffset? OpenedUntil);

    #endregion

    #region Nested type: Slot

    private sealed class Slot(SemaphoreSlim limiter) : IDisposable
    {
        private int _released;

        #region IDisposable Members

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this._released, 1) == 0)
            {
                limiter.Release();
            }
        }

        #endregion
    }

    #endregion
}

/// <summary>
///     Applies timeout, bounded retry, and concurrency limiting around a tool executor.
/// </summary>
internal static class ToolExecutionPipeline
{
    public static async ValueTask<Result<object?>> RunAsync(
        ToolDefinition tool,
        IReadOnlyDictionary<string, object?> arguments,
        ToolExecutionPolicy policy,
        ToolResilienceCoordinator coordinator,
        CancellationToken cancellationToken)
    {
        Result circuit = coordinator.EnsureCircuitClosed(tool.Name, policy);
        if (circuit.Failed)
        {
            return Result.Failure<object?>(circuit.Error);
        }

        int attempts = Math.Max(0, policy.MaxRetries) + 1;
        Error lastError = new(AiErrorCodes.ToolFailed, $"Tool '{tool.Name}' did not execute.");

        for (int attempt = 1; attempt <= attempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IDisposable? slot = await coordinator.AcquireSlotAsync(tool.Name, policy, cancellationToken).ConfigureAwait(false);
            try
            {
                Result<object?> outcome = await ExecuteOnceAsync(tool, arguments, policy, cancellationToken).ConfigureAwait(false);
                if (outcome.Succeeded)
                {
                    coordinator.RecordSuccess(tool.Name);
                    return outcome;
                }

                lastError = outcome.Error;
                coordinator.RecordFailure(tool.Name, policy);

                if (!IsRetryable(outcome.Error) || attempt == attempts)
                {
                    return outcome;
                }
            }
            finally
            {
                slot?.Dispose();
            }

            TimeSpan delay = ComputeBackoff(policy, attempt);
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
        }

        return Result.Failure<object?>(lastError);
    }

    private static async ValueTask<Result<object?>> ExecuteOnceAsync(
        ToolDefinition tool,
        IReadOnlyDictionary<string, object?> arguments,
        ToolExecutionPolicy policy,
        CancellationToken cancellationToken)
    {
        TimeSpan? timeout = policy.Timeout;
        if (timeout is null || timeout <= TimeSpan.Zero)
        {
            return await InvokeAsync(tool, arguments, cancellationToken).ConfigureAwait(false);
        }

        using CancellationTokenSource timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(timeout.Value);

        Task<Result<object?>> execution = InvokeAsync(tool, arguments, timeoutSource.Token).AsTask();

        // A non-cooperative tool may ignore the token, so the wall clock is bounded independently.
        Task completed = await Task.WhenAny(execution, Task.Delay(timeout.Value, CancellationToken.None)).ConfigureAwait(false);
        if (!ReferenceEquals(completed, execution))
        {
            return Result.Failure<object?>(new Error(
                AiErrorCodes.ToolTimeout,
                $"Tool '{tool.Name}' timed out after {timeout.Value}."));
        }

        return await execution.ConfigureAwait(false);
    }

    private static async ValueTask<Result<object?>> InvokeAsync(
        ToolDefinition tool,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken)
    {
        try
        {
            object? value = await tool.ExecuteAsync!(arguments, cancellationToken).ConfigureAwait(false);
            return Result.Success(value);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<object?>(new Error(AiErrorCodes.ToolTimeout, $"Tool '{tool.Name}' was cancelled or timed out."));
        }
        catch (NonRetryableToolException ex)
        {
            return Result.Failure<object?>(new Error(AiErrorCodes.ToolNonRetryable, ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<object?>(new Error(AiErrorCodes.ToolFailed, ex.Message));
        }
    }

    private static bool IsRetryable(Error error)
        => string.Equals(error.Code, AiErrorCodes.ToolTimeout, StringComparison.Ordinal)
           || string.Equals(error.Code, AiErrorCodes.ToolFailed, StringComparison.Ordinal);

    private static TimeSpan ComputeBackoff(ToolExecutionPolicy policy, int attempt)
    {
        TimeSpan baseDelay = policy.RetryDelay ?? TimeSpan.Zero;
        if (baseDelay <= TimeSpan.Zero)
        {
            return TimeSpan.Zero;
        }

        double factor = Math.Pow(2, attempt - 1);
        return TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * factor);
    }
}