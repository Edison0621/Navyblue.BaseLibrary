// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ContentSafety.cs
// Created          : 2026-07-30  15:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="ContentSafety.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Text.RegularExpressions;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Context passed to safety filters.
/// </summary>
public sealed record AiSafetyContext(string? UserId = null, string? TenantId = null, string? Domain = null, string? Stage = null);

/// <summary>
///     Inspects inbound user text and redacts outbound content.
///     Registered as a pipeline-level hook so applications can plug in their own moderation or DLP service.
/// </summary>
public interface IContentSafetyFilter
{
    /// <summary>
    ///     Inspects user input before classification. Return a failure to block the request.
    ///     The returned string is the (possibly normalized) text used downstream.
    /// </summary>
    ValueTask<Result<string>> InspectInputAsync(string text, AiSafetyContext context, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Redacts sensitive values from text that will be shown to the user or fed back into a prompt.
    /// </summary>
    ValueTask<string> RedactAsync(string text, AiSafetyContext context, CancellationToken cancellationToken = default);
}

/// <summary>
///     Screens retrieved knowledge before it is used as prompt context, guarding against prompt injection
///     embedded in documents ("ignore previous instructions", tool-call directives, and similar).
/// </summary>
public interface IPromptInjectionGuard
{
    ValueTask<Result<IReadOnlyList<KnowledgeHit>>> ScreenAsync(IReadOnlyList<KnowledgeHit> hits, AiSafetyContext context, CancellationToken cancellationToken = default);
}

/// <summary>
///     Pass-through safety filter. Default when the application registers no moderation provider.
/// </summary>
public sealed class NullContentSafetyFilter : IContentSafetyFilter
{
    #region IContentSafetyFilter Members

    /// <inheritdoc />
    public ValueTask<Result<string>> InspectInputAsync(string text, AiSafetyContext context, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Success(text));

    /// <inheritdoc />
    public ValueTask<string> RedactAsync(string text, AiSafetyContext context, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(text);

    #endregion
}

/// <summary>
///     Regex-based safety filter: blocks input matching configured deny patterns and masks matches of redaction patterns.
///     Intended as a usable default for PII masking; production deployments may replace it with a moderation service.
/// </summary>
public sealed class RegexContentSafetyFilter(IEnumerable<string>? blockPatterns = null, IEnumerable<string>? redactPatterns = null) : IContentSafetyFilter
{
    private readonly IReadOnlyList<Regex> _blockPatterns = Compile(blockPatterns);
    private readonly IReadOnlyList<Regex> _redactPatterns = Compile(redactPatterns);

    #region IContentSafetyFilter Members

    /// <inheritdoc />
    public ValueTask<Result<string>> InspectInputAsync(string text, AiSafetyContext context, CancellationToken cancellationToken = default)
    {
        string value = text ?? string.Empty;
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (Regex pattern in this._blockPatterns)
        {
            if (pattern.IsMatch(value))
            {
                return ValueTask.FromResult(Result.Failure<string>(new Error(
                    AiErrorCodes.ContentBlocked,
                    "The request was blocked by the content safety policy.")));
            }
        }

        return ValueTask.FromResult(Result.Success(value));
    }

    /// <inheritdoc />
    public ValueTask<string> RedactAsync(string text, AiSafetyContext context, CancellationToken cancellationToken = default)
    {
        string value = text ?? string.Empty;
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (Regex pattern in this._redactPatterns)
        {
            value = pattern.Replace(value, "***");
        }

        return ValueTask.FromResult(value);
    }

    #endregion

    private static IReadOnlyList<Regex> Compile(IEnumerable<string>? patterns)
        => patterns?.Where(p => !string.IsNullOrWhiteSpace(p))
               .Select(p => new Regex(p, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
               .ToArray()
           ?? [];
}

/// <summary>
///     Pass-through injection guard.
/// </summary>
public sealed class NullPromptInjectionGuard : IPromptInjectionGuard
{
    #region IPromptInjectionGuard Members

    /// <inheritdoc />
    public ValueTask<Result<IReadOnlyList<KnowledgeHit>>> ScreenAsync(IReadOnlyList<KnowledgeHit> hits, AiSafetyContext context, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Success(hits));

    #endregion
}

/// <summary>
///     Drops retrieved documents containing instruction-injection markers instead of feeding them to the model.
/// </summary>
public sealed class KeywordPromptInjectionGuard(IEnumerable<string>? markers = null) : IPromptInjectionGuard
{
    private static readonly string[] _defaultMarkers =
    [
        "ignore previous instructions",
        "ignore all previous",
        "disregard the above",
        "you are now",
        "system prompt:",
        "忽略以上指令",
        "忽略之前的指令",
        "忽略前面的所有"
    ];

    private readonly string[] _markers = markers?.ToArray() ?? _defaultMarkers;

    #region IPromptInjectionGuard Members

    /// <inheritdoc />
    public ValueTask<Result<IReadOnlyList<KnowledgeHit>>> ScreenAsync(IReadOnlyList<KnowledgeHit> hits, AiSafetyContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(hits);

        IReadOnlyList<KnowledgeHit> safe = hits
            .Where(h => !this._markers.Any(m => h.Content.Contains(m, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        return ValueTask.FromResult(Result.Success(safe));
    }

    #endregion
}