// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : EventBus.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="EventBus.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs
{
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
}