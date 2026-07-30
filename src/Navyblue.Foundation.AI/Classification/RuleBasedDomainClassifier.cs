// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : RuleBasedDomainClassifier.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="RuleBasedDomainClassifier.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.AI;

/// <summary>
///     Configurable keyword-based domain classifier.
///     Uses application-defined <see cref="NavyblueAiOptions.DomainRules" /> only — no baked-in business domains.
/// </summary>
public sealed class RuleBasedDomainClassifier(NavyblueAiOptions? options = null) : IDomainClassifier
{
    private readonly double _minConfidence = options?.MinDomainConfidence ?? 0.6;
    private readonly IReadOnlyList<DomainKeywordRule> _rules = options?.DomainRules.ToArray() ?? [];

    #region IDomainClassifier Members

    /// <inheritdoc />
    public ValueTask<DomainClassificationResult> ClassifyAsync(UserUtterance utterance, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(utterance);
        string text = utterance.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(text))
        {
            return ValueTask.FromResult(new DomainClassificationResult(AiWellKnownDomains.Clarify, 0, "Empty utterance.", []));
        }

        if (this._rules.Count == 0)
        {
            return ValueTask.FromResult(new DomainClassificationResult(
                AiWellKnownDomains.Clarify,
                0,
                "No domain rules configured. Register application domain rules via NavyblueAiOptions.DomainRules.",
                []));
        }

        List<(string Domain, int Hits)> scored = [];
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (DomainKeywordRule rule in this._rules)
        {
            if (string.IsNullOrWhiteSpace(rule.Domain) || AiWellKnownDomains.RequiresClarification(rule.Domain))
            {
                continue;
            }

            int hits = rule.Keywords.Count(k => !string.IsNullOrWhiteSpace(k) && text.Contains(k, StringComparison.OrdinalIgnoreCase));
            if (hits > 0)
            {
                scored.Add((rule.Domain, hits));
            }
        }

        if (scored.Count == 0)
        {
            return ValueTask.FromResult(new DomainClassificationResult(AiWellKnownDomains.Clarify, 0, "No domain keywords matched; clarification required.", []));
        }

        scored.Sort((a, b) => b.Hits.CompareTo(a.Hits));
        (string topDomain, int topHits) = scored[0];
        List<string> competing = scored.Where(x => !string.Equals(x.Domain, topDomain, StringComparison.OrdinalIgnoreCase) && x.Hits == topHits)
            .Select(x => x.Domain)
            .ToList();

        if (competing.Count > 0)
        {
            string[] all = [topDomain, .. competing];
            return ValueTask.FromResult(new DomainClassificationResult(
                AiWellKnownDomains.Clarify,
                0.4,
                "Multiple domains matched with equal strength.",
                all));
        }

        double confidence = Math.Min(1.0, 0.5 + topHits * 0.2);
        if (confidence < this._minConfidence)
        {
            return ValueTask.FromResult(new DomainClassificationResult(
                AiWellKnownDomains.Clarify,
                confidence,
                $"Confidence {confidence:F2} below threshold {this._minConfidence:F2}.",
                [topDomain]));
        }

        return ValueTask.FromResult(new DomainClassificationResult(topDomain, confidence, $"Matched {topHits} keyword(s).", []));
    }

    #endregion
}