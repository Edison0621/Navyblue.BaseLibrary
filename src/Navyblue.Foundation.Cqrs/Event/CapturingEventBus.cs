// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : CapturingEventBus.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="CapturingEventBus.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// </summary>
/// <seealso cref="Navyblue.Foundation.Cqrs.IDomainEventBus" />
public class CapturingEventBus : IDomainEventBus
{
    private readonly IRequestEventCollector _collector;
    private readonly EventBus _inner;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CapturingEventBus" /> class.
    /// </summary>
    /// <param name="inner">The inner.</param>
    /// <param name="collector">The collector.</param>
    public CapturingEventBus(EventBus inner, IRequestEventCollector collector)
    {
        this._inner = inner;
        this._collector = collector;
    }

    #region IDomainEventBus Members

    /// <summary>
    ///     Sends an event to the bus
    /// </summary>
    /// <param name="event">Event being published</param>
    /// <returns>
    ///     Completed Task
    /// </returns>
    public Task Send(Event @event)
    {
        this._collector.Add(@event);
        return this._inner.Send(@event);
    }

    /// <summary>
    ///     Sends a stream of event to the bus
    /// </summary>
    /// <param name="events">Event stream</param>
    /// <returns>
    ///     Completed Task
    /// </returns>
    public Task Send(IEnumerable<Event> events)
    {
        this._collector.AddRange(events);
        return this._inner.Send(events);
    }

    #endregion
}