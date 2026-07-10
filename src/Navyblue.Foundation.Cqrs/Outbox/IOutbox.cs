// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IOutbox.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IOutbox.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// 
/// </summary>
public interface IOutbox
{
    /// <summary>
    /// Saves the asynchronous.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <returns></returns>
    Task SaveAsync(Event @event);

    /// <summary>
    /// Saves the asynchronous.
    /// </summary>
    /// <param name="events">The events.</param>
    /// <returns></returns>
    Task SaveAsync(IEnumerable<Event> events);
}