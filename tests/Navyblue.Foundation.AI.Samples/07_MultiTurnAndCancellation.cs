// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : 07_MultiTurnAndCancellation.cs
// Created          : 2026-07-30  16:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="07_MultiTurnAndCancellation.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Navyblue.Foundation.Primitives;
using Xunit;

namespace Navyblue.Foundation.AI.Samples;

/// <summary>
///     Scenario 07 — multi-turn clarification, draft cancellation, and the maker-checker rule.
/// </summary>
// ReSharper disable once InconsistentNaming
[Collection(DemoToolsCollection.Name)]
// ReSharper disable once InconsistentNaming
public sealed class MultiTurnAndCancellation_07
{
    [Fact]
    public async Task Ambiguous_question_is_completed_by_the_next_turn()
    {
        await using ServiceProvider sp = DemoAiHost.Create();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();
        IConversationStore conversations = sp.GetRequiredService<IConversationStore>();

        // Turn 1: no domain keyword at all — the framework must ask instead of guessing.
        Result<AiPipelineResult> turn1 = await orchestrator.ProcessAsync(
            new UserUtterance("帮我查一下", UserId: "u1", TenantId: "t-001", ConversationId: "conv-1"));

        Assert.Equal(AiPipelineStatus.Clarify, turn1.Value!.Status);
        Assert.NotNull((await conversations.GetAsync("conv-1"))!.PendingClarification);

        // Turn 2: the answer is merged with the original question, so the intent can now be resolved.
        Result<AiPipelineResult> turn2 = await orchestrator.ProcessAsync(
            new UserUtterance("员工工号 1001", UserId: "u1", TenantId: "t-001", ConversationId: "conv-1"));

        Assert.Equal(AiPipelineStatus.Executed, turn2.Value!.Status);
        Assert.Equal("conv-1", turn2.Value.ConversationId);
        Assert.Null((await conversations.GetAsync("conv-1"))!.PendingClarification);
    }

    [Fact]
    public async Task User_can_cancel_a_pending_write_draft()
    {
        const string name = "王五取消";
        await using ServiceProvider sp = DemoAiHost.Create();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();

        Result<AiPipelineResult> draft = await orchestrator.ProcessAsync(
            new UserUtterance($"新增员工{name}", UserId: "u1", TenantId: "t-001", ConversationId: "conv-2"));
        Assert.Equal(AiPipelineStatus.AwaitingConfirmation, draft.Value!.Status);

        Result<AiPipelineResult> cancelled = await orchestrator.CancelDraftAsync(draft.Value.DraftId!, "u1", "t-001");
        Assert.Equal(AiPipelineStatus.Cancelled, cancelled.Value!.Status);

        Result<AiPipelineResult> tooLate = await orchestrator.ConfirmDraftAsync(
            new DraftConfirmationRequest(draft.Value.DraftId!, true, "u1", "t-001"));

        Assert.True(tooLate.Failed);
        Assert.Equal(0, CountCreated(name));
    }

    [Fact]
    public async Task Concurrent_confirmations_never_double_write()
    {
        const string name = "王五并发";
        await using ServiceProvider sp = DemoAiHost.Create();
        IAiOrchestrator orchestrator = sp.GetRequiredService<IAiOrchestrator>();

        Result<AiPipelineResult> draft = await orchestrator.ProcessAsync(
            new UserUtterance($"新增员工{name}", UserId: "u1", TenantId: "t-001"));
        string draftId = draft.Value!.DraftId!;

        await Task.WhenAll(Enumerable.Range(0, 12).Select(_ =>
            orchestrator.ConfirmDraftAsync(new DraftConfirmationRequest(draftId, true, "u1", "t-001")).AsTask()));

        Assert.Equal(1, CountCreated(name));
    }

    private static int CountCreated(string name)
        => DemoTools.CreatedEmployees
            .OfType<IReadOnlyDictionary<string, object?>>()
            .Count(e => e.TryGetValue("name", out object? value) && string.Equals(value?.ToString(), name, StringComparison.Ordinal));
}