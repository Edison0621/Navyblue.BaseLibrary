// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AiModels.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="AiModels.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.AI;

/// <summary>
///     Framework-only domain codes. Business domains are application-defined strings via
///     <see cref="NavyblueAiOptions.DomainRules" /> — never baked into this package.
/// </summary>
public static class AiWellKnownDomains
{
    /// <summary>Domain could not be determined.</summary>
    public const string Unknown = "unknown";

    /// <summary>User clarification is required (ambiguous or low confidence).</summary>
    public const string Clarify = "clarify";

    /// <summary>
    ///     Returns true when <paramref name="domain" /> must not proceed to intent execution.
    /// </summary>
    public static bool RequiresClarification(string? domain)
        => string.IsNullOrWhiteSpace(domain)
           || string.Equals(domain, Unknown, StringComparison.OrdinalIgnoreCase)
           || string.Equals(domain, Clarify, StringComparison.OrdinalIgnoreCase);
}

/// <summary>
///     Keyword rule for <see cref="RuleBasedDomainClassifier" />. <see cref="Domain" /> is an application string.
/// </summary>
public sealed record DomainKeywordRule(string Domain, IReadOnlyList<string> Keywords)
{
    public DomainKeywordRule(string domain, params string[] keywords)
        : this(domain, (IReadOnlyList<string>)keywords)
    {
    }
}

/// <summary>
///     User utterance input for AI classification and recognition.
/// </summary>
/// <param name="Text">Raw user text.</param>
/// <param name="UserId">Acting user identity; required for ownership checks on write operations.</param>
/// <param name="TenantId">Acting tenant identity; used for tool scoping and knowledge isolation.</param>
/// <param name="ConversationId">Optional conversation for multi-turn clarification and slot carry-over.</param>
/// <param name="Context">Additional context passed to recognizer/resolver (carried slots are merged in).</param>
public sealed record UserUtterance(
    string Text,
    string? UserId = null,
    string? TenantId = null,
    string? ConversationId = null,
    IReadOnlyDictionary<string, string>? Context = null)
{
    /// <summary>
    ///     Creates an utterance from plain text.
    /// </summary>
    public static UserUtterance FromText(string text) => new(text);
}

/// <summary>
///     Result of domain classification. <see cref="Domain" /> is an application-defined code (or a well-known framework value).
/// </summary>
public sealed record DomainClassificationResult(
    string Domain,
    double Confidence,
    string? Reason = null,
    IReadOnlyList<string>? CompetingDomains = null)
{
    /// <summary>
    ///     Gets a value indicating whether clarification is required.
    /// </summary>
    public bool RequiresClarification => AiWellKnownDomains.RequiresClarification(this.Domain);
}

/// <summary>
///     Candidate slot produced by the LLM / recognizer (not final business parameters).
/// </summary>
public sealed record SlotValue(string Name, string? RawValue, double Confidence = 1.0);

/// <summary>
///     Candidate intent and slots. Final parameters must be produced by <see cref="ISlotResolver" />.
/// </summary>
public sealed record IntentCandidate(
    string IntentName,
    string Domain,
    double Confidence,
    IReadOnlyList<SlotValue> Slots,
    string? Reason = null);

/// <summary>
///     Clarification request when domain/intent confidence is insufficient.
/// </summary>
/// <param name="Message">Question presented to the user.</param>
/// <param name="Options">Candidate answers (e.g. competing domains) when known.</param>
/// <param name="MissingSlots">Slot names still required to complete the operation.</param>
public sealed record ClarificationRequest(
    string Message,
    IReadOnlyList<string>? Options = null,
    IReadOnlyList<string>? MissingSlots = null);

/// <summary>
///     Resolved command or query ready for tool execution (backend-owned parameters).
/// </summary>
/// <param name="IdempotencyKey">
///     Optional business idempotency key. When supplied, repeated confirmations of equivalent drafts can be de-duplicated
///     by the application's tool implementation or draft store.
/// </param>
public sealed record ResolvedOperation(
    string OperationName,
    string Domain,
    ToolRiskLevel RiskLevel,
    IReadOnlyDictionary<string, object?> Parameters,
    string? PermissionName = null,
    string? ToolName = null,
    string? IdempotencyKey = null);

/// <summary>
///     Outcome of the AI orchestration pipeline.
/// </summary>
public sealed class AiPipelineResult
{
    private AiPipelineResult(
        AiPipelineStatus status,
        DomainClassificationResult? classification = null,
        IntentCandidate? candidate = null,
        ResolvedOperation? operation = null,
        ClarificationRequest? clarification = null,
        object? executionResult = null,
        string? draftId = null,
        string? conversationId = null)
    {
        this.Status = status;
        this.Classification = classification;
        this.Candidate = candidate;
        this.Operation = operation;
        this.Clarification = clarification;
        this.ExecutionResult = executionResult;
        this.DraftId = draftId;
        this.ConversationId = conversationId;
    }

    public AiPipelineStatus Status { get; }
    public DomainClassificationResult? Classification { get; }
    public IntentCandidate? Candidate { get; }
    public ResolvedOperation? Operation { get; }
    public ClarificationRequest? Clarification { get; }
    public object? ExecutionResult { get; }
    public string? DraftId { get; }
    public string? ConversationId { get; private set; }

    public static AiPipelineResult Clarify(ClarificationRequest request, DomainClassificationResult? classification = null)
        => new(AiPipelineStatus.Clarify, classification, clarification: request);

    public static AiPipelineResult DraftCreated(string draftId, ResolvedOperation operation, IntentCandidate? candidate = null, DomainClassificationResult? classification = null)
        => new(AiPipelineStatus.AwaitingConfirmation, classification, candidate, operation, draftId: draftId);

    public static AiPipelineResult Executed(object? result, ResolvedOperation operation, IntentCandidate? candidate = null, DomainClassificationResult? classification = null)
        => new(AiPipelineStatus.Executed, classification, candidate, operation, executionResult: result);

    public static AiPipelineResult Failed(string message, DomainClassificationResult? classification = null)
        => new(AiPipelineStatus.Failed, classification, clarification: new ClarificationRequest(message));

    public static AiPipelineResult Cancelled(string draftId, ResolvedOperation? operation = null)
        => new(AiPipelineStatus.Cancelled, operation: operation, draftId: draftId);

    /// <summary>
    ///     Associates the result with a conversation. Used by the orchestrator when multi-turn state is enabled.
    /// </summary>
    public AiPipelineResult WithConversation(string? conversationId)
    {
        this.ConversationId = conversationId;
        return this;
    }
}

/// <summary>
///     AI pipeline status.
/// </summary>
public enum AiPipelineStatus
{
    Clarify = 0,
    AwaitingConfirmation = 1,
    Executed = 2,
    Failed = 3,
    Cancelled = 4
}

/// <summary>
///     LLM completion request (provider-agnostic).
/// </summary>
public sealed record LlmCompletionRequest(string Prompt, string? SystemPrompt = null, double Temperature = 0, int? MaxTokens = null);

/// <summary>
///     LLM completion response (provider-agnostic).
/// </summary>
/// <param name="PromptTokens">Prompt token usage when reported by the provider; used for budget accounting.</param>
/// <param name="CompletionTokens">Completion token usage when reported by the provider.</param>
public sealed record LlmCompletionResponse(
    string Text,
    double? Confidence = null,
    int? PromptTokens = null,
    int? CompletionTokens = null)
{
    /// <summary>
    ///     Gets total reported token usage, or zero when the provider does not report usage.
    /// </summary>
    public int TotalTokens => (this.PromptTokens ?? 0) + (this.CompletionTokens ?? 0);
}

/// <summary>
///     A streamed fragment of an LLM completion.
/// </summary>
public sealed record LlmChunk(string Delta, bool IsFinal = false, int? TokenCount = null);

/// <summary>
///     Tool risk level used by the gateway for draft/confirm gating.
/// </summary>
public enum ToolRiskLevel
{
    Read = 0,
    Write = 1,
    HighRisk = 2
}

/// <summary>
///     Declared parameter type for tool argument validation and MCP schema generation.
/// </summary>
public enum ToolParameterType
{
    String = 0,
    Integer = 1,
    Number = 2,
    Boolean = 3,
    DateTime = 4,
    Array = 5,
    Object = 6
}

/// <summary>
///     Declared tool parameter. Enables backend-side validation of arguments that originated from an LLM,
///     and JSON schema generation for MCP publication.
/// </summary>
public sealed record ToolParameterSpec(
    string Name,
    ToolParameterType Type = ToolParameterType.String,
    bool Required = false,
    string? Description = null,
    int? MaxLength = null,
    double? Minimum = null,
    double? Maximum = null,
    IReadOnlyList<string>? AllowedValues = null);

/// <summary>
///     Execution policy applied by <see cref="IToolGateway" /> around a single tool invocation.
/// </summary>
/// <param name="Timeout">Hard timeout for one attempt. Null uses the global default.</param>
/// <param name="MaxRetries">Retry attempts for transient failures (0 disables retry).</param>
/// <param name="RetryDelay">Base delay between retries; exponential backoff is applied.</param>
/// <param name="MaxConcurrency">Maximum concurrent executions of this tool across the process (0 = unlimited).</param>
/// <param name="CircuitBreakerFailureThreshold">Consecutive failures before the circuit opens (0 disables).</param>
/// <param name="CircuitBreakerDuration">How long the circuit stays open.</param>
public sealed record ToolExecutionPolicy(
    TimeSpan? Timeout = null,
    int MaxRetries = 0,
    TimeSpan? RetryDelay = null,
    int MaxConcurrency = 0,
    int CircuitBreakerFailureThreshold = 0,
    TimeSpan? CircuitBreakerDuration = null);

/// <summary>
///     Budget applied to a bounded AI run (agent loop or a single orchestration).
/// </summary>
/// <param name="MaxSteps">Maximum planner steps.</param>
/// <param name="MaxDuration">Maximum wall-clock duration for the whole run.</param>
/// <param name="MaxToolCalls">Maximum tool invocations.</param>
/// <param name="MaxLlmTokens">Maximum reported LLM tokens.</param>
public sealed record AiBudget(
    int? MaxSteps = null,
    TimeSpan? MaxDuration = null,
    int? MaxToolCalls = null,
    int? MaxLlmTokens = null);