// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IntentEvaluation.cs
// Created          : 2026-07-30  16:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="IntentEvaluation.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Text;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     A single golden-set expectation. Negative samples (those that must clarify rather than guess a domain)
///     are expressed with <see cref="ExpectClarify" />.
/// </summary>
public sealed record IntentEvaluationCase(
    string Utterance,
    string? ExpectedDomain = null,
    string? ExpectedIntent = null,
    bool ExpectClarify = false);

/// <summary>
///     Outcome for a single evaluation case.
/// </summary>
public sealed record IntentEvaluationOutcome(
    IntentEvaluationCase Case,
    string ActualDomain,
    double DomainConfidence,
    string? ActualIntent,
    bool Passed,
    string? FailureReason);

/// <summary>
///     Aggregate result of a golden-set run.
/// </summary>
public sealed record IntentEvaluationReport(IReadOnlyList<IntentEvaluationOutcome> Outcomes)
{
    public int Total => this.Outcomes.Count;
    public int Passed => this.Outcomes.Count(o => o.Passed);
    public int Failed => this.Total - this.Passed;
    public double Accuracy => this.Total == 0 ? 1d : (double)this.Passed / this.Total;

    /// <summary>
    ///     Renders the failures in a form suitable for an assertion message.
    /// </summary>
    public string Describe()
    {
        StringBuilder builder = new();
        builder.AppendLine($"Intent evaluation: {this.Passed}/{this.Total} passed (accuracy {this.Accuracy:P1}).");
        foreach (IntentEvaluationOutcome outcome in this.Outcomes.Where(o => !o.Passed))
        {
            builder.AppendLine($"  ✗ \"{outcome.Case.Utterance}\" → domain={outcome.ActualDomain} ({outcome.DomainConfidence:F2}), intent={outcome.ActualIntent ?? "-"}: {outcome.FailureReason}");
        }

        return builder.ToString();
    }
}

/// <summary>
///     Runs a golden set of utterances through classification (and optionally intent recognition) so that
///     regressions — especially negative samples that must clarify instead of guessing — fail the build.
/// </summary>
public sealed class IntentEvaluationHarness(IDomainClassifier classifier, IIntentRecognizer? recognizer = null)
{
    public async ValueTask<IntentEvaluationReport> RunAsync(IEnumerable<IntentEvaluationCase> cases, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cases);

        List<IntentEvaluationOutcome> outcomes = [];
        foreach (IntentEvaluationCase testCase in cases)
        {
            DomainClassificationResult classification = await classifier
                .ClassifyAsync(new UserUtterance(testCase.Utterance), cancellationToken)
                .ConfigureAwait(false);

            string? intent = null;
            string? failure = null;

            if (testCase.ExpectClarify)
            {
                if (!classification.RequiresClarification)
                {
                    failure = $"expected clarification but got domain '{classification.Domain}'";
                }
            }
            else if (classification.RequiresClarification)
            {
                failure = $"expected domain '{testCase.ExpectedDomain}' but clarification was requested";
            }
            else if (testCase.ExpectedDomain is not null
                     && !string.Equals(classification.Domain, testCase.ExpectedDomain, StringComparison.OrdinalIgnoreCase))
            {
                failure = $"expected domain '{testCase.ExpectedDomain}'";
            }

            if (failure is null && testCase.ExpectedIntent is not null && recognizer is not null)
            {
                Result<IntentCandidate> recognized = await recognizer
                    .RecognizeAsync(new UserUtterance(testCase.Utterance), classification.Domain, cancellationToken)
                    .ConfigureAwait(false);

                if (recognized.Failed)
                {
                    failure = $"intent recognition failed: {recognized.Error.Message}";
                }
                else
                {
                    intent = recognized.Value!.IntentName;
                    if (!string.Equals(intent, testCase.ExpectedIntent, StringComparison.OrdinalIgnoreCase))
                    {
                        failure = $"expected intent '{testCase.ExpectedIntent}'";
                    }
                }
            }

            outcomes.Add(new IntentEvaluationOutcome(
                testCase,
                classification.Domain,
                classification.Confidence,
                intent,
                failure is null,
                failure));
        }

        return new IntentEvaluationReport(outcomes);
    }
}