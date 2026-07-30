// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AuditLoggers.cs
// Created          : 2026-07-30  16:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="AuditLoggers.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Bounded in-memory audit sink for single-node deployments, tests, and assertions.
///     Entries beyond <see cref="Capacity" /> evict the oldest, so it cannot grow without limit.
/// </summary>
public sealed class InMemoryAiAuditLogger(int capacity = 10_000) : IAiAuditLogger
{
    private readonly ConcurrentQueue<AiAuditEntry> _entries = new();

    public int Capacity { get; } = capacity <= 0 ? 10_000 : capacity;

    public IReadOnlyList<AiAuditEntry> Entries => this._entries.ToArray();

    #region IAiAuditLogger Members

    /// <inheritdoc />
    public ValueTask<Result> WriteAsync(AiAuditEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);
        this._entries.Enqueue(entry);

        while (this._entries.Count > this.Capacity && this._entries.TryDequeue(out _))
        {
        }

        return ValueTask.FromResult(Result.Success());
    }

    #endregion

    public void Clear()
    {
        while (this._entries.TryDequeue(out _))
        {
        }
    }
}

/// <summary>
///     Audit sink that refuses every write. Registered as the production default so that a deployment which
///     forgot to configure durable auditing fails closed instead of silently discarding the compliance trail.
/// </summary>
public sealed class UnconfiguredAiAuditLogger : IAiAuditLogger
{
    #region IAiAuditLogger Members

    /// <inheritdoc />
    public ValueTask<Result> WriteAsync(AiAuditEntry entry, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Failure(new Error(
            AiErrorCodes.AuditNotConfigured,
            "No durable IAiAuditLogger is registered. Register one, or set NavyblueAiOptions.AllowInMemoryPersistence for non-production use.")));

    #endregion
}

/// <summary>
///     Writes audit entries through <see cref="ILogger" />. Useful when the platform already ships logs to a
///     durable, tamper-evident sink.
/// </summary>
public sealed class LoggingAiAuditLogger(ILogger<LoggingAiAuditLogger> logger) : IAiAuditLogger
{
    #region IAiAuditLogger Members

    /// <inheritdoc />
    public ValueTask<Result> WriteAsync(AiAuditEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);

        logger.Log(
            entry.Succeeded ? LogLevel.Information : LogLevel.Warning,
            "ai-audit action={Action} tool={Tool} user={User} tenant={Tenant} correlation={Correlation} ok={Succeeded} detail={Detail}",
            entry.Action,
            entry.ToolName,
            entry.UserId,
            entry.TenantId,
            entry.CorrelationId,
            entry.Succeeded,
            entry.Detail);

        return ValueTask.FromResult(Result.Success());
    }

    #endregion
}

/// <summary>
///     Fans an audit entry out to several sinks. Succeeds only when every sink succeeds, so a broken
///     compliance sink cannot be masked by a healthy one.
/// </summary>
public sealed class CompositeAiAuditLogger(IEnumerable<IAiAuditLogger> sinks) : IAiAuditLogger
{
    private readonly IAiAuditLogger[] _sinks = sinks.ToArray();

    #region IAiAuditLogger Members

    /// <inheritdoc />
    public async ValueTask<Result> WriteAsync(AiAuditEntry entry, CancellationToken cancellationToken = default)
    {
        Error? failure = null;
        foreach (IAiAuditLogger sink in this._sinks)
        {
            Result written = await sink.WriteAsync(entry, cancellationToken).ConfigureAwait(false);
            failure ??= written.Failed ? written.Error : null;
        }

        return failure is null ? Result.Success() : Result.Failure(failure);
    }

    #endregion
}