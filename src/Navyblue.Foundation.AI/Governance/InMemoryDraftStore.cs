// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : InMemoryDraftStore.cs
// Created          : 2026-07-30  16:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="InMemoryDraftStore.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Single-node draft store with TTL, ownership, maker-checker, and atomic state transitions.
///     Every mutation is a compare-and-swap, so two concurrent confirmations can never both reach execution.
///     Replace with a durable store for multi-instance deployments.
/// </summary>
public sealed class InMemoryDraftStore(IClock? clock = null, NavyblueAiOptions? options = null) : IDraftStore
{
    private readonly IClock _clock = clock ?? new SystemClock();
    private readonly TimeSpan _defaultTtl = options?.DraftTimeToLive ?? TimeSpan.FromMinutes(30);
    private readonly ConcurrentDictionary<string, OperationDraft> _drafts = new(StringComparer.Ordinal);
    private readonly bool _requireDistinctApprovers = options?.RequireDistinctApprovers ?? false;

    public IReadOnlyCollection<OperationDraft> Drafts => this._drafts.Values.ToArray();

    #region IDraftStore Members

    /// <inheritdoc />
    public ValueTask<OperationDraft> CreateAsync(ResolvedOperation operation, string? userId, string? tenantId, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        DateTimeOffset now = this._clock.UtcNow;
        TimeSpan effectiveTtl = ttl ?? this._defaultTtl;
        string id = Guid.NewGuid().ToString("N");
        OperationDraft draft = new(id, operation, userId, tenantId, now, now.Add(effectiveTtl));
        this._drafts[id] = draft;
        AiTelemetry.DraftTransitions.Add(1, new KeyValuePair<string, object?>("transition", "created"));
        return ValueTask.FromResult(draft);
    }

    /// <inheritdoc />
    public ValueTask<OperationDraft?> GetAsync(string draftId, CancellationToken cancellationToken = default)
    {
        if (!this._drafts.TryGetValue(draftId, out OperationDraft? draft))
        {
            return ValueTask.FromResult<OperationDraft?>(null);
        }

        if (this.IsExpired(draft) && draft.Status == DraftStatus.Pending)
        {
            OperationDraft expired = draft with { Status = DraftStatus.Expired };
            this._drafts.TryUpdate(draftId, expired, draft);
            return ValueTask.FromResult<OperationDraft?>(expired);
        }

        return ValueTask.FromResult<OperationDraft?>(draft);
    }

    /// <inheritdoc />
    public ValueTask<Result<OperationDraft>> ConfirmAsync(DraftConfirmationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        while (true)
        {
            if (!this._drafts.TryGetValue(request.DraftId, out OperationDraft? current))
            {
                return ValueTask.FromResult(NotFound(request.DraftId));
            }

            if (this.IsExpired(current))
            {
                this._drafts.TryUpdate(request.DraftId, current with { Status = DraftStatus.Expired }, current);
                return ValueTask.FromResult(Fail(AiErrorCodes.DraftExpired, $"Draft '{request.DraftId}' has expired."));
            }

            if (current.Status is not (DraftStatus.Pending or DraftStatus.Confirmed))
            {
                return ValueTask.FromResult(Fail(AiErrorCodes.DraftInvalidState, $"Draft '{request.DraftId}' is {current.Status}."));
            }

            if (!OwnerMatches(current.TenantId, request.TenantId))
            {
                return ValueTask.FromResult(Fail(AiErrorCodes.DraftOwnerMismatch, "Draft ownership validation failed."));
            }

            IReadOnlyList<string> approvers = current.ConfirmedBy ?? [];

            // The first confirmation must come from the requesting user; later approvals come from reviewers.
            if (approvers.Count == 0 && !OwnerMatches(current.UserId, request.UserId))
            {
                return ValueTask.FromResult(Fail(AiErrorCodes.DraftOwnerMismatch, "Draft ownership validation failed."));
            }

            if (!request.Confirmed)
            {
                OperationDraft rejected = current with { Status = DraftStatus.Rejected };
                if (this._drafts.TryUpdate(request.DraftId, rejected, current))
                {
                    AiTelemetry.DraftTransitions.Add(1, new KeyValuePair<string, object?>("transition", "rejected"));
                    return ValueTask.FromResult(Result.Success(rejected));
                }

                continue;
            }

            if (this._requireDistinctApprovers
                && !string.IsNullOrWhiteSpace(request.UserId)
                && approvers.Any(a => string.Equals(a, request.UserId, StringComparison.Ordinal)))
            {
                return ValueTask.FromResult(Fail(
                    AiErrorCodes.DraftDistinctApproverRequired,
                    "This draft requires confirmation by a different approver."));
            }

            List<string> updatedApprovers = [.. approvers];
            if (!string.IsNullOrWhiteSpace(request.UserId))
            {
                updatedApprovers.Add(request.UserId!);
            }

            OperationDraft confirmed = current with
            {
                Status = DraftStatus.Confirmed,
                ConfirmationCount = current.ConfirmationCount + 1,
                ConfirmedBy = updatedApprovers
            };

            if (this._drafts.TryUpdate(request.DraftId, confirmed, current))
            {
                AiTelemetry.DraftTransitions.Add(1, new KeyValuePair<string, object?>("transition", "confirmed"));
                return ValueTask.FromResult(Result.Success(confirmed));
            }
        }
    }

    /// <inheritdoc />
    public ValueTask<Result<OperationDraft>> TryBeginExecutionAsync(string draftId, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            if (!this._drafts.TryGetValue(draftId, out OperationDraft? current))
            {
                return ValueTask.FromResult(NotFound(draftId));
            }

            if (current.Status != DraftStatus.Confirmed)
            {
                return ValueTask.FromResult(Fail(
                    AiErrorCodes.DraftAlreadyExecuting,
                    $"Draft '{draftId}' is {current.Status} and cannot be executed again."));
            }

            OperationDraft executing = current with { Status = DraftStatus.Executing };
            if (this._drafts.TryUpdate(draftId, executing, current))
            {
                AiTelemetry.DraftTransitions.Add(1, new KeyValuePair<string, object?>("transition", "executing"));
                return ValueTask.FromResult(Result.Success(executing));
            }
        }
    }

    /// <inheritdoc />
    public ValueTask MarkExecutedAsync(string draftId, CancellationToken cancellationToken = default)
    {
        this.Transition(draftId, d => d with { Status = DraftStatus.Executed });
        AiTelemetry.DraftTransitions.Add(1, new KeyValuePair<string, object?>("transition", "executed"));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask MarkFailedAsync(string draftId, string reason, CancellationToken cancellationToken = default)
    {
        this.Transition(draftId, d => d with { Status = DraftStatus.Failed, FailureReason = reason });
        AiTelemetry.DraftTransitions.Add(1, new KeyValuePair<string, object?>("transition", "failed"));
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask<Result<OperationDraft>> CancelAsync(string draftId, string? userId, string? tenantId, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            if (!this._drafts.TryGetValue(draftId, out OperationDraft? current))
            {
                return ValueTask.FromResult(NotFound(draftId));
            }

            if (!OwnerMatches(current.UserId, userId) || !OwnerMatches(current.TenantId, tenantId))
            {
                return ValueTask.FromResult(Fail(AiErrorCodes.DraftOwnerMismatch, "Draft ownership validation failed."));
            }

            if (current.Status is not (DraftStatus.Pending or DraftStatus.Confirmed))
            {
                return ValueTask.FromResult(Fail(AiErrorCodes.DraftInvalidState, $"Draft '{draftId}' is {current.Status}."));
            }

            OperationDraft cancelled = current with { Status = DraftStatus.Cancelled };
            if (this._drafts.TryUpdate(draftId, cancelled, current))
            {
                AiTelemetry.DraftTransitions.Add(1, new KeyValuePair<string, object?>("transition", "cancelled"));
                return ValueTask.FromResult(Result.Success(cancelled));
            }
        }
    }

    /// <inheritdoc />
    public ValueTask<int> PurgeExpiredAsync(CancellationToken cancellationToken = default)
    {
        int purged = 0;
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (KeyValuePair<string, OperationDraft> pair in this._drafts)
        {
            bool terminal = pair.Value.Status is DraftStatus.Executed or DraftStatus.Failed or DraftStatus.Rejected or DraftStatus.Cancelled or DraftStatus.Expired;
            if ((this.IsExpired(pair.Value) || terminal) && this._drafts.TryRemove(pair))
            {
                purged++;
            }
        }

        return ValueTask.FromResult(purged);
    }

    #endregion

    private void Transition(string draftId, Func<OperationDraft, OperationDraft> mutate)
    {
        while (this._drafts.TryGetValue(draftId, out OperationDraft? current))
        {
            if (this._drafts.TryUpdate(draftId, mutate(current), current))
            {
                return;
            }
        }
    }

    private bool IsExpired(OperationDraft draft)
        => draft.ExpiresAt is { } expiresAt && expiresAt <= this._clock.UtcNow;

    private static Result<OperationDraft> NotFound(string draftId)
        => Fail(AiErrorCodes.DraftNotFound, $"Draft '{draftId}' was not found.");

    private static Result<OperationDraft> Fail(string code, string message)
        => Result.Failure<OperationDraft>(new Error(code, message));

    private static bool OwnerMatches(string? expected, string? actual)
    {
        if (string.IsNullOrWhiteSpace(expected))
        {
            return true; // draft created without owner — cannot enforce
        }

        return string.Equals(expected, actual, StringComparison.Ordinal);
    }
}