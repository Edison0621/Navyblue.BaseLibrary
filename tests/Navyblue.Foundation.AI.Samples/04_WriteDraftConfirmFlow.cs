// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : 04_WriteDraftConfirmFlow.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="04_WriteDraftConfirmFlow.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Navyblue.Foundation.Primitives;
using Xunit;

namespace Navyblue.Foundation.AI.Samples;

/// <summary>
///     Scenario 04 — Write operations require draft → user confirm → execute.
/// </summary>
// ReSharper disable once InconsistentNaming
[Collection(DemoToolsCollection.Name)]
// ReSharper disable once InconsistentNaming
public sealed class WriteDraftConfirmFlow_04
{
    public WriteDraftConfirmFlow_04() => DemoTools.Reset();

    [Fact]
    public async Task Create_employee_waits_for_confirmation()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        IToolGateway gateway = sp.GetRequiredService<IToolGateway>();

        Result<AiPipelineResult> first = await orchestrator.ProcessAsync(
            new UserUtterance("新增员工李四", UserId: "hr-admin"));

        Assert.True(first.Succeeded);
        Assert.Equal(AiPipelineStatus.AwaitingConfirmation, first.Value!.Status);
        Assert.NotNull(first.Value.DraftId);
        Assert.Empty(DemoTools.CreatedEmployees); // not executed yet

        // Direct invoke of write tool is blocked without confirmation
        Result<object?> blocked = await gateway.InvokeResolvedAsync(first.Value.Operation!);
        Assert.True(blocked.Failed);
        Assert.Equal("ai.tool.requires_confirmation", blocked.Error.Code);

        // Owner confirms the draft (ownership is enforced)
        Result<AiPipelineResult> confirmed = await orchestrator.ConfirmDraftAsync(
            new DraftConfirmationRequest(first.Value.DraftId!, Confirmed: true, UserId: "hr-admin"));
        Assert.True(confirmed.Succeeded);
        Assert.Equal(AiPipelineStatus.Executed, confirmed.Value!.Status);
        Assert.Single(DemoTools.CreatedEmployees);
    }

    [Fact]
    public async Task User_can_reject_the_draft()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();

        Result<AiPipelineResult> first = await orchestrator.ProcessAsync(
            new UserUtterance("新增员工王五", UserId: "hr-admin"));
        Assert.Equal(AiPipelineStatus.AwaitingConfirmation, first.Value!.Status);

        Result<AiPipelineResult> rejected = await orchestrator.ConfirmDraftAsync(
            new DraftConfirmationRequest(first.Value.DraftId!, Confirmed: false, UserId: "hr-admin"));
        Assert.True(rejected.Succeeded);
        Assert.Equal(AiPipelineStatus.Failed, rejected.Value!.Status);
        Assert.Empty(DemoTools.CreatedEmployees);
    }
}