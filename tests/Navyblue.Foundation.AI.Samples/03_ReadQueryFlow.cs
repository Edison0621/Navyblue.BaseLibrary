// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : 03_ReadQueryFlow.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="03_ReadQueryFlow.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Navyblue.Foundation.Primitives;
using Xunit;

namespace Navyblue.Foundation.AI.Samples;

/// <summary>
///     Scenario 03 — Read query executes immediately (no draft).
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class ReadQueryFlow_03
{
    [Fact]
    public async Task Sales_rank_query_runs_end_to_end()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        InMemoryAiAuditLogger audit = sp.GetRequiredService<InMemoryAiAuditLogger>();

        Result<AiPipelineResult> result = await orchestrator.ProcessAsync(
            new UserUtterance("本月销售额排名", UserId: "u-001", TenantId: "t-001"));

        Assert.True(result.Succeeded);
        AiPipelineResult pipeline = result.Value!;

        Assert.Equal(AiPipelineStatus.Executed, pipeline.Status);
        Assert.Null(pipeline.DraftId);
        Assert.Equal(DemoDomains.Sales, pipeline.Classification!.Domain);
        Assert.Equal("GetSalesRank", pipeline.Candidate!.IntentName);
        Assert.NotNull(pipeline.ExecutionResult);

        Assert.Contains(audit.Entries, e => e.Action == "ai.classify" && e.Succeeded);
        Assert.Contains(audit.Entries, e => e.Action == "tool.executed" && e.ToolName == DemoTools.GetSalesRank);
    }

    [Fact]
    public async Task Inventory_on_hand_query()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();

        Result<AiPipelineResult> result = await orchestrator.ProcessAsync(UserUtterance.FromText("查询仓库现存量"));

        Assert.True(result.Succeeded);
        Assert.Equal(AiPipelineStatus.Executed, result.Value!.Status);
        Assert.Equal(DemoDomains.Inventory, result.Value.Classification!.Domain);
    }

    [Fact]
    public async Task Hr_employee_lookup_is_hr_not_sales()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();

        Result<AiPipelineResult> result = await orchestrator.ProcessAsync(UserUtterance.FromText("销售部员工工号1001"));

        Assert.True(result.Succeeded);
        Assert.Equal(AiPipelineStatus.Executed, result.Value!.Status);
        Assert.Equal(DemoDomains.Hr, result.Value.Classification!.Domain);
        Assert.Equal("GetEmployee", result.Value.Candidate!.IntentName);
    }
}