// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AiTelemetry.cs
// Created          : 2026-07-30  15:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="AiTelemetry.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Navyblue.Foundation.AI;

/// <summary>
///     OpenTelemetry-compatible instrumentation for the AI pipeline.
///     Subscribe to <see cref="ActivitySourceName" /> for traces and <see cref="MeterName" /> for metrics.
/// </summary>
public static class AiTelemetry
{
    /// <summary>Activity source name used for pipeline and tool spans.</summary>
    public const string ActivitySourceName = "Navyblue.Foundation.AI";

    private static readonly Meter _meterInstance = new(MeterName, "3.1.0");

    /// <summary>Meter name used for pipeline and tool metrics.</summary>
    public const string MeterName = "Navyblue.Foundation.AI";

    /// <summary>Activity source for pipeline stages, tool calls, and agent steps.</summary>
    public static readonly ActivitySource Source = new(ActivitySourceName, "3.1.0");

    /// <summary>Counts completed pipeline runs by status.</summary>
    public static readonly Counter<long> PipelineRuns = _meterInstance.CreateCounter<long>("navyblue.ai.pipeline.runs");

    /// <summary>Measures pipeline latency in milliseconds.</summary>
    public static readonly Histogram<double> PipelineDuration = _meterInstance.CreateHistogram<double>("navyblue.ai.pipeline.duration", "ms");

    /// <summary>Counts tool invocations by tool name and outcome.</summary>
    public static readonly Counter<long> ToolInvocations = _meterInstance.CreateCounter<long>("navyblue.ai.tool.invocations");

    /// <summary>Measures tool execution latency in milliseconds.</summary>
    public static readonly Histogram<double> ToolDuration = _meterInstance.CreateHistogram<double>("navyblue.ai.tool.duration", "ms");

    /// <summary>Counts drafts by lifecycle transition.</summary>
    public static readonly Counter<long> DraftTransitions = _meterInstance.CreateCounter<long>("navyblue.ai.draft.transitions");

    /// <summary>Counts agent steps by outcome.</summary>
    public static readonly Counter<long> AgentSteps = _meterInstance.CreateCounter<long>("navyblue.ai.agent.steps");

    /// <summary>
    ///     Starts a pipeline/tool span. Returns null when no listener is attached (zero-cost path).
    /// </summary>
    public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
        => Source.StartActivity(name, kind);

    /// <summary>
    ///     Marks an activity as failed and records the error code.
    /// </summary>
    public static void MarkFailed(Activity? activity, string errorCode, string? message = null)
    {
        if (activity is null)
        {
            return;
        }

        activity.SetStatus(ActivityStatusCode.Error, message ?? errorCode);
        activity.SetTag("ai.error.code", errorCode);
    }
}