// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : 05_PermissionAndAudit.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="05_PermissionAndAudit.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Navyblue.Foundation.Primitives;
using Xunit;

namespace Navyblue.Foundation.AI.Samples;

/// <summary>
///     Scenario 05 — Permission guard + audit trail.
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class PermissionAndAudit_05
{
    [Fact]
    public async Task Missing_permission_blocks_tool_and_writes_audit()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        DemoPermissionChecker permissions = sp.GetRequiredService<DemoPermissionChecker>();
        IToolGateway gateway = sp.GetRequiredService<IToolGateway>();
        InMemoryAiAuditLogger audit = sp.GetRequiredService<InMemoryAiAuditLogger>();

        permissions.Revoke("sales.rank.read");

        Result<object?> result = await gateway.InvokeAsync(new ToolInvocation(
            DemoTools.GetSalesRank,
            new Dictionary<string, object?> { ["period"] = "month" },
            UserId: "u-guest"));

        Assert.True(result.Failed);
        Assert.Equal("ai.permission.denied", result.Error.Code);
        Assert.Contains(audit.Entries, e => e.Action == "tool.permission_denied" && !e.Succeeded);
    }

    [Fact]
    public async Task Successful_read_is_audited()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        InMemoryAiAuditLogger audit = sp.GetRequiredService<InMemoryAiAuditLogger>();

        await orchestrator.ProcessAsync(new UserUtterance("本月销售额排名", UserId: "u-001", TenantId: "t-001"));

        AiAuditEntry? executed = audit.Entries.LastOrDefault(e => e.Action == "tool.executed");
        Assert.NotNull(executed);
        Assert.True(executed!.Succeeded);
        Assert.Equal(DemoTools.GetSalesRank, executed.ToolName);
        Assert.Equal("u-001", executed.UserId);
        Assert.Equal("t-001", executed.TenantId);
    }
}