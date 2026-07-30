// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AiFakes.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="AiFakes.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Runtime.CompilerServices;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Deterministic LLM client for tests and offline demos.
/// </summary>
public sealed class FakeLlmClient(Func<LlmCompletionRequest, string>? responder = null) : IStreamingLlmClient
{
    private readonly List<LlmCompletionRequest> _requests = [];

    public IReadOnlyList<LlmCompletionRequest> Requests => this._requests;

    #region IStreamingLlmClient Members

    /// <inheritdoc />
    public ValueTask<Result<LlmCompletionResponse>> CompleteAsync(LlmCompletionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        this._requests.Add(request);
        string text = responder?.Invoke(request) ?? string.Empty;
        return ValueTask.FromResult(Result.Success(new LlmCompletionResponse(text, 1.0, request.Prompt.Length / 4, text.Length / 4)));
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<LlmChunk> CompleteStreamingAsync(LlmCompletionRequest request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Result<LlmCompletionResponse> completed = await this.CompleteAsync(request, cancellationToken).ConfigureAwait(false);
        string text = completed.Succeeded ? completed.Value!.Text : string.Empty;

        foreach (char character in text)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new LlmChunk(character.ToString());
        }

        yield return new LlmChunk(string.Empty, true, text.Length / 4);
    }

    #endregion
}

/// <summary>
///     Spy tool gateway that records invocations and optionally forwards to an inner gateway.
/// </summary>
public sealed class SpyToolGateway(IToolGateway? inner = null, IToolRegistry? registry = null) : IToolGateway
{
    private readonly IToolRegistry _registry = registry ?? new ToolRegistry();

    public List<ToolInvocation> Invocations { get; } = [];
    public List<ResolvedOperation> ResolvedInvocations { get; } = [];
    public List<ResolvedOperation> ConfirmedExecutions { get; } = [];

    #region IToolGateway Members

    /// <inheritdoc />
    public IReadOnlyList<ToolDefinition> ListTools(string? tenantId = null) => inner?.ListTools(tenantId) ?? this._registry.ListTools(tenantId);

    /// <inheritdoc />
    public bool TryGetTool(string name, out ToolDefinition? tool)
        => inner is not null ? inner.TryGetTool(name, out tool) : this._registry.TryGetTool(name, out tool);

    /// <inheritdoc />
    public async ValueTask<Result<object?>> InvokeAsync(ToolInvocation invocation, CancellationToken cancellationToken = default)
    {
        this.Invocations.Add(invocation);
        if (inner is not null)
        {
            return await inner.InvokeAsync(invocation, cancellationToken).ConfigureAwait(false);
        }

        if (!this._registry.TryGetTool(invocation.ToolName, out ToolDefinition? tool) || tool is null)
        {
            return Result.Failure<object?>(new Error(AiErrorCodes.ToolNotFound, $"Tool '{invocation.ToolName}' is not registered."));
        }

        if (!invocation.Confirmed && tool.RiskLevel is ToolRiskLevel.Write or ToolRiskLevel.HighRisk)
        {
            return Result.Failure<object?>(new Error(AiErrorCodes.ToolRequiresConfirmation, "Write or high-risk operations require draft confirmation."));
        }

        if (tool.ExecuteAsync is null)
        {
            return Result.Failure<object?>(new Error(AiErrorCodes.ToolNoExecutor, $"Tool '{tool.Name}' has no executor."));
        }

        return Result.Success(await tool.ExecuteAsync(invocation.Arguments, cancellationToken).ConfigureAwait(false));
    }

    /// <inheritdoc />
    public async ValueTask<Result<object?>> InvokeResolvedAsync(ResolvedOperation operation, string? userId = null, string? tenantId = null, string? correlationId = null, bool confirmed = false, CancellationToken cancellationToken = default)
    {
        this.ResolvedInvocations.Add(operation);
        if (!confirmed && operation.RiskLevel is ToolRiskLevel.Write or ToolRiskLevel.HighRisk)
        {
            return Result.Failure<object?>(new Error(AiErrorCodes.ToolRequiresConfirmation, "Write or high-risk operations require draft confirmation."));
        }

        if (inner is not null)
        {
            return await inner.InvokeResolvedAsync(operation, userId, tenantId, correlationId, confirmed, cancellationToken).ConfigureAwait(false);
        }

        return Result.Success<object?>(operation.Parameters);
    }

    /// <inheritdoc />
    public async ValueTask<Result<object?>> ExecuteConfirmedAsync(ResolvedOperation operation, string? userId = null, string? tenantId = null, string? correlationId = null, CancellationToken cancellationToken = default)
    {
        this.ConfirmedExecutions.Add(operation);
        return await this.InvokeResolvedAsync(operation, userId, tenantId, correlationId, confirmed: true, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    /// <summary>
    ///     Registers a tool in the spy's own registry (and the inner registry when one was supplied).
    /// </summary>
    public SpyToolGateway Register(ToolDefinition tool)
    {
        this._registry.Register(tool);
        return this;
    }
}

/// <summary>
///     Fixed intent recognizer for unit tests.
/// </summary>
public sealed class FixedIntentRecognizer : IIntentRecognizer
{
    private readonly Dictionary<string, IntentCandidate> _byDomain = new(StringComparer.OrdinalIgnoreCase);
    private IntentCandidate? _default;

    #region IIntentRecognizer Members

    /// <inheritdoc />
    public ValueTask<Result<IntentCandidate>> RecognizeAsync(UserUtterance utterance, string domain, CancellationToken cancellationToken = default)
    {
        if (this._byDomain.TryGetValue(domain, out IntentCandidate? candidate))
        {
            return ValueTask.FromResult(Result.Success(candidate));
        }

        if (this._default is not null)
        {
            return ValueTask.FromResult(Result.Success(this._default with { Domain = domain }));
        }

        return ValueTask.FromResult(Result.Failure<IntentCandidate>(new Error("ai.intent.unavailable", "No intent recognizer result configured.")));
    }

    #endregion

    public FixedIntentRecognizer ForDomain(string domain, IntentCandidate candidate)
    {
        this._byDomain[domain] = candidate;
        return this;
    }

    public FixedIntentRecognizer WithDefault(IntentCandidate candidate)
    {
        this._default = candidate;
        return this;
    }
}

/// <summary>
///     Fixed slot resolver for unit tests.
/// </summary>
public sealed class FixedSlotResolver(Func<IntentCandidate, UserUtterance, ResolvedOperation>? map = null) : ISlotResolver
{
    #region ISlotResolver Members

    /// <inheritdoc />
    public ValueTask<Result<ResolvedOperation>> ResolveAsync(IntentCandidate candidate, UserUtterance utterance, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        if (map is not null)
        {
            return ValueTask.FromResult(Result.Success(map(candidate, utterance)));
        }

        Dictionary<string, object?> parameters = candidate.Slots.ToDictionary(s => s.Name, s => (object?)s.RawValue, StringComparer.OrdinalIgnoreCase);
        ResolvedOperation operation = new(candidate.IntentName, candidate.Domain, ToolRiskLevel.Read, parameters, ToolName: candidate.IntentName);
        return ValueTask.FromResult(Result.Success(operation));
    }

    #endregion
}

/// <summary>
///     Intent recognizer that always fails until configured.
/// </summary>
public sealed class UnavailableIntentRecognizer : IIntentRecognizer
{
    #region IIntentRecognizer Members

    /// <inheritdoc />
    public ValueTask<Result<IntentCandidate>> RecognizeAsync(UserUtterance utterance, string domain, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Failure<IntentCandidate>(new Error(AiErrorCodes.IntentNotConfigured, "IIntentRecognizer is not configured.")));

    #endregion
}

/// <summary>
///     Slot resolver that always fails until configured.
/// </summary>
public sealed class UnavailableSlotResolver : ISlotResolver
{
    #region ISlotResolver Members

    /// <inheritdoc />
    public ValueTask<Result<ResolvedOperation>> ResolveAsync(IntentCandidate candidate, UserUtterance utterance, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Failure<ResolvedOperation>(new Error(AiErrorCodes.ResolveNotConfigured, "ISlotResolver is not configured.")));

    #endregion
}

/// <summary>
///     Empty knowledge retriever.
/// </summary>
public sealed class NullKnowledgeRetriever : IKnowledgeRetriever
{
    #region IKnowledgeRetriever Members

    /// <inheritdoc />
    public ValueTask<Result<IReadOnlyList<KnowledgeHit>>> SearchAsync(KnowledgeQuery query, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Success<IReadOnlyList<KnowledgeHit>>([]));

    #endregion
}

/// <summary>
///     In-memory knowledge retriever for demos/tests. Applies tenant isolation so demos mirror production rules.
/// </summary>
public sealed class InMemoryKnowledgeRetriever(IEnumerable<KnowledgeHit>? docs = null) : IKnowledgeRetriever
{
    private readonly List<KnowledgeHit> _docs = docs?.ToList() ?? [];

    #region IKnowledgeRetriever Members

    /// <inheritdoc />
    public ValueTask<Result<IReadOnlyList<KnowledgeHit>>> SearchAsync(KnowledgeQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        IReadOnlyList<KnowledgeHit> hits = this._docs
            .Where(d => string.IsNullOrWhiteSpace(d.TenantId) || string.Equals(d.TenantId, query.TenantId, StringComparison.Ordinal))
            .Where(d => d.Content.Contains(query.Query, StringComparison.OrdinalIgnoreCase) || d.Title.Contains(query.Query, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(d => d.Score)
            .Take(Math.Max(1, query.TopK))
            .ToArray();

        return ValueTask.FromResult(Result.Success(hits));
    }

    #endregion

    public void Add(KnowledgeHit hit) => this._docs.Add(hit);
}