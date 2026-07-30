// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AiOrchestrator.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="AiOrchestrator.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics;
using Navyblue.Foundation.Diagnostics;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Production AI orchestrator: Safety → Quota → Conversation → Domain → Intent → Resolve → Knowledge/Draft/Execute.
///     Cross-cutting concerns are supplied by <see cref="IAiPipelineBehavior" /> implementations.
/// </summary>
public sealed class AiOrchestrator(
    IDomainClassifier classifier,
    IIntentRecognizer recognizer,
    ISlotResolver resolver,
    IToolGateway gateway,
    IConfirmationService confirmationService,
    IDraftStore draftStore,
    IAiAuditLogger auditLogger,
    IKnowledgeRetriever knowledgeRetriever,
    IContentSafetyFilter safetyFilter,
    IPromptInjectionGuard injectionGuard,
    IAiQuotaGuard quotaGuard,
    IConversationStore conversationStore,
    NavyblueAiOptions options,
    IClock? clock = null,
    IEnumerable<IAiPipelineBehavior>? behaviors = null) : IAiOrchestrator
{
    private readonly IAiPipelineBehavior[] _behaviors = behaviors?.Reverse().ToArray() ?? [];
    private readonly IClock _clock = clock ?? new SystemClock();

    #region IAiOrchestrator Members

    /// <inheritdoc />
    public ValueTask<Result<AiPipelineResult>> ProcessAsync(UserUtterance utterance, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(utterance);

        Func<CancellationToken, ValueTask<Result<AiPipelineResult>>> next = ct => this.ProcessInstrumentedAsync(utterance, ct);
        foreach (IAiPipelineBehavior behavior in this._behaviors)
        {
            Func<CancellationToken, ValueTask<Result<AiPipelineResult>>> current = next;
            IAiPipelineBehavior captured = behavior;
            next = ct => captured.HandleAsync(utterance, current, ct);
        }

        return next(cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<Result<AiPipelineResult>> ConfirmDraftAsync(DraftConfirmationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using Activity? activity = AiTelemetry.StartActivity("ai.draft.confirm");
        activity?.SetTag("ai.draft.id", request.DraftId);

        // High-risk acknowledgement is validated before the draft state is mutated, so a rejected
        // acknowledgement does not burn a confirmation.
        if (request.Confirmed && options.RequireHighRiskAcknowledgement && !request.HighRiskAcknowledged)
        {
            OperationDraft? pending = await draftStore.GetAsync(request.DraftId, cancellationToken).ConfigureAwait(false);
            if (pending is { Operation.RiskLevel: ToolRiskLevel.HighRisk })
            {
                return Failure(new Error(
                    AiErrorCodes.DraftHighRiskAckRequired,
                    "High-risk operations require HighRiskAcknowledged = true."));
            }
        }

        Result<OperationDraft> confirm = await confirmationService.ConfirmAsync(request, cancellationToken).ConfigureAwait(false);
        if (confirm.Failed)
        {
            AiTelemetry.MarkFailed(activity, confirm.Error.Code, confirm.Error.Message);
            return Failure(confirm.Error);
        }

        OperationDraft draft = confirm.Value!;
        string? correlationId = CorrelationContext.Current;

        await this.AuditAsync(
            request.Confirmed ? AiAuditActions.DraftConfirmed : AiAuditActions.DraftRejected,
            draft.Operation.ToolName,
            draft.UserId,
            draft.TenantId,
            correlationId,
            true,
            request.DraftId,
            cancellationToken).ConfigureAwait(false);

        if (!request.Confirmed)
        {
            await this.ClearPendingDraftAsync(request.ConversationId, cancellationToken).ConfigureAwait(false);
            return Result.Success(AiPipelineResult.Failed("The operation was rejected by the user."));
        }

        if (options.RequireDoubleConfirmForHighRisk
            && draft.Operation.RiskLevel == ToolRiskLevel.HighRisk
            && draft.ConfirmationCount < 2)
        {
            return Result.Success(AiPipelineResult.DraftCreated(draft.DraftId, draft.Operation));
        }

        // Exactly one caller may proceed past this point, even under concurrent confirmations.
        Result<OperationDraft> claim = await draftStore.TryBeginExecutionAsync(draft.DraftId, cancellationToken).ConfigureAwait(false);
        if (claim.Failed)
        {
            AiTelemetry.MarkFailed(activity, claim.Error.Code, claim.Error.Message);
            return Failure(claim.Error);
        }

        Result<object?> executed = await gateway
            .ExecuteConfirmedAsync(draft.Operation, draft.UserId, draft.TenantId, correlationId, cancellationToken)
            .ConfigureAwait(false);

        if (executed.Failed)
        {
            await draftStore.MarkFailedAsync(draft.DraftId, executed.Error.Message, cancellationToken).ConfigureAwait(false);
            AiTelemetry.MarkFailed(activity, executed.Error.Code, executed.Error.Message);
            return Failure(executed.Error);
        }

        await draftStore.MarkExecutedAsync(draft.DraftId, cancellationToken).ConfigureAwait(false);
        await this.ClearPendingDraftAsync(request.ConversationId, cancellationToken).ConfigureAwait(false);
        return Result.Success(AiPipelineResult.Executed(executed.Value, draft.Operation).WithConversation(request.ConversationId));
    }

    /// <inheritdoc />
    public async ValueTask<Result<AiPipelineResult>> CancelDraftAsync(string draftId, string? userId = null, string? tenantId = null, CancellationToken cancellationToken = default)
    {
        Result<OperationDraft> cancelled = await draftStore.CancelAsync(draftId, userId, tenantId, cancellationToken).ConfigureAwait(false);
        if (cancelled.Failed)
        {
            return Failure(cancelled.Error);
        }

        OperationDraft draft = cancelled.Value!;
        await this.AuditAsync(
            AiAuditActions.DraftCancelled,
            draft.Operation.ToolName,
            draft.UserId,
            draft.TenantId,
            CorrelationContext.Current,
            true,
            draftId,
            cancellationToken).ConfigureAwait(false);

        return Result.Success(AiPipelineResult.Cancelled(draftId, draft.Operation));
    }

    #endregion

    private async ValueTask<Result<AiPipelineResult>> ProcessInstrumentedAsync(UserUtterance utterance, CancellationToken cancellationToken)
    {
        using Activity? activity = AiTelemetry.StartActivity("ai.pipeline.process");
        activity?.SetTag("ai.tenant.id", utterance.TenantId);
        activity?.SetTag("ai.conversation.id", utterance.ConversationId);

        long startedAt = Stopwatch.GetTimestamp();
        Result<AiPipelineResult> result = await this.ProcessCoreAsync(utterance, activity, cancellationToken).ConfigureAwait(false);

        double elapsedMs = (Stopwatch.GetTimestamp() - startedAt) * 1000d / Stopwatch.Frequency;
        AiTelemetry.PipelineDuration.Record(elapsedMs);
        AiTelemetry.PipelineRuns.Add(
            1,
            new KeyValuePair<string, object?>("status", result.Succeeded ? result.Value!.Status.ToString() : "Error"));

        return result;
    }

    private async ValueTask<Result<AiPipelineResult>> ProcessCoreAsync(UserUtterance utterance, Activity? activity, CancellationToken cancellationToken)
    {
        string? correlationId = CorrelationContext.Current;
        AiSafetyContext safetyContext = new(utterance.UserId, utterance.TenantId, Stage: "input");

        Result<string> inspected = await safetyFilter.InspectInputAsync(utterance.Text ?? string.Empty, safetyContext, cancellationToken).ConfigureAwait(false);
        if (inspected.Failed)
        {
            AiTelemetry.MarkFailed(activity, inspected.Error.Code, inspected.Error.Message);
            return Failure(inspected.Error);
        }

        Result quota = await quotaGuard
            .TryAcquireAsync(new AiQuotaRequest("pipeline", utterance.UserId, utterance.TenantId), cancellationToken)
            .ConfigureAwait(false);
        if (quota.Failed)
        {
            AiTelemetry.MarkFailed(activity, quota.Error.Code, quota.Error.Message);
            return Failure(quota.Error);
        }

        AiConversation? conversation = await this.LoadConversationAsync(utterance, cancellationToken).ConfigureAwait(false);
        UserUtterance effective = Merge(utterance, inspected.Value!, conversation);

        Result<AiPipelineResult> outcome = await this.RunStagesAsync(effective, correlationId, activity, cancellationToken).ConfigureAwait(false);
        await this.PersistConversationAsync(conversation, effective, outcome, cancellationToken).ConfigureAwait(false);

        if (outcome.Succeeded)
        {
            outcome.Value!.WithConversation(conversation?.ConversationId);
        }

        return outcome;
    }

    private async ValueTask<Result<AiPipelineResult>> RunStagesAsync(UserUtterance utterance, string? correlationId, Activity? activity, CancellationToken cancellationToken)
    {
        DomainClassificationResult classification = await classifier.ClassifyAsync(utterance, cancellationToken).ConfigureAwait(false);
        activity?.SetTag("ai.domain", classification.Domain);

        await this.AuditAsync(
            AiAuditActions.Classified,
            null,
            utterance.UserId,
            utterance.TenantId,
            correlationId,
            true,
            $"{classification.Domain}:{classification.Confidence:F2}",
            cancellationToken).ConfigureAwait(false);

        if (classification.RequiresClarification)
        {
            ClarificationRequest clarify = new(
                classification.Reason ?? "Please clarify which business domain you mean.",
                classification.CompetingDomains?.ToArray());
            return Result.Success(AiPipelineResult.Clarify(clarify, classification));
        }

        if (this.IsKnowledgeDomain(classification.Domain))
        {
            return await this.SearchKnowledgeAsync(utterance, classification, correlationId, cancellationToken).ConfigureAwait(false);
        }

        Result<IntentCandidate> recognized = await recognizer.RecognizeAsync(utterance, classification.Domain, cancellationToken).ConfigureAwait(false);
        if (recognized.Failed)
        {
            await this.AuditAsync(AiAuditActions.RecognizeFailed, null, utterance.UserId, utterance.TenantId, correlationId, false, recognized.Error.Message, cancellationToken).ConfigureAwait(false);
            return Result.Success(AiPipelineResult.Clarify(new ClarificationRequest(recognized.Error.Message), classification));
        }

        IntentCandidate candidate = recognized.Value!;
        activity?.SetTag("ai.intent", candidate.IntentName);

        if (candidate.Confidence < options.MinIntentConfidence)
        {
            return Result.Success(AiPipelineResult.Clarify(
                new ClarificationRequest("Intent confidence is too low. Please provide more detail."),
                classification));
        }

        Result<ResolvedOperation> resolved = await resolver.ResolveAsync(candidate, utterance, cancellationToken).ConfigureAwait(false);
        if (resolved.Failed)
        {
            await this.AuditAsync(AiAuditActions.ResolveFailed, null, utterance.UserId, utterance.TenantId, correlationId, false, resolved.Error.Message, cancellationToken).ConfigureAwait(false);
            return Result.Success(AiPipelineResult.Clarify(new ClarificationRequest(resolved.Error.Message), classification));
        }

        ResolvedOperation operation = resolved.Value!;
        if (operation.RiskLevel is ToolRiskLevel.Write or ToolRiskLevel.HighRisk)
        {
            OperationDraft draft = await draftStore
                .CreateAsync(operation, utterance.UserId, utterance.TenantId, options.DraftTimeToLive, cancellationToken)
                .ConfigureAwait(false);

            await this.AuditAsync(AiAuditActions.DraftCreated, operation.ToolName ?? operation.OperationName, utterance.UserId, utterance.TenantId, correlationId, true, draft.DraftId, cancellationToken).ConfigureAwait(false);
            return Result.Success(AiPipelineResult.DraftCreated(draft.DraftId, operation, candidate, classification));
        }

        Result<object?> executed = await gateway
            .InvokeResolvedAsync(operation, utterance.UserId, utterance.TenantId, correlationId, confirmed: false, cancellationToken)
            .ConfigureAwait(false);

        if (executed.Failed)
        {
            AiTelemetry.MarkFailed(activity, executed.Error.Code, executed.Error.Message);
            return Failure(executed.Error);
        }

        return Result.Success(AiPipelineResult.Executed(executed.Value, operation, candidate, classification));
    }

    private async ValueTask<Result<AiPipelineResult>> SearchKnowledgeAsync(UserUtterance utterance, DomainClassificationResult classification, string? correlationId, CancellationToken cancellationToken)
    {
        KnowledgeQuery query = new(utterance.Text, options.KnowledgeTopK, utterance.UserId, utterance.TenantId);
        Result<IReadOnlyList<KnowledgeHit>> retrieved = await knowledgeRetriever.SearchAsync(query, cancellationToken).ConfigureAwait(false);
        if (retrieved.Failed)
        {
            await this.AuditAsync(AiAuditActions.KnowledgeSearched, null, utterance.UserId, utterance.TenantId, correlationId, false, retrieved.Error.Message, cancellationToken).ConfigureAwait(false);
            return Failure(retrieved.Error);
        }

        AiSafetyContext safetyContext = new(utterance.UserId, utterance.TenantId, classification.Domain, "knowledge");
        Result<IReadOnlyList<KnowledgeHit>> screened = await injectionGuard.ScreenAsync(retrieved.Value!, safetyContext, cancellationToken).ConfigureAwait(false);
        if (screened.Failed)
        {
            return Failure(screened.Error);
        }

        IReadOnlyList<KnowledgeHit> hits = screened.Value!;
        await this.AuditAsync(AiAuditActions.KnowledgeSearched, null, utterance.UserId, utterance.TenantId, correlationId, true, $"hits={hits.Count}", cancellationToken).ConfigureAwait(false);

        ResolvedOperation knowledgeOperation = new(
            "KnowledgeSearch",
            classification.Domain,
            ToolRiskLevel.Read,
            new Dictionary<string, object?> { ["query"] = utterance.Text });

        return Result.Success(AiPipelineResult.Executed(hits, knowledgeOperation, classification: classification));
    }

    private async ValueTask<AiConversation?> LoadConversationAsync(UserUtterance utterance, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(utterance.ConversationId))
        {
            return null;
        }

        return await conversationStore
            .GetOrCreateAsync(utterance.ConversationId!, utterance.UserId, utterance.TenantId, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    ///     Rebuilds the effective utterance from conversation state: a reply to a pending clarification is
    ///     appended to the original question, and previously captured slots are merged into the context.
    /// </summary>
    private static UserUtterance Merge(UserUtterance utterance, string inspectedText, AiConversation? conversation)
    {
        string text = inspectedText;
        if (conversation is { PendingClarification: not null, PendingUtterance: { } previous } && !string.IsNullOrWhiteSpace(previous))
        {
            text = $"{previous} {inspectedText}".Trim();
        }

        if (conversation is null || conversation.Slots.Count == 0)
        {
            return utterance with { Text = text };
        }

        Dictionary<string, string> context = new(StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, string> slot in conversation.Slots)
        {
            context[slot.Key] = slot.Value;
        }

        if (utterance.Context is not null)
        {
            foreach (KeyValuePair<string, string> pair in utterance.Context)
            {
                context[pair.Key] = pair.Value;
            }
        }

        return utterance with { Text = text, Context = context };
    }

    private async ValueTask PersistConversationAsync(AiConversation? conversation, UserUtterance utterance, Result<AiPipelineResult> outcome, CancellationToken cancellationToken)
    {
        if (conversation is null)
        {
            return;
        }

        DateTimeOffset now = this._clock.UtcNow;
        conversation.AddTurn(new AiConversationTurn(AiConversationRoles.User, utterance.Text, now), options.MaxConversationTurns);

        if (outcome.Failed)
        {
            await conversationStore.SaveAsync(conversation, cancellationToken).ConfigureAwait(false);
            return;
        }

        AiPipelineResult result = outcome.Value!;
        conversation.LastDomain = result.Classification?.Domain ?? conversation.LastDomain;

        if (result.Candidate is { } candidate)
        {
            foreach (SlotValue slot in candidate.Slots)
            {
                if (!string.IsNullOrWhiteSpace(slot.RawValue))
                {
                    conversation.Slots[slot.Name] = slot.RawValue!;
                }
            }
        }

        switch (result.Status)
        {
            case AiPipelineStatus.Clarify:
                conversation.PendingClarification = result.Clarification;
                conversation.PendingUtterance = utterance.Text;
                break;

            case AiPipelineStatus.AwaitingConfirmation:
                conversation.ResolveClarification();
                conversation.PendingDraftId = result.DraftId;
                break;

            default:
                conversation.ResolveClarification();
                conversation.Slots.Clear();
                break;
        }

        conversation.AddTurn(
            new AiConversationTurn(
                AiConversationRoles.Assistant,
                result.Clarification?.Message ?? result.Status.ToString(),
                now,
                result.Classification?.Domain,
                result.Candidate?.IntentName,
                result.DraftId),
            options.MaxConversationTurns);

        await conversationStore.SaveAsync(conversation, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask ClearPendingDraftAsync(string? conversationId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(conversationId))
        {
            return;
        }

        AiConversation? conversation = await conversationStore.GetAsync(conversationId!, cancellationToken).ConfigureAwait(false);
        if (conversation is null)
        {
            return;
        }

        conversation.PendingDraftId = null;
        await conversationStore.SaveAsync(conversation, cancellationToken).ConfigureAwait(false);
    }

    private ValueTask<Result> AuditAsync(string action, string? toolName, string? userId, string? tenantId, string? correlationId, bool succeeded, string? detail, CancellationToken cancellationToken)
        => this.WriteAuditAsync(new AiAuditEntry(action, toolName, userId, tenantId, correlationId, succeeded, detail, this._clock.UtcNow), cancellationToken);

    private async ValueTask<Result> WriteAuditAsync(AiAuditEntry entry, CancellationToken cancellationToken)
    {
        try
        {
            Result written = await auditLogger.WriteAsync(entry, cancellationToken).ConfigureAwait(false);
            return written.Failed && options.AuditFailurePolicy == AuditFailurePolicy.FailClosed
                ? written
                : Result.Success();
        }
        catch (Exception ex) when (options.AuditFailurePolicy == AuditFailurePolicy.Continue)
        {
            _ = ex;
            return Result.Success();
        }
    }

    private bool IsKnowledgeDomain(string domain)
        => options.KnowledgeDomains.Any(d => string.Equals(d, domain, StringComparison.OrdinalIgnoreCase));

    private static Result<AiPipelineResult> Failure(Error error) => Result.Failure<AiPipelineResult>(error);
}