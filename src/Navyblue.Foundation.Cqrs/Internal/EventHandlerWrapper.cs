// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : EventHandlerWrapper.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="EventHandlerWrapper.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Runtime.CompilerServices;
using Navyblue.Foundation.Cqrs.Exceptions;

[assembly: InternalsVisibleTo("Navyblue.Foundation.Cqrs.Tests")]

namespace Navyblue.Foundation.Cqrs.Internal;

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