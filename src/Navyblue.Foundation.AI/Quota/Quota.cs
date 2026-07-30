// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Quota.cs
// Created          : 2026-07-30  15:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="Quota.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     A quota consumption request.
/// </summary>
/// <param name="Scope">Logical bucket, e.g. "pipeline" or "agent".</param>
/// <param name="Cost">Units consumed by this call.</param>
public sealed record AiQuotaRequest(string Scope, string? UserId = null, string? TenantId = null, int Cost = 1);

/// <summary>
///     Guards per-user / per-tenant AI usage. Protects downstream model spend and prevents a single caller
///     from saturating the pipeline.
/// </summary>
public interface IAiQuotaGuard
{
    ValueTask<Result> TryAcquireAsync(AiQuotaRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
///     Quota guard that never throttles. Default when the application configures no limits.
/// </summary>
public sealed class NullAiQuotaGuard : IAiQuotaGuard
{
    #region IAiQuotaGuard Members

    /// <inheritdoc />
    public ValueTask<Result> TryAcquireAsync(AiQuotaRequest request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Success());

    #endregion
}

/// <summary>
///     Fixed-window in-process quota guard. Suitable for single-node deployments and as a local safety net;
///     replace with a distributed limiter for multi-node clusters.
/// </summary>
public sealed class InMemoryFixedWindowQuotaGuard(int permitLimit, TimeSpan window, IClock? clock = null) : IAiQuotaGuard
{
    private readonly IClock _clock = clock ?? new SystemClock();
    private readonly ConcurrentDictionary<string, Window> _windows = new(StringComparer.Ordinal);

    #region IAiQuotaGuard Members

    /// <inheritdoc />
    public ValueTask<Result> TryAcquireAsync(AiQuotaRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (permitLimit <= 0)
        {
            return ValueTask.FromResult(Result.Success());
        }

        string key = $"{request.Scope}|{request.TenantId}|{request.UserId}";
        DateTimeOffset now = this._clock.UtcNow;

        while (true)
        {
            Window current = this._windows.GetOrAdd(key, _ => new Window(now, 0));
            Window candidate = now - current.StartedAt >= window
                ? new Window(now, request.Cost)
                : new Window(current.StartedAt, current.Used + request.Cost);

            if (candidate.Used > permitLimit)
            {
                return ValueTask.FromResult(Result.Failure(new Error(
                    AiErrorCodes.QuotaExceeded,
                    $"AI quota exceeded for scope '{request.Scope}' ({permitLimit} per {window}).")));
            }

            if (this._windows.TryUpdate(key, candidate, current))
            {
                return ValueTask.FromResult(Result.Success());
            }
        }
    }

    #endregion

    #region Nested type: Window

    private sealed record Window(DateTimeOffset StartedAt, int Used);

    #endregion
}

/// <summary>
///     Tracks consumption of an <see cref="AiBudget" /> across a bounded run.
/// </summary>
public sealed class AiBudgetTracker(AiBudget? budget, IClock? clock = null)
{
    private readonly IClock _clock = clock ?? new SystemClock();
    private readonly DateTimeOffset _startedAt = (clock ?? new SystemClock()).UtcNow;

    public int Steps { get; private set; }
    public int ToolCalls { get; private set; }
    public int LlmTokens { get; private set; }

    public void RecordStep() => this.Steps++;

    public void RecordToolCall() => this.ToolCalls++;

    public void RecordTokens(int tokens) => this.LlmTokens += Math.Max(0, tokens);

    /// <summary>
    ///     Returns a failure when any configured budget dimension is exhausted.
    /// </summary>
    public Result EnsureWithinBudget()
    {
        if (budget is null)
        {
            return Result.Success();
        }

        if (budget.MaxSteps is { } maxSteps && this.Steps >= maxSteps)
        {
            return Exceeded($"step limit {maxSteps}");
        }

        if (budget.MaxToolCalls is { } maxToolCalls && this.ToolCalls >= maxToolCalls)
        {
            return Exceeded($"tool call limit {maxToolCalls}");
        }

        if (budget.MaxLlmTokens is { } maxTokens && this.LlmTokens >= maxTokens)
        {
            return Exceeded($"token limit {maxTokens}");
        }

        if (budget.MaxDuration is { } maxDuration && this._clock.UtcNow - this._startedAt >= maxDuration)
        {
            return Exceeded($"duration limit {maxDuration}");
        }

        return Result.Success();
    }

    private static Result Exceeded(string what)
        => Result.Failure(new Error(AiErrorCodes.BudgetExceeded, $"AI budget exceeded: {what}."));
}