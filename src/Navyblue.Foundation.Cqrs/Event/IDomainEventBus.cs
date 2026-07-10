// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IDomainEventBus.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IDomainEventBus.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Bus for sending events
/// </summary>
public interface IDomainEventBus
{
    /// <summary>
    ///     Sends an event to the bus
    /// </summary>
    /// <param name="event" cref="Event">Event being published</param>
    /// <returns>Completed Task</returns>
    Task Send(Event @event);

    /// <summary>
    ///     Sends a stream of event to the bus
    /// </summary>
    /// <param name="events" cref="IEnumerable{Event}">Event stream</param>
    /// <returns>Completed Task</returns>
    Task Send(IEnumerable<Event> events);
}