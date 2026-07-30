// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : 08_ResilienceObservabilityAndMcp.cs
// Created          : 2026-07-30  16:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="08_ResilienceObservabilityAndMcp.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Navyblue.Foundation.Primitives;
using Xunit;

namespace Navyblue.Foundation.AI.Samples;

/// <summary>
///     Scenario 08 — argument validation, tracing, and MCP schema publication.
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class ResilienceObservabilityAndMcp_08
{
    [Fact]
    public async Task Undeclared_arguments_never_reach_the_application_service()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IMcpToolBridge mcp = sp.GetRequiredService<IMcpToolBridge>();

        Result<object?> injected = await mcp.InvokeToolAsync(
            DemoTools.GetStock,
            new Dictionary<string, object?> { ["sku"] = "SKU-001", ["__rawSql"] = "select * from salary" },
            userId: "mcp-user",
            tenantId: "t-001");

        Assert.True(injected.Failed);
        Assert.Equal(AiErrorCodes.ToolArgumentInvalid, injected.Error.Code);
    }

    [Fact]
    public async Task Values_outside_the_declared_domain_are_rejected()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IMcpToolBridge mcp = sp.GetRequiredService<IMcpToolBridge>();

        Result<object?> badPeriod = await mcp.InvokeToolAsync(
            DemoTools.GetSalesRank,
            new Dictionary<string, object?> { ["period"] = "century" },
            userId: "mcp-user",
            tenantId: "t-001");

        Assert.True(badPeriod.Failed);
        Assert.Equal(AiErrorCodes.ToolArgumentInvalid, badPeriod.Error.Code);
    }

    [Fact]
    public async Task Pipeline_emits_traces_for_operations_teams()
    {
        List<string> activities = [];
        using ActivityListener listener = new()
        {
            ShouldListenTo = source => source.Name == AiTelemetry.ActivitySourceName,
            Sample = (ref _) => ActivitySamplingResult.AllData,
            ActivityStopped = activity => activities.Add(activity.OperationName)
        };
        ActivitySource.AddActivityListener(listener);

        await using ServiceProvider sp = DemoAiHost.Create();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();

        await orchestrator.ProcessAsync(new UserUtterance("查一下员工工号 1001", UserId: "u1", TenantId: "t-001"));

        Assert.Contains("ai.pipeline.process", activities);
        Assert.Contains("ai.tool.invoke", activities);
    }

    [Fact]
    public async Task Tool_catalog_is_published_to_mcp_with_generated_schemas()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IMcpToolBridge mcp = sp.GetRequiredService<IMcpToolBridge>();

        IReadOnlyList<McpToolDescriptor> descriptors = McpSchemaGenerator.Describe(mcp.ListTools("t-001"));
        McpToolDescriptor stock = descriptors.Single(d => d.Name == DemoTools.GetStock);

        Assert.Contains("\"required\":[\"sku\"]", stock.InputSchemaJson, StringComparison.Ordinal);
        Assert.Equal(nameof(ToolRiskLevel.Read), stock.RiskLevel);
    }

    [Fact]
    public async Task Negative_samples_stay_green_as_the_rules_evolve()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IntentEvaluationHarness harness = new(sp.GetRequiredService<IDomainClassifier>());

        IntentEvaluationReport report = await harness.RunAsync(
        [
            new IntentEvaluationCase("销售部有多少员工", DemoDomains.Hr),
            new IntentEvaluationCase("本月销售额排名", DemoDomains.Sales),
            new IntentEvaluationCase("仓库人员名单", DemoDomains.Hr),
            new IntentEvaluationCase("今天天气怎么样", ExpectClarify: true)
        ]);

        Assert.True(report.Failed == 0, report.Describe());
    }
}