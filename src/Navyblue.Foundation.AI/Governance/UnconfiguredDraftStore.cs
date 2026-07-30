// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : UnconfiguredDraftStore.cs
// Created          : 2026-07-30  16:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="UnconfiguredDraftStore.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Draft store that refuses every operation. Registered as the production default so a deployment that
///     forgot to configure durable draft storage cannot silently lose pending write operations, and so a
///     draft created on one node is never "lost" by being confirmed on another.
/// </summary>
public sealed class UnconfiguredDraftStore : IDraftStore
{
    private static Error NotConfigured => new(
        AiErrorCodes.ConfigurationInvalid,
        "No durable IDraftStore is registered. Register one, or set NavyblueAiOptions.AllowInMemoryPersistence for non-production use.");

    #region IDraftStore Members

    /// <inheritdoc />
    public ValueTask<OperationDraft> CreateAsync(ResolvedOperation operation, string? userId, string? tenantId, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
        => throw new InvalidOperationException(NotConfigured.Message);

    /// <inheritdoc />
    public ValueTask<OperationDraft?> GetAsync(string draftId, CancellationToken cancellationToken = default)
        => ValueTask.FromResult<OperationDraft?>(null);

    /// <inheritdoc />
    public ValueTask<Result<OperationDraft>> ConfirmAsync(DraftConfirmationRequest request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Failure<OperationDraft>(NotConfigured));

    /// <inheritdoc />
    public ValueTask<Result<OperationDraft>> TryBeginExecutionAsync(string draftId, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Failure<OperationDraft>(NotConfigured));

    /// <inheritdoc />
    public ValueTask MarkExecutedAsync(string draftId, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

    /// <inheritdoc />
    public ValueTask MarkFailedAsync(string draftId, string reason, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

    /// <inheritdoc />
    public ValueTask<Result<OperationDraft>> CancelAsync(string draftId, string? userId, string? tenantId, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Failure<OperationDraft>(NotConfigured));

    /// <inheritdoc />
    public ValueTask<int> PurgeExpiredAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult(0);

    #endregion
}