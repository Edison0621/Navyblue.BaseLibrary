// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : InMemoryOutbox.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="InMemoryOutbox.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Navyblue.Foundation.Cqrs.IOutbox" />
/// <seealso cref="Navyblue.Foundation.Cqrs.IOutboxDrain" />
public class InMemoryOutbox : IOutbox, IOutboxDrain
{
    /// <summary>
    /// The queue
    /// </summary>
    private readonly ConcurrentQueue<Event> _queue = new ConcurrentQueue<Event>();

    #region IOutbox Members

    /// <summary>
    /// Saves the asynchronous.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <returns></returns>
    public Task SaveAsync(Event @event)
    {
        if (@event != null)
            this._queue.Enqueue(@event);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Saves the asynchronous.
    /// </summary>
    /// <param name="events">The events.</param>
    /// <returns></returns>
    public Task SaveAsync(IEnumerable<Event> events)
    {
        if (events != null)
        {
            foreach (Event e in events)
                this._queue.Enqueue(e);
        }

        return Task.CompletedTask;
    }

    #endregion

    #region IOutboxDrain Members

    /// <summary>
    /// Drains this instance.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Event> Drain()
    {
        List<Event> list = new List<Event>();
        while (this._queue.TryDequeue(out Event e))
            list.Add(e);
        return list;
    }

    #endregion
}