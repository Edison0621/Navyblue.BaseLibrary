// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Conversation.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="Conversation.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Conversation roles used by <see cref="AiConversationTurn" />.
/// </summary>
public static class AiConversationRoles
{
    public const string User = "user";
    public const string Assistant = "assistant";
    public const string System = "system";
}

/// <summary>
///     A single turn in a multi-turn AI conversation.
/// </summary>
public sealed record AiConversationTurn(
    string Role,
    string Content,
    DateTimeOffset OccurredAt,
    string? Domain = null,
    string? IntentName = null,
    string? DraftId = null);

/// <summary>
///     Multi-turn conversation state. Carries the pending clarification and accumulated slots so a follow-up
///     utterance can complete an operation that was previously under-specified.
/// </summary>
public sealed class AiConversation(string conversationId, string? userId = null, string? tenantId = null)
{
    private readonly object _sync = new();
    private readonly List<AiConversationTurn> _turns = [];

    public string ConversationId { get; } = conversationId;
    public string? UserId { get; } = userId;
    public string? TenantId { get; } = tenantId;

    /// <summary>Draft awaiting confirmation, if any.</summary>
    public string? PendingDraftId { get; set; }

    /// <summary>Clarification the assistant last asked for; the next user turn is treated as its answer.</summary>
    public ClarificationRequest? PendingClarification { get; set; }

    /// <summary>Text of the turn that triggered <see cref="PendingClarification" />, used to rebuild full intent.</summary>
    public string? PendingUtterance { get; set; }

    /// <summary>Domain resolved on the previous successful turn; used as a hint for follow-ups.</summary>
    public string? LastDomain { get; set; }

    /// <summary>Slots accumulated across turns and merged into the next utterance context.</summary>
    public IDictionary<string, string> Slots { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<AiConversationTurn> Turns
    {
        get
        {
            lock (this._sync)
            {
                return this._turns.ToArray();
            }
        }
    }

    /// <summary>
    ///     Appends a turn, trimming the oldest entries beyond <paramref name="maxTurns" /> so long-running
    ///     conversations cannot grow without bound.
    /// </summary>
    public void AddTurn(AiConversationTurn turn, int maxTurns = 100)
    {
        ArgumentNullException.ThrowIfNull(turn);
        lock (this._sync)
        {
            this._turns.Add(turn);
            int excess = this._turns.Count - Math.Max(1, maxTurns);
            if (excess > 0)
            {
                this._turns.RemoveRange(0, excess);
            }
        }
    }

    /// <summary>
    ///     Clears the pending clarification once it has been answered.
    /// </summary>
    public void ResolveClarification()
    {
        this.PendingClarification = null;
        this.PendingUtterance = null;
    }
}

/// <summary>
///     Conversation persistence for multi-turn flows.
/// </summary>
public interface IConversationStore
{
    ValueTask<AiConversation?> GetAsync(string conversationId, CancellationToken cancellationToken = default);

    ValueTask SaveAsync(AiConversation conversation, CancellationToken cancellationToken = default);

    ValueTask<AiConversation> GetOrCreateAsync(string conversationId, string? userId = null, string? tenantId = null, CancellationToken cancellationToken = default);
}

/// <summary>
///     In-memory conversation store for single-node / tests. Replace in production for distributed scenarios.
/// </summary>
public sealed class InMemoryConversationStore : IConversationStore
{
    private readonly ConcurrentDictionary<string, AiConversation> _items = new(StringComparer.Ordinal);

    #region IConversationStore Members

    /// <inheritdoc />
    public ValueTask<AiConversation?> GetAsync(string conversationId, CancellationToken cancellationToken = default)
    {
        this._items.TryGetValue(conversationId, out AiConversation? conversation);
        return ValueTask.FromResult(conversation);
    }

    /// <inheritdoc />
    public ValueTask SaveAsync(AiConversation conversation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(conversation);
        this._items[conversation.ConversationId] = conversation;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask<AiConversation> GetOrCreateAsync(string conversationId, string? userId = null, string? tenantId = null, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(this._items.GetOrAdd(conversationId, id => new AiConversation(id, userId, tenantId)));

    #endregion
}