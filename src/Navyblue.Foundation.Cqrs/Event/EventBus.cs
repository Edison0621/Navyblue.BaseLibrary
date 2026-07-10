// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : EventBus.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="EventBus.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Bus for sending events
/// </summary>
public class EventBus : IDomainEventBus
{
    private readonly IDictionary<Type, object> _eventHandlerWrappers;
    private readonly IRequestHandlerResolver _requestHandlerResolver;

    /// <summary>
    ///     Constructs the bus
    /// </summary>
    /// <param name="requestHandlerResolver">Resolved event handlers</param>
    public EventBus(IRequestHandlerResolver requestHandlerResolver)
    {
        this._requestHandlerResolver = requestHandlerResolver;
        this._eventHandlerWrappers = new ConcurrentDictionary<Type, object>();
    }

    #region IDomainEventBus Members

    /// <summary>
    ///     Sends an event to the bus
    /// </summary>
    /// <param name="event" cref="Event">Event being published</param>
    /// <returns>Completed Task</returns>
    public Task Send(Event @event)
    {
        Type eventType = @event.GetType();
        if (!this._eventHandlerWrappers.ContainsKey(eventType))
        {
            object eventHandlerWrapper = Activator.CreateInstance(typeof(EventHandlerWrapper<>)
                .MakeGenericType(eventType));
            this._eventHandlerWrappers.Add(eventType, eventHandlerWrapper);
        }

        IEventHandlerWrapper eventHandler = (IEventHandlerWrapper)this._eventHandlerWrappers[eventType];
        return eventHandler.Handle(@event, this._requestHandlerResolver);
    }

    /// <summary>
    ///     Sends a stream of event concurrently to the bus
    /// </summary>
    /// <param name="events" cref="IEnumerable{Event}">Event stream</param>
    /// <returns>Completed Task</returns>
    public async Task Send(IEnumerable<Event> events)
    {
        if (events == null)
            return;

        IList<Task> sendEvents = events.Select(@event => this.Send(@event)).ToList();

        await Task.WhenAll(sendEvents);
    }

    #endregion
}