// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : SpyEventBus.cs
// Created          : 2026-07-09  16:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="SpyEventBus.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Events;

namespace Navyblue.Foundation.Testing;

/// <summary>
///     Records published integration events for assertions.
/// </summary>
public sealed class SpyEventBus(IEventBus? inner = null) : IEventBus
{
    private readonly List<object> _published = [];

    /// <summary>
    ///     Gets published events in order.
    /// </summary>
    public IReadOnlyList<object> Published => this._published;

    #region IEventBus Members

    /// <inheritdoc />
    public async ValueTask PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default) where TEvent : class, IIntegrationEvent
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        this._published.Add(integrationEvent);
        if (inner is not null)
        {
            await inner.PublishAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public ValueTask SubscribeAsync<TEvent, TConsumer>(CancellationToken cancellationToken = default)
        where TEvent : class, IIntegrationEvent
        where TConsumer : IMessageConsumer<TEvent>
        => inner?.SubscribeAsync<TEvent, TConsumer>(cancellationToken) ?? ValueTask.CompletedTask;

    #endregion

    /// <summary>
    ///     Clears recorded events.
    /// </summary>
    public void Clear() => this._published.Clear();

    /// <summary>
    ///     Returns published events of type <typeparamref name="T" />.
    /// </summary>
    public IReadOnlyList<T> OfType<T>() where T : class, IIntegrationEvent => this._published.OfType<T>().ToArray();
}