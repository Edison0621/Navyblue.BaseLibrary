// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AiOrchestratorTests.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="AiOrchestratorTests.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Navyblue.Foundation.Primitives;
using Xunit;

namespace Navyblue.Foundation.AI.Tests;

public sealed class AiOrchestratorTests
{
    private static ServiceProvider CreateHost(
        FixedIntentRecognizer recognizer,
        ISlotResolver resolver,
        Action<IToolRegistry>? registerTools = null,
        Action<NavyblueAiOptions>? configure = null)
    {
        ServiceCollection services = new();
        services.AddNavyblueAi(o =>
        {
            o.UseDevelopmentDefaults();
            o.MinDomainConfidence = 0.5;
            o.DomainRules.Add(new DomainKeywordRule("hr", "员工", "工号", "人事"));
            o.DomainRules.Add(new DomainKeywordRule("sales", "销售额", "毛利", "销售排名"));
            o.KnowledgeDomains.Add("knowledge");
            configure?.Invoke(o);
        });
        services.RemoveAll<IIntentRecognizer>();
        services.RemoveAll<ISlotResolver>();
        services.AddSingleton<IIntentRecognizer>(recognizer);
        services.AddSingleton(resolver);

        ServiceProvider sp = services.BuildServiceProvider();
        registerTools?.Invoke(sp.GetRequiredService<IToolRegistry>());
        return sp;
    }

    [Fact]
    public async Task Write_operation_creates_draft_and_requires_owner_confirm()
    {
        int executions = 0;
        FixedIntentRecognizer recognizer = new FixedIntentRecognizer().ForDomain(
            "hr",
            new IntentCandidate("CreateEmployee", "hr", 0.95, [new SlotValue("name", "张三")]));

        await using ServiceProvider sp = CreateHost(
            recognizer,
            new FixedSlotResolver((_, _) => new ResolvedOperation(
                "CreateEmployee",
                "hr",
                ToolRiskLevel.Write,
                new Dictionary<string, object?> { ["name"] = "张三" },
                ToolName: "CreateEmployee")),
            registry => registry.Register(new ToolDefinition(
                "CreateEmployee",
                "create",
                ToolRiskLevel.Write,
                ExecuteAsync: (_, _) =>
                {
                    executions++;
                    return ValueTask.FromResult<object?>("ok");
                })));

        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        Result<AiPipelineResult> first = await orchestrator.ProcessAsync(new UserUtterance("新增员工张三", UserId: "u1", TenantId: "t1"));
        Assert.Equal(AiPipelineStatus.AwaitingConfirmation, first.Value!.Status);

        Result<AiPipelineResult> wrongOwner = await orchestrator.ConfirmDraftAsync(new DraftConfirmationRequest(first.Value.DraftId!, true, UserId: "other", TenantId: "t1"));
        Assert.True(wrongOwner.Failed);
        Assert.Equal(AiErrorCodes.DraftOwnerMismatch, wrongOwner.Error.Code);
        Assert.Equal(0, executions);

        Result<AiPipelineResult> confirmed = await orchestrator.ConfirmDraftAsync(new DraftConfirmationRequest(first.Value.DraftId!, true, UserId: "u1", TenantId: "t1"));
        Assert.Equal(AiPipelineStatus.Executed, confirmed.Value!.Status);
        Assert.Equal(1, executions);
    }

    [Fact]
    public async Task Concurrent_confirmations_execute_the_write_exactly_once()
    {
        int executions = 0;
        FixedIntentRecognizer recognizer = new FixedIntentRecognizer().ForDomain(
            "hr",
            new IntentCandidate("CreateEmployee", "hr", 0.95, [new SlotValue("name", "张三")]));

        await using ServiceProvider sp = CreateHost(
            recognizer,
            new FixedSlotResolver((_, _) => new ResolvedOperation(
                "CreateEmployee",
                "hr",
                ToolRiskLevel.Write,
                new Dictionary<string, object?> { ["name"] = "张三" },
                ToolName: "CreateEmployee")),
            registry => registry.Register(new ToolDefinition(
                "CreateEmployee",
                "create",
                ToolRiskLevel.Write,
                ExecuteAsync: async (_, _) =>
                {
                    Interlocked.Increment(ref executions);
                    // ReSharper disable once MethodSupportsCancellation
                    await Task.Delay(20);
                    return "ok";
                })));

        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        Result<AiPipelineResult> draft = await orchestrator.ProcessAsync(new UserUtterance("新增员工张三", UserId: "u1", TenantId: "t1"));
        string draftId = draft.Value!.DraftId!;

        Result<AiPipelineResult>[] outcomes = await Task.WhenAll(Enumerable.Range(0, 16).Select(_ =>
            orchestrator.ConfirmDraftAsync(new DraftConfirmationRequest(draftId, true, UserId: "u1", TenantId: "t1")).AsTask()));

        Assert.Equal(1, executions);
        Assert.Equal(1, outcomes.Count(o => o.Succeeded && o.Value!.Status == AiPipelineStatus.Executed));
        Assert.All(
            outcomes.Where(o => o.Failed),
            o => Assert.Contains(o.Error.Code, new[] { AiErrorCodes.DraftAlreadyExecuting, AiErrorCodes.DraftInvalidState }));
    }

    [Fact]
    public async Task Double_confirmation_requires_a_second_approver()
    {
        FixedIntentRecognizer recognizer = new FixedIntentRecognizer().ForDomain(
            "hr",
            new IntentCandidate("DeleteEmployee", "hr", 0.95, [new SlotValue("id", "1")]));

        await using ServiceProvider sp = CreateHost(
            recognizer,
            new FixedSlotResolver((_, _) => new ResolvedOperation(
                "DeleteEmployee",
                "hr",
                ToolRiskLevel.HighRisk,
                new Dictionary<string, object?> { ["id"] = "1" },
                ToolName: "DeleteEmployee")),
            registry => registry.Register(new ToolDefinition(
                "DeleteEmployee",
                "delete",
                ToolRiskLevel.HighRisk,
                ExecuteAsync: (_, _) => ValueTask.FromResult<object?>("ok"))),
            o =>
            {
                o.RequireDoubleConfirmForHighRisk = true;
                o.RequireDistinctApprovers = true;
            });

        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        Result<AiPipelineResult> draft = await orchestrator.ProcessAsync(new UserUtterance("删除员工", UserId: "maker", TenantId: "t1"));
        string draftId = draft.Value!.DraftId!;

        Result<AiPipelineResult> firstApproval = await orchestrator.ConfirmDraftAsync(
            new DraftConfirmationRequest(draftId, true, UserId: "maker", TenantId: "t1", HighRiskAcknowledged: true));
        Assert.Equal(AiPipelineStatus.AwaitingConfirmation, firstApproval.Value!.Status);

        Result<AiPipelineResult> sameApprover = await orchestrator.ConfirmDraftAsync(
            new DraftConfirmationRequest(draftId, true, UserId: "maker", TenantId: "t1", HighRiskAcknowledged: true));
        Assert.True(sameApprover.Failed);
        Assert.Equal(AiErrorCodes.DraftDistinctApproverRequired, sameApprover.Error.Code);

        Result<AiPipelineResult> checker = await orchestrator.ConfirmDraftAsync(
            new DraftConfirmationRequest(draftId, true, UserId: "checker", TenantId: "t1", HighRiskAcknowledged: true));
        Assert.Equal(AiPipelineStatus.Executed, checker.Value!.Status);
    }

    [Fact]
    public async Task Draft_can_be_cancelled_by_its_owner_only()
    {
        FixedIntentRecognizer recognizer = new FixedIntentRecognizer().ForDomain(
            "hr",
            new IntentCandidate("CreateEmployee", "hr", 0.95, []));

        await using ServiceProvider sp = CreateHost(
            recognizer,
            new FixedSlotResolver((_, _) => new ResolvedOperation(
                "CreateEmployee",
                "hr",
                ToolRiskLevel.Write,
                new Dictionary<string, object?>(),
                ToolName: "CreateEmployee")),
            registry => registry.Register(new ToolDefinition("CreateEmployee", "create", ToolRiskLevel.Write, ExecuteAsync: (_, _) => ValueTask.FromResult<object?>("ok"))));

        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        Result<AiPipelineResult> draft = await orchestrator.ProcessAsync(new UserUtterance("新增员工", UserId: "u1", TenantId: "t1"));
        string draftId = draft.Value!.DraftId!;

        Result<AiPipelineResult> stranger = await orchestrator.CancelDraftAsync(draftId, "other", "t1");
        Assert.True(stranger.Failed);

        Result<AiPipelineResult> cancelled = await orchestrator.CancelDraftAsync(draftId, "u1", "t1");
        Assert.Equal(AiPipelineStatus.Cancelled, cancelled.Value!.Status);

        Result<AiPipelineResult> tooLate = await orchestrator.ConfirmDraftAsync(new DraftConfirmationRequest(draftId, true, UserId: "u1", TenantId: "t1"));
        Assert.True(tooLate.Failed);
        Assert.Equal(AiErrorCodes.DraftInvalidState, tooLate.Error.Code);
    }

    [Fact]
    public async Task Missing_tool_fails_closed()
    {
        FixedIntentRecognizer recognizer = new FixedIntentRecognizer().ForDomain(
            "sales",
            new IntentCandidate("GetSalesRank", "sales", 0.9, []));

        await using ServiceProvider sp = CreateHost(
            recognizer,
            new FixedSlotResolver((_, _) => new ResolvedOperation("GetSalesRank", "sales", ToolRiskLevel.Read, new Dictionary<string, object?>(), ToolName: "GetSalesRank")));

        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        Result<AiPipelineResult> result = await orchestrator.ProcessAsync(new UserUtterance("本月销售额排名"));
        Assert.True(result.Failed);
        Assert.Equal(AiErrorCodes.ToolNotFound, result.Error.Code);
    }

    [Fact]
    public async Task Knowledge_domain_uses_retriever()
    {
        ServiceCollection services = new();
        services.AddNavyblueAi(o =>
        {
            o.UseDevelopmentDefaults();
            o.EnableKnowledgePermissionFiltering = false;
            o.DomainRules.Add(new DomainKeywordRule("knowledge", "手册", "制度"));
            o.KnowledgeDomains.Add("knowledge");
        });
        services.RemoveAll<IKnowledgeRetriever>();
        InMemoryKnowledgeRetriever knowledge = new([new KnowledgeHit("1", "Leave policy", "请假制度说明", 1)]);
        services.AddSingleton<IKnowledgeRetriever>(knowledge);
        services.AddSingleton<IIntentRecognizer, UnavailableIntentRecognizer>();
        services.AddSingleton<ISlotResolver, UnavailableSlotResolver>();

        await using ServiceProvider sp = services.BuildServiceProvider();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        Result<AiPipelineResult> result = await orchestrator.ProcessAsync(UserUtterance.FromText("如何操作请假制度"));
        Assert.True(result.Succeeded);
        Assert.Equal(AiPipelineStatus.Executed, result.Value!.Status);
        Assert.IsAssignableFrom<IReadOnlyList<KnowledgeHit>>(result.Value.ExecutionResult);
    }

    [Fact]
    public async Task Clarify_when_no_domain_rules()
    {
        ServiceCollection services = new();
        services.AddNavyblueAi(o =>
        {
            o.UseDevelopmentDefaults();
            o.RegisterFakes = true;
            // DomainRules left empty — framework must Clarify, never invent a business domain
        });
        await using ServiceProvider sp = services.BuildServiceProvider();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        Result<AiPipelineResult> result = await orchestrator.ProcessAsync(new UserUtterance("随便聊聊"));
        Assert.Equal(AiPipelineStatus.Clarify, result.Value!.Status);
        Assert.True(AiWellKnownDomains.RequiresClarification(result.Value.Classification?.Domain));
    }

    [Fact]
    public async Task Production_default_permission_guard_is_fail_closed()
    {
        ServiceCollection services = new();
        services.AddNavyblueAi(); // UsePermissiveDefaults = false
        await using ServiceProvider sp = services.BuildServiceProvider();
        IPermissionGuard guard = sp.GetRequiredService<IPermissionGuard>();
        Result denied = await guard.EnsureGrantedAsync("sales.rank.read");
        Assert.True(denied.Failed);
        Assert.Equal(AiErrorCodes.PermissionGuardNotConfigured, denied.Error.Code);
    }

    [Fact]
    public async Task Production_defaults_refuse_in_memory_persistence()
    {
        ServiceCollection services = new();
        services.AddNavyblueAi();
        await using ServiceProvider sp = services.BuildServiceProvider();

        Assert.IsType<UnconfiguredDraftStore>(sp.GetRequiredService<IDraftStore>());
        Assert.IsType<UnconfiguredAiAuditLogger>(sp.GetRequiredService<IAiAuditLogger>());
        Assert.IsType<UnavailableAgentPlanner>(sp.GetRequiredService<IAgentPlanner>());
    }

    [Fact]
    public async Task Agent_runs_read_tools_via_gateway_only()
    {
        ServiceCollection services = new();
        ScriptedAgentPlanner planner = new ScriptedAgentPlanner()
            .Enqueue("echo", new Dictionary<string, object?> { ["v"] = 1 })
            .EnqueueDone("done");
        services.AddNavyblueAi(o => o.UseDevelopmentDefaults());
        services.RemoveAll<IAgentPlanner>();
        services.AddSingleton<IAgentPlanner>(planner);

        await using ServiceProvider sp = services.BuildServiceProvider();
        sp.GetRequiredService<IToolRegistry>()
            .Register(new ToolDefinition("echo", "echo", ToolRiskLevel.Read, ExecuteAsync: (a, _) => ValueTask.FromResult<object?>(a["v"])));

        IAiAgent agent = sp.GetRequiredService<IAiAgent>();
        Result<AiAgentResult> result = await agent.RunAsync(new AiAgentRequest("echo once"));
        Assert.True(result.Succeeded);
        Assert.True(result.Value!.Completed);
        Assert.Single(result.Value.Steps);
    }

    [Fact]
    public async Task Agent_aborts_when_planner_repeats_itself()
    {
        ServiceCollection services = new();
        ScriptedAgentPlanner planner = new();
        for (int i = 0; i < 5; i++)
        {
            planner.Enqueue("echo", new Dictionary<string, object?> { ["v"] = 1 });
        }

        services.AddNavyblueAi(o => o.UseDevelopmentDefaults());
        services.RemoveAll<IAgentPlanner>();
        services.AddSingleton<IAgentPlanner>(planner);

        await using ServiceProvider sp = services.BuildServiceProvider();
        sp.GetRequiredService<IToolRegistry>()
            .Register(new ToolDefinition("echo", "echo", ToolRiskLevel.Read, ExecuteAsync: (a, _) => ValueTask.FromResult<object?>(a["v"])));

        IAiAgent agent = sp.GetRequiredService<IAiAgent>();
        Result<AiAgentResult> result = await agent.RunAsync(new AiAgentRequest("loop"));

        Assert.True(result.Succeeded);
        Assert.False(result.Value!.Completed);
        Assert.Equal(AiAgentStopReasons.LoopDetected, result.Value.StopReason);
        Assert.Single(result.Value.Steps);
    }

    [Fact]
    public async Task Agent_planner_is_fail_closed_by_default()
    {
        ServiceCollection services = new();
        services.AddNavyblueAi(o => o.UseDevelopmentDefaults());
        await using ServiceProvider sp = services.BuildServiceProvider();

        IAiAgent agent = sp.GetRequiredService<IAiAgent>();
        Result<AiAgentResult> result = await agent.RunAsync(new AiAgentRequest("anything"));

        Assert.True(result.Failed);
        Assert.Equal(AiErrorCodes.PlannerNotConfigured, result.Error.Code);
    }

    [Fact]
    public async Task HighRisk_requires_acknowledgement_before_draft_mutation()
    {
        int executions = 0;
        FixedIntentRecognizer recognizer = new FixedIntentRecognizer().ForDomain(
            "hr",
            new IntentCandidate("DeleteEmployee", "hr", 0.95, [new SlotValue("id", "1")]));

        await using ServiceProvider sp = CreateHost(
            recognizer,
            new FixedSlotResolver((_, _) => new ResolvedOperation(
                "DeleteEmployee",
                "hr",
                ToolRiskLevel.HighRisk,
                new Dictionary<string, object?> { ["id"] = "1" },
                ToolName: "DeleteEmployee")),
            registry => registry.Register(new ToolDefinition(
                "DeleteEmployee",
                "delete",
                ToolRiskLevel.HighRisk,
                ExecuteAsync: (_, _) =>
                {
                    executions++;
                    return ValueTask.FromResult<object?>("ok");
                })),
            o => o.RequireHighRiskAcknowledgement = true);

        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        IDraftStore drafts = sp.GetRequiredService<IDraftStore>();

        Result<AiPipelineResult> first = await orchestrator.ProcessAsync(new UserUtterance("删除员工", UserId: "u1"));
        Assert.Equal(AiPipelineStatus.AwaitingConfirmation, first.Value!.Status);

        Result<AiPipelineResult> withoutAck = await orchestrator.ConfirmDraftAsync(
            new DraftConfirmationRequest(first.Value.DraftId!, true, UserId: "u1", HighRiskAcknowledged: false));
        Assert.True(withoutAck.Failed);
        Assert.Equal(AiErrorCodes.DraftHighRiskAckRequired, withoutAck.Error.Code);
        Assert.Equal(DraftStatus.Pending, (await drafts.GetAsync(first.Value.DraftId!))!.Status);
        Assert.Equal(0, executions);

        Result<AiPipelineResult> withAck = await orchestrator.ConfirmDraftAsync(
            new DraftConfirmationRequest(first.Value.DraftId!, true, UserId: "u1", HighRiskAcknowledged: true));
        Assert.Equal(AiPipelineStatus.Executed, withAck.Value!.Status);
        Assert.Equal(1, executions);
    }

    [Fact]
    public async Task Follow_up_turn_completes_a_previously_ambiguous_request()
    {
        FixedIntentRecognizer recognizer = new();
        recognizer.ForDomain("hr", new IntentCandidate("GetEmployee", "hr", 0.95, [new SlotValue("employeeNo", "1001")]));

        int calls = 0;
        await using ServiceProvider sp = CreateHost(
            recognizer,
            new FixedSlotResolver((candidate, utterance) =>
            {
                calls++;
                // First turn lacks the employee number; the follow-up turn supplies it.
                if (!utterance.Text.Contains("1001", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("unreachable");
                }

                return new ResolvedOperation(
                    "GetEmployee",
                    "hr",
                    ToolRiskLevel.Read,
                    new Dictionary<string, object?> { ["employeeNo"] = "1001" },
                    ToolName: "GetEmployee");
            }),
            registry => registry.Register(new ToolDefinition(
                "GetEmployee",
                "read",
                ToolRiskLevel.Read,
                ExecuteAsync: (a, _) => ValueTask.FromResult<object?>($"employee {a["employeeNo"]}"))));

        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        IConversationStore conversations = sp.GetRequiredService<IConversationStore>();

        Result<AiPipelineResult> ambiguous = await orchestrator.ProcessAsync(
            new UserUtterance("查一下这个人", UserId: "u1", TenantId: "t1", ConversationId: "c1"));
        Assert.Equal(AiPipelineStatus.Clarify, ambiguous.Value!.Status);

        AiConversation? conversation = await conversations.GetAsync("c1");
        Assert.NotNull(conversation!.PendingClarification);

        Result<AiPipelineResult> followUp = await orchestrator.ProcessAsync(
            new UserUtterance("员工工号 1001", UserId: "u1", TenantId: "t1", ConversationId: "c1"));

        Assert.Equal(AiPipelineStatus.Executed, followUp.Value!.Status);
        Assert.Equal("employee 1001", followUp.Value.ExecutionResult);
        Assert.Equal("c1", followUp.Value.ConversationId);
        Assert.Equal(1, calls);

        conversation = await conversations.GetAsync("c1");
        Assert.Null(conversation!.PendingClarification);
    }

    [Fact]
    public async Task Quota_guard_throttles_a_noisy_caller()
    {
        ServiceCollection services = new();
        services.AddNavyblueAi(o =>
        {
            o.UseDevelopmentDefaults();
            o.RegisterFakes = true;
            o.QuotaPermitLimit = 2;
            o.QuotaWindow = TimeSpan.FromMinutes(5);
        });

        await using ServiceProvider sp = services.BuildServiceProvider();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();

        for (int i = 0; i < 2; i++)
        {
            Assert.True((await orchestrator.ProcessAsync(new UserUtterance("hello", UserId: "u1"))).Succeeded);
        }

        Result<AiPipelineResult> throttled = await orchestrator.ProcessAsync(new UserUtterance("hello", UserId: "u1"));
        Assert.True(throttled.Failed);
        Assert.Equal(AiErrorCodes.QuotaExceeded, throttled.Error.Code);
    }

    [Fact]
    public async Task Content_safety_filter_can_block_input()
    {
        ServiceCollection services = new();
        services.AddNavyblueAi(o =>
        {
            o.UseDevelopmentDefaults();
            o.RegisterFakes = true;
        });
        services.RemoveAll<IContentSafetyFilter>();
        services.AddSingleton<IContentSafetyFilter>(new RegexContentSafetyFilter(["drop\\s+table"]));

        await using ServiceProvider sp = services.BuildServiceProvider();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();

        Result<AiPipelineResult> blocked = await orchestrator.ProcessAsync(new UserUtterance("please DROP TABLE employees"));
        Assert.True(blocked.Failed);
        Assert.Equal(AiErrorCodes.ContentBlocked, blocked.Error.Code);
    }
}