// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Events.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="Events.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Events;

/// <summary>
///     The integration event interface.
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>
    ///     Gets the event identifier.
    /// </summary>
    /// <value>
    ///     The event identifier.
    /// </value>
    Guid EventId { get; }

    /// <summary>
    ///     Gets the occurred at.
    /// </summary>
    /// <value>
    ///     The occurred at.
    /// </value>
    DateTimeOffset OccurredAt { get; }

    /// <summary>
    ///     Gets the name of the event.
    /// </summary>
    /// <value>
    ///     The name of the event.
    /// </value>
    string EventName { get; }
}

/// <summary>
///     The integration event.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Events.IIntegrationEvent" />
public abstract record IntegrationEvent(Guid EventId, DateTimeOffset OccurredAt) : IIntegrationEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IntegrationEvent" /> class.
    /// </summary>
    protected IntegrationEvent() : this(Guid.NewGuid(), DateTimeOffset.UtcNow)
    {
    }

    #region IIntegrationEvent Members

    /// <summary>
    ///     Gets the event name.
    /// </summary>
    /// <value>
    ///     The name of the event.
    /// </value>
    public virtual string EventName => this.GetType().Name;

    #endregion
}

/// <summary>
///     The event bus interface.
/// </summary>
public interface IEventBus
{
    /// <summary>
    ///     Publishes the asynchronous.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="integrationEvent">The integration event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A ValueTask
    /// </returns>
    ValueTask PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default) where TEvent : class, IIntegrationEvent;

    /// <summary>
    ///     Subscribes the asynchronous.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <typeparam name="TConsumer">The type of the consumer.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A ValueTask
    /// </returns>
    ValueTask SubscribeAsync<TEvent, TConsumer>(CancellationToken cancellationToken = default) where TEvent : class, IIntegrationEvent where TConsumer : IMessageConsumer<TEvent>;
}

/// <summary>
///     The message publisher interface.
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    ///     Publishes the asynchronous.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="message">The message.</param>
    /// <param name="topic">The topic.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A ValueTask
    /// </returns>
    ValueTask PublishAsync<TMessage>(TMessage message, string? topic = null, CancellationToken cancellationToken = default) where TMessage : class;
}

/// <summary>
///     The message consumer interface.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
public interface IMessageConsumer<in TMessage> where TMessage : class
{
    /// <summary>
    ///     Consumes the asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A ValueTask
    /// </returns>
    ValueTask ConsumeAsync(TMessage message, CancellationToken cancellationToken = default);
}

/// <summary>
///     The outbox message.
/// </summary>
public sealed record OutboxMessage(Guid Id, string EventName, string Payload, DateTimeOffset OccurredAt, DateTimeOffset? ProcessedAt = null, string? Error = null);

/// <summary>
///     The outbox store interface.
/// </summary>
public interface IOutboxStore
{
    /// <summary>
    ///     Enqueues the asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A ValueTask
    /// </returns>
    ValueTask EnqueueAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Dequeues and return a valuetask of a read only list of outboxmessages asynchronously.
    /// </summary>
    /// <param name="maxCount">The max count.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     <![CDATA[ValueTask<IReadOnlyList<OutboxMessage>>]]>
    /// </returns>
    ValueTask<IReadOnlyList<OutboxMessage>> DequeueAsync(int maxCount, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Mark the processed asynchronously.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A ValueTask
    /// </returns>
    ValueTask MarkProcessedAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Mark the failed asynchronously.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="error">The error.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A ValueTask
    /// </returns>
    ValueTask MarkFailedAsync(Guid id, string error, CancellationToken cancellationToken = default);
}