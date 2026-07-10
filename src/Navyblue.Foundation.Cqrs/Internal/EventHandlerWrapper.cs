// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : EventHandlerWrapper.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="EventHandlerWrapper.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Navyblue.Foundation.Cqrs.Exceptions;

[assembly: InternalsVisibleTo("Navyblue.Foundation.Cqrs.Tests")]

namespace Navyblue.Foundation.Cqrs.Internal
{
    internal interface IEventHandlerWrapper
    {
        Task Handle(Event @event, IRequestHandlerResolver requestHandlerResolver);
    }

    internal class EventHandlerWrapper<TEvent> : IEventHandlerWrapper where TEvent : Event
    {
        #region IEventHandlerWrapper Members

        public async Task Handle(Event @event, IRequestHandlerResolver requestHandlerResolver)
        {
            try
            {
                IEnumerable<EventHandler<TEvent>> eventHandlers = requestHandlerResolver.ResolveAll<EventHandler<TEvent>>();
                if (eventHandlers != null && eventHandlers.Any())
                {
                    foreach (EventHandler<TEvent> eventHandler in eventHandlers)
                    {
                        await eventHandler.Handle((TEvent)@event);
                    }
                }
            }
            catch (HandlerNotFoundException)
            {
                //Events are allowed to have no handlers
            }
        }

        #endregion
    }
}