// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : InMemoryDomainEventCollector.cs
// Created          : 2026-07-09  16:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  16:06
// ****************************************************************************************************************************************
// <copyright file="InMemoryDomainEventCollector.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Domain;

namespace Navyblue.Foundation.Testing;

/// <summary>
///     Records domain events for assertions. Optionally forwards to an inner dispatcher.
/// </summary>
public sealed class InMemoryDomainEventCollector(IDomainEventDispatcher? inner = null) : IDomainEventDispatcher
{
    private readonly List<IDomainEvent> _events = [];

    /// <summary>
    ///     Gets the recorded domain events.
    /// </summary>
    public IReadOnlyList<IDomainEvent> Events => this._events;

    /// <inheritdoc />
    public async ValueTask DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);
        IDomainEvent[] batch = domainEvents as IDomainEvent[] ?? domainEvents.ToArray();
        this._events.AddRange(batch);
        if (inner is not null)
        {
            await inner.DispatchAsync(batch, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Clears recorded events.
    /// </summary>
    public void Clear() => this._events.Clear();

    /// <summary>
    ///     Returns recorded events of type <typeparamref name="T" />.
    /// </summary>
    public IReadOnlyList<T> OfType<T>() where T : IDomainEvent => this._events.OfType<T>().ToArray();

    /// <summary>
    ///     Asserts exactly one event of type <typeparamref name="T" /> was recorded.
    /// </summary>
    public T AssertSingle<T>() where T : IDomainEvent
    {
        T[] matches = this.OfType<T>().ToArray();
        return matches.Length switch
        {
            0 => throw new InvalidOperationException($"Expected a single {typeof(T).Name} domain event, but none were recorded."),
            1 => matches[0],
            _ => throw new InvalidOperationException($"Expected a single {typeof(T).Name} domain event, but found {matches.Length}.")
        };
    }
}
