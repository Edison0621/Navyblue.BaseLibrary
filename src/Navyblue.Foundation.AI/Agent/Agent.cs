// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Agent.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="Agent.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics;
using System.Globalization;
using Navyblue.Foundation.Diagnostics;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Agent request for multi-step tool orchestration.
/// </summary>
/// <param name="Budget">Optional budget. Falls back to <see cref="NavyblueAiOptions.DefaultAgentBudget" />.</param>
public sealed record AiAgentRequest(
    string Goal,
    string? UserId = null,
    string? TenantId = null,
    string? ConversationId = null,
    IReadOnlyDictionary<string, object?>? InitialArguments = null,
    int? MaxSteps = null,
    AiBudget? Budget = null);

/// <summary>
///     One step taken by the agent.
/// </summary>
public sealed record AiAgentStep(
    int Index,
    string ToolName,
    IReadOnlyDictionary<string, object?> Arguments,
    bool Succeeded,
    object? Result = null,
    string? Error = null);

/// <summary>
///     Reasons an agent run stopped.
/// </summary>
public static class AiAgentStopReasons
{
    public const string PlannerDone = "planner_done";
    public const string WriteRequiresConfirmation = "write_requires_confirmation";
    public const string ToolFailed = "tool_failed";
    public const string StepLimit = "step_limit";
    public const string BudgetExceeded = "budget_exceeded";
    public const string LoopDetected = "loop_detected";
}

/// <summary>
///     Agent run result.
/// </summary>
public sealed class AiAgentResult
{
    public bool Completed { get; init; }
    public IReadOnlyList<AiAgentStep> Steps { get; init; } = [];
    public object? FinalResult { get; init; }
    public string? StopReason { get; init; }
}

/// <summary>
///     Plans the next tool call for an agent. LLM-backed planners must only emit tool names + candidate args.
/// </summary>
public interface IAgentPlanner
{
    ValueTask<Result<AgentPlan>> PlanNextAsync(AiAgentRequest request, IReadOnlyList<AiAgentStep> history, IReadOnlyList<ToolDefinition> availableTools, CancellationToken cancellationToken = default);
}

/// <summary>
///     Planned tool invocation (candidate only; gateway still enforces permission/rules/confirm).
/// </summary>
public sealed record AgentPlan(string ToolName, IReadOnlyDictionary<string, object?> Arguments, bool Done = false, string? Summary = null);

/// <summary>
///     Multi-step agent that can only call tools through <see cref="IToolGateway" />.
/// </summary>
public interface IAiAgent
{
    ValueTask<Result<AiAgentResult>> RunAsync(AiAgentRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
///     Bounded tool-calling agent. Never bypasses <see cref="IToolGateway" />, only auto-executes read tools,
///     enforces a wall-clock/step/tool-call budget, and aborts when the planner starts repeating itself.
/// </summary>
public sealed class ToolCallingAgent(
    IToolGateway gateway,
    IAgentPlanner planner,
    IAiAuditLogger auditLogger,
    NavyblueAiOptions options,
    IClock? clock = null) : IAiAgent
{
    private readonly IClock _clock = clock ?? new SystemClock();

    #region IAiAgent Members

    /// <inheritdoc />
    public async ValueTask<Result<AiAgentResult>> RunAsync(AiAgentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using Activity? activity = AiTelemetry.StartActivity("ai.agent.run");
        activity?.SetTag("ai.agent.goal", request.Goal);

        AiBudget budget = ResolveBudget(request, options);
        int maxSteps = budget.MaxSteps ?? options.MaxAgentSteps;
        if (maxSteps <= 0)
        {
            return Result.Failure<AiAgentResult>(new Error(AiErrorCodes.ConfigurationInvalid, "MaxAgentSteps must be positive."));
        }

        AiBudgetTracker tracker = new(budget with { MaxSteps = maxSteps }, this._clock);
        List<AiAgentStep> steps = [];
        HashSet<string> seenCalls = new(StringComparer.Ordinal);

        IReadOnlyList<ToolDefinition> tools = gateway
            .ListTools(request.TenantId)
            .Where(t => t.RiskLevel == ToolRiskLevel.Read)
            .ToArray();

        while (true)
        {
            Result withinBudget = tracker.EnsureWithinBudget();
            if (withinBudget.Failed)
            {
                await this.AuditAsync(AiAuditActions.AgentBudgetExceeded, request, false, withinBudget.Error.Message, cancellationToken).ConfigureAwait(false);
                return Result.Success(Stop(steps, tracker.Steps >= maxSteps ? AiAgentStopReasons.StepLimit : AiAgentStopReasons.BudgetExceeded));
            }

            Result<AgentPlan> planResult = await planner.PlanNextAsync(request, steps, tools, cancellationToken).ConfigureAwait(false);
            if (planResult.Failed)
            {
                await this.AuditAsync(AiAuditActions.AgentPlanFailed, request, false, planResult.Error.Message, cancellationToken).ConfigureAwait(false);
                AiTelemetry.MarkFailed(activity, planResult.Error.Code, planResult.Error.Message);
                return Result.Failure<AiAgentResult>(planResult.Error);
            }

            tracker.RecordStep();
            AgentPlan plan = planResult.Value!;

            if (plan.Done)
            {
                AiTelemetry.AgentSteps.Add(1, new KeyValuePair<string, object?>("outcome", "done"));
                return Result.Success(new AiAgentResult
                {
                    Completed = true,
                    Steps = steps,
                    FinalResult = plan.Summary ?? steps.LastOrDefault()?.Result,
                    StopReason = AiAgentStopReasons.PlannerDone
                });
            }

            if (!gateway.TryGetTool(plan.ToolName, out ToolDefinition? tool) || tool is null)
            {
                return Result.Failure<AiAgentResult>(new Error(AiErrorCodes.ToolNotFound, $"Planned tool '{plan.ToolName}' is not registered."));
            }

            if (tool.RiskLevel is ToolRiskLevel.Write or ToolRiskLevel.HighRisk)
            {
                // Complex write flows must go through orchestrator draft confirmation — never auto-execute here.
                AiTelemetry.AgentSteps.Add(1, new KeyValuePair<string, object?>("outcome", "write_blocked"));
                return Result.Success(new AiAgentResult
                {
                    Completed = false,
                    Steps = steps,
                    FinalResult = new { RequiresConfirmation = true, Tool = tool.Name, plan.Arguments },
                    StopReason = AiAgentStopReasons.WriteRequiresConfirmation
                });
            }

            if (options.DetectAgentLoops && !seenCalls.Add(Fingerprint(plan)))
            {
                AiTelemetry.AgentSteps.Add(1, new KeyValuePair<string, object?>("outcome", "loop"));
                return Result.Success(Stop(steps, AiAgentStopReasons.LoopDetected));
            }

            Result<object?> invoked = await gateway.InvokeAsync(
                    new ToolInvocation(plan.ToolName, plan.Arguments, request.UserId, request.TenantId, CorrelationContext.Current),
                    cancellationToken)
                .ConfigureAwait(false);

            tracker.RecordToolCall();
            steps.Add(new AiAgentStep(
                steps.Count + 1,
                plan.ToolName,
                plan.Arguments,
                invoked.Succeeded,
                invoked.Succeeded ? invoked.Value : null,
                invoked.Failed ? invoked.Error.Message : null));

            AiTelemetry.AgentSteps.Add(1, new KeyValuePair<string, object?>("outcome", invoked.Succeeded ? "ok" : "failed"));

            if (invoked.Failed)
            {
                return Result.Success(Stop(steps, AiAgentStopReasons.ToolFailed));
            }
        }
    }

    #endregion

    private static AiBudget ResolveBudget(AiAgentRequest request, NavyblueAiOptions options)
    {
        AiBudget budget = request.Budget ?? options.DefaultAgentBudget ?? new AiBudget();
        return request.MaxSteps is { } explicitSteps ? budget with { MaxSteps = explicitSteps } : budget;
    }

    private static AiAgentResult Stop(IReadOnlyList<AiAgentStep> steps, string reason) => new()
    {
        Completed = false,
        Steps = steps,
        FinalResult = steps.LastOrDefault()?.Result,
        StopReason = reason
    };

    private static string Fingerprint(AgentPlan plan)
    {
        IEnumerable<string> parts = plan.Arguments
            .OrderBy(p => p.Key, StringComparer.Ordinal)
            .Select(p => $"{p.Key}={Convert.ToString(p.Value, CultureInfo.InvariantCulture)}");

        return $"{plan.ToolName}({string.Join("&", parts)})";
    }

    private ValueTask<Result> AuditAsync(string action, AiAgentRequest request, bool succeeded, string? detail, CancellationToken cancellationToken)
        => auditLogger.WriteAsync(
            new AiAuditEntry(action, null, request.UserId, request.TenantId, CorrelationContext.Current, succeeded, detail, this._clock.UtcNow),
            cancellationToken);
}

/// <summary>
///     Planner that refuses to plan. Registered as the production default so an application that forgot to
///     supply a real planner fails closed instead of silently running a scripted test double.
/// </summary>
public sealed class UnavailableAgentPlanner : IAgentPlanner
{
    #region IAgentPlanner Members

    /// <inheritdoc />
    public ValueTask<Result<AgentPlan>> PlanNextAsync(AiAgentRequest request, IReadOnlyList<AiAgentStep> history, IReadOnlyList<ToolDefinition> availableTools, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Failure<AgentPlan>(new Error(
            AiErrorCodes.PlannerNotConfigured,
            "IAgentPlanner is not configured. Register a planner before using IAiAgent.")));

    #endregion
}

/// <summary>
///     Deterministic planner for tests and scripted multi-step flows.
/// </summary>
public sealed class ScriptedAgentPlanner : IAgentPlanner
{
    private readonly Queue<AgentPlan> _plans = new();

    #region IAgentPlanner Members

    /// <inheritdoc />
    public ValueTask<Result<AgentPlan>> PlanNextAsync(AiAgentRequest request, IReadOnlyList<AiAgentStep> history, IReadOnlyList<ToolDefinition> availableTools, CancellationToken cancellationToken = default)
    {
        if (this._plans.Count == 0)
        {
            return ValueTask.FromResult(Result.Success(new AgentPlan(string.Empty, new Dictionary<string, object?>(), true, "no more plans")));
        }

        return ValueTask.FromResult(Result.Success(this._plans.Dequeue()));
    }

    #endregion

    public ScriptedAgentPlanner Enqueue(string toolName, IReadOnlyDictionary<string, object?>? args = null)
    {
        this._plans.Enqueue(new AgentPlan(toolName, args ?? new Dictionary<string, object?>()));
        return this;
    }

    public ScriptedAgentPlanner EnqueueDone(string? summary = null)
    {
        this._plans.Enqueue(new AgentPlan(string.Empty, new Dictionary<string, object?>(), true, summary));
        return this;
    }
}