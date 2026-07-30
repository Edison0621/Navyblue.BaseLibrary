// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Governance.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="Governance.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Tool definition registered with <see cref="IToolRegistry" />.
/// </summary>
/// <param name="Parameters">
///     Declared parameters. When supplied, the gateway validates arguments against this contract before execution,
///     so LLM-produced values can never reach the executor unchecked.
/// </param>
/// <param name="Policy">Per-tool timeout / retry / circuit-breaker / concurrency policy. Null uses global defaults.</param>
/// <param name="TenantIds">Tenants allowed to see and call this tool. Null means all tenants.</param>
public sealed record ToolDefinition(
    string Name,
    string Description,
    ToolRiskLevel RiskLevel,
    string? PermissionName = null,
    Func<IReadOnlyDictionary<string, object?>, CancellationToken, ValueTask<object?>>? ExecuteAsync = null,
    string? InputSchemaJson = null,
    IReadOnlyList<string>? Tags = null,
    IReadOnlyList<ToolParameterSpec>? Parameters = null,
    ToolExecutionPolicy? Policy = null,
    IReadOnlyList<string>? TenantIds = null)
{
    /// <summary>
    ///     Returns true when the tool is visible to the supplied tenant.
    /// </summary>
    public bool IsVisibleTo(string? tenantId)
        => this.TenantIds is null
           || this.TenantIds.Count == 0
           || this.TenantIds.Any(t => string.Equals(t, tenantId, StringComparison.Ordinal));
}

/// <summary>
///     Tool invocation request.
/// </summary>
public sealed record ToolInvocation(
    string ToolName,
    IReadOnlyDictionary<string, object?> Arguments,
    string? UserId = null,
    string? TenantId = null,
    string? CorrelationId = null,
    bool Confirmed = false);

/// <summary>
///     AI audit log entry. Every tool call must be audited.
/// </summary>
public sealed record AiAuditEntry(
    string Action,
    string? ToolName,
    string? UserId,
    string? TenantId,
    string? CorrelationId,
    bool Succeeded,
    string? Detail = null,
    DateTimeOffset? OccurredAt = null);

/// <summary>
///     Draft for write/high-risk operations awaiting user confirmation.
/// </summary>
/// <param name="ConfirmedBy">Distinct users who have confirmed this draft, used for maker-checker enforcement.</param>
public sealed record OperationDraft(
    string DraftId,
    ResolvedOperation Operation,
    string? UserId,
    string? TenantId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt = null,
    DraftStatus Status = DraftStatus.Pending,
    int ConfirmationCount = 0,
    IReadOnlyList<string>? ConfirmedBy = null,
    string? FailureReason = null);

/// <summary>
///     Draft lifecycle status.
/// </summary>
public enum DraftStatus
{
    Pending = 0,
    Confirmed = 1,
    Rejected = 2,
    Expired = 3,
    Executed = 4,

    /// <summary>Execution has been atomically claimed by exactly one caller.</summary>
    Executing = 5,

    /// <summary>Execution was attempted and failed; the draft is terminal.</summary>
    Failed = 6,

    /// <summary>Cancelled by its owner before execution.</summary>
    Cancelled = 7
}

/// <summary>
///     Permission guard for AI tool calls. May adapt <c>IPermissionChecker</c>.
///     Register with a scoped lifetime when the underlying checker depends on the current user.
/// </summary>
public interface IPermissionGuard
{
    ValueTask<Result> EnsureGrantedAsync(string? permissionName, CancellationToken cancellationToken = default);
}

/// <summary>
///     Business rule validator for resolved operations before execution.
/// </summary>
public interface IBusinessRuleValidator
{
    ValueTask<Result> ValidateAsync(ResolvedOperation operation, CancellationToken cancellationToken = default);
}

/// <summary>
///     Validates tool arguments against the tool's declared <see cref="ToolDefinition.Parameters" />.
/// </summary>
public interface IToolArgumentValidator
{
    ValueTask<Result<IReadOnlyDictionary<string, object?>>> ValidateAsync(
        ToolDefinition tool,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken = default);
}

/// <summary>
///     Audit logger for AI / tool activity. Returns a result so the gateway can fail closed when auditing is unavailable.
/// </summary>
public interface IAiAuditLogger
{
    ValueTask<Result> WriteAsync(AiAuditEntry entry, CancellationToken cancellationToken = default);
}

/// <summary>
///     Confirmation request for a draft.
/// </summary>
/// <param name="ConversationId">Conversation this confirmation belongs to, so pending state can be cleared.</param>
public sealed record DraftConfirmationRequest(
    string DraftId,
    bool Confirmed,
    string? UserId = null,
    string? TenantId = null,
    bool HighRiskAcknowledged = false,
    string? ConversationId = null);

/// <summary>
///     Stores drafts for write/high-risk operations.
///     Implementations must make every state transition atomic: two concurrent confirmations of the same draft
///     must never both proceed to execution.
/// </summary>
public interface IDraftStore
{
    ValueTask<OperationDraft> CreateAsync(ResolvedOperation operation, string? userId, string? tenantId, TimeSpan? ttl = null, CancellationToken cancellationToken = default);

    ValueTask<OperationDraft?> GetAsync(string draftId, CancellationToken cancellationToken = default);

    ValueTask<Result<OperationDraft>> ConfirmAsync(DraftConfirmationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Atomically claims the draft for execution. Exactly one caller receives success; all others fail with
    ///     <see cref="AiErrorCodes.DraftAlreadyExecuting" />. This is the guard against duplicate writes.
    /// </summary>
    ValueTask<Result<OperationDraft>> TryBeginExecutionAsync(string draftId, CancellationToken cancellationToken = default);

    ValueTask MarkExecutedAsync(string draftId, CancellationToken cancellationToken = default);

    ValueTask MarkFailedAsync(string draftId, string reason, CancellationToken cancellationToken = default);

    ValueTask<Result<OperationDraft>> CancelAsync(string draftId, string? userId, string? tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes expired drafts. Returns the number purged.
    /// </summary>
    ValueTask<int> PurgeExpiredAsync(CancellationToken cancellationToken = default);
}

/// <summary>
///     User confirmation service for drafts (ownership-aware).
/// </summary>
public interface IConfirmationService
{
    ValueTask<Result<OperationDraft>> ConfirmAsync(DraftConfirmationRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
///     Process-wide catalog of callable tools. Registered as a singleton and populated at startup.
///     Separated from <see cref="IToolGateway" /> so the gateway itself can be scoped and safely depend on
///     per-request services such as the permission guard.
/// </summary>
public interface IToolRegistry
{
    void Register(ToolDefinition tool);

    bool TryRemove(string name);

    /// <summary>
    ///     Lists tools visible to the supplied tenant. Pass null to list tenant-agnostic tools only alongside global ones.
    /// </summary>
    IReadOnlyList<ToolDefinition> ListTools(string? tenantId = null);

    bool TryGetTool(string name, out ToolDefinition? tool);
}

/// <summary>
///     Tool gateway: validate → permission → rules → risk gate → execute → audit.
///     Agents and MCP must go through this. Register with a scoped lifetime in server applications.
/// </summary>
public interface IToolGateway
{
    IReadOnlyList<ToolDefinition> ListTools(string? tenantId = null);

    bool TryGetTool(string name, out ToolDefinition? tool);

    ValueTask<Result<object?>> InvokeAsync(ToolInvocation invocation, CancellationToken cancellationToken = default);

    ValueTask<Result<object?>> InvokeResolvedAsync(ResolvedOperation operation, string? userId = null, string? tenantId = null, string? correlationId = null, bool confirmed = false, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes a write/high-risk operation after draft confirmation (thread-safe; does not mutate shared state).
    /// </summary>
    ValueTask<Result<object?>> ExecuteConfirmedAsync(ResolvedOperation operation, string? userId = null, string? tenantId = null, string? correlationId = null, CancellationToken cancellationToken = default);
}

/// <summary>
///     MCP bridge that forwards tool calls to <see cref="IToolGateway" />. Must not access databases directly.
/// </summary>
public interface IMcpToolBridge
{
    IReadOnlyList<ToolDefinition> ListTools(string? tenantId = null);

    ValueTask<Result<object?>> InvokeToolAsync(string toolName, IReadOnlyDictionary<string, object?> arguments, string? userId = null, string? tenantId = null, string? correlationId = null, CancellationToken cancellationToken = default);
}