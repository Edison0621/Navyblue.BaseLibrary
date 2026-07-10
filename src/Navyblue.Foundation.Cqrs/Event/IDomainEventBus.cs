// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : IDomainEventBus.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="IDomainEventBus.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
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
}