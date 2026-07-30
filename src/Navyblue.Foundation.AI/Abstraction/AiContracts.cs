// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AiContracts.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="AiContracts.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Classifies user utterance into a business domain before intent recognition.
/// </summary>
public interface IDomainClassifier
{
    ValueTask<DomainClassificationResult> ClassifyAsync(UserUtterance utterance, CancellationToken cancellationToken = default);
}

/// <summary>
///     Produces candidate intents and slots only. Must not emit final business parameters.
/// </summary>
public interface IIntentRecognizer
{
    ValueTask<Result<IntentCandidate>> RecognizeAsync(UserUtterance utterance, string domain, CancellationToken cancellationToken = default);
}

/// <summary>
///     Resolves candidate slots into a final command/query via schema, master data, and business rules.
/// </summary>
public interface ISlotResolver
{
    ValueTask<Result<ResolvedOperation>> ResolveAsync(IntentCandidate candidate, UserUtterance utterance, CancellationToken cancellationToken = default);
}

/// <summary>
///     Provider-agnostic LLM client. Implementations may wrap any model API.
/// </summary>
public interface ILlmClient
{
    ValueTask<Result<LlmCompletionResponse>> CompleteAsync(LlmCompletionRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
///     Optional streaming capability for chat-style user experiences.
///     Implement alongside <see cref="ILlmClient" /> when the provider supports token streaming.
/// </summary>
public interface IStreamingLlmClient : ILlmClient
{
    IAsyncEnumerable<LlmChunk> CompleteStreamingAsync(LlmCompletionRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
///     Orchestrates Classify → Recognize → Resolve → Draft/Execute (with optional pipeline behaviors).
/// </summary>
public interface IAiOrchestrator
{
    ValueTask<Result<AiPipelineResult>> ProcessAsync(UserUtterance utterance, CancellationToken cancellationToken = default);

    ValueTask<Result<AiPipelineResult>> ConfirmDraftAsync(DraftConfirmationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Cancels a pending draft. Only the owning user/tenant may cancel.
    /// </summary>
    ValueTask<Result<AiPipelineResult>> CancelDraftAsync(string draftId, string? userId = null, string? tenantId = null, CancellationToken cancellationToken = default);
}

/// <summary>
///     Cross-cutting pipeline behavior around <see cref="IAiOrchestrator.ProcessAsync" />.
/// </summary>
public interface IAiPipelineBehavior
{
    ValueTask<Result<AiPipelineResult>> HandleAsync(
        UserUtterance utterance,
        Func<CancellationToken, ValueTask<Result<AiPipelineResult>>> next,
        CancellationToken cancellationToken = default);
}