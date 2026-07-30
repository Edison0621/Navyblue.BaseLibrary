// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AiErrorCodes.cs
// Created          : 2026-07-30  15:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="AiErrorCodes.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.AI;

/// <summary>
///     Stable error codes emitted by the AI pipeline. Applications may map these to user-facing messages.
/// </summary>
public static class AiErrorCodes
{
    // Pipeline
    public const string IntentNotConfigured = "ai.intent.not_configured";
    public const string ResolveNotConfigured = "ai.resolve.not_configured";
    public const string PlannerNotConfigured = "ai.agent.planner_not_configured";
    public const string ConfigurationInvalid = "ai.configuration.invalid";

    // Permission / safety
    public const string PermissionDenied = "ai.permission.denied";
    public const string PermissionGuardNotConfigured = "ai.permission.guard_not_configured";
    public const string ContentBlocked = "ai.safety.content_blocked";
    public const string PromptInjectionDetected = "ai.safety.prompt_injection";

    // Tools
    public const string ToolNotFound = "ai.tool.not_found";
    public const string ToolNoExecutor = "ai.tool.no_executor";
    public const string ToolRequiresConfirmation = "ai.tool.requires_confirmation";
    public const string ToolFailed = "ai.tool.failed";
    public const string ToolNonRetryable = "ai.tool.non_retryable";
    public const string ToolTimeout = "ai.tool.timeout";
    public const string ToolCircuitOpen = "ai.tool.circuit_open";
    public const string ToolTenantDenied = "ai.tool.tenant_denied";
    public const string ToolArgumentInvalid = "ai.tool.argument_invalid";

    // Drafts
    public const string DraftNotFound = "ai.draft.not_found";
    public const string DraftExpired = "ai.draft.expired";
    public const string DraftInvalidState = "ai.draft.invalid_state";
    public const string DraftOwnerMismatch = "ai.draft.owner_mismatch";
    public const string DraftHighRiskAckRequired = "ai.draft.high_risk_ack_required";
    public const string DraftDistinctApproverRequired = "ai.draft.distinct_approver_required";
    public const string DraftAlreadyExecuting = "ai.draft.already_executing";

    // Audit
    public const string AuditNotConfigured = "ai.audit.not_configured";
    public const string AuditWriteFailed = "ai.audit.write_failed";

    // Budget / quota
    public const string QuotaExceeded = "ai.quota.exceeded";
    public const string BudgetExceeded = "ai.budget.exceeded";

    // Knowledge
    public const string KnowledgeFailed = "ai.knowledge.failed";
}

/// <summary>
///     Audit action names written by the framework.
/// </summary>
public static class AiAuditActions
{
    public const string Classified = "ai.classify";
    public const string RecognizeFailed = "ai.recognize.failed";
    public const string ResolveFailed = "ai.resolve.failed";
    public const string KnowledgeSearched = "ai.knowledge.searched";
    public const string DraftCreated = "ai.draft.created";
    public const string DraftConfirmed = "ai.draft.confirmed";
    public const string DraftRejected = "ai.draft.rejected";
    public const string DraftCancelled = "ai.draft.cancelled";
    public const string ToolMissing = "tool.missing";
    public const string ToolPermissionDenied = "tool.permission_denied";
    public const string ToolRuleFailed = "tool.rule_failed";
    public const string ToolArgumentInvalid = "tool.argument_invalid";
    public const string ToolWriteBlocked = "tool.write_blocked";
    public const string ToolNoExecutor = "tool.no_executor";
    public const string ToolExecuting = "tool.executing";
    public const string ToolExecuted = "tool.executed";
    public const string ToolFailed = "tool.failed";
    public const string AgentPlanFailed = "ai.agent.plan_failed";
    public const string AgentBudgetExceeded = "ai.agent.budget_exceeded";
}