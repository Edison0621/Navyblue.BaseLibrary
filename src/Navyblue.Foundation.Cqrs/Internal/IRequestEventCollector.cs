// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IRequestEventCollector.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IRequestEventCollector.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs.Internal;

/// <summary>
/// </summary>
public interface IRequestEventCollector
{
    /// <summary>
    ///     Adds the specified event.
    /// </summary>
    /// <param name="event">The event.</param>
    void Add(Event @event);

    /// <summary>
    ///     Adds the range.
    /// </summary>
    /// <param name="events">The events.</param>
    void AddRange(IEnumerable<Event> events);

    /// <summary>
    ///     Drains this instance.
    /// </summary>
    /// <returns></returns>
    IEnumerable<Event> Drain();
}