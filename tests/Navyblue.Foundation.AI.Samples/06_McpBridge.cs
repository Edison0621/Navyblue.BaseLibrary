// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : 06_McpBridge.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="06_McpBridge.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Navyblue.Foundation.Primitives;
using Xunit;

namespace Navyblue.Foundation.AI.Samples;

/// <summary>
///     Scenario 06 — MCP bridge always forwards to ToolGateway (never DB).
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class McpBridge_06
{
    [Fact]
    public async Task Mcp_invoke_goes_through_gateway_with_permission_and_audit()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IMcpToolBridge mcp = sp.GetRequiredService<IMcpToolBridge>();
        InMemoryAiAuditLogger audit = sp.GetRequiredService<InMemoryAiAuditLogger>();

        Result<object?> result = await mcp.InvokeToolAsync(
            DemoTools.GetStock,
            new Dictionary<string, object?> { ["sku"] = "SKU-001" },
            userId: "mcp-user",
            tenantId: "t-001");

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Contains(audit.Entries, e => e.Action == "tool.executed" && e.ToolName == DemoTools.GetStock);
    }

    [Fact]
    public async Task Mcp_cannot_bypass_write_confirmation()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IMcpToolBridge mcp = sp.GetRequiredService<IMcpToolBridge>();

        Result<object?> result = await mcp.InvokeToolAsync(
            DemoTools.CreateEmployee,
            new Dictionary<string, object?> { ["name"] = "赵六" },
            userId: "mcp-user");

        Assert.True(result.Failed);
        Assert.Equal("ai.tool.requires_confirmation", result.Error.Code);
    }

    [Fact]
    public async Task Clarify_short_circuits_before_any_tool()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        InMemoryAiAuditLogger audit = sp.GetRequiredService<InMemoryAiAuditLogger>();

        Result<AiPipelineResult> result = await orchestrator.ProcessAsync(UserUtterance.FromText("随便聊聊"));

        Assert.True(result.Succeeded);
        Assert.Equal(AiPipelineStatus.Clarify, result.Value!.Status);
        Assert.NotNull(result.Value.Clarification);
        Assert.DoesNotContain(audit.Entries, e => e.Action == "tool.executed");
    }
}