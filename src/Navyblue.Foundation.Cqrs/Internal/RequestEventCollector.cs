// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : RequestEventCollector.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="RequestEventCollector.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs.Internal;

internal class RequestEventCollector : IRequestEventCollector
{
    private readonly List<Event> _events = new List<Event>();

    #region IRequestEventCollector Members

    public void Add(Event @event)
    {
        if (@event != null)
            this._events.Add(@event);
    }

    public void AddRange(IEnumerable<Event> events)
    {
        if (events == null) return;
        foreach (Event e in events)
            this.Add(e);
    }

    public IEnumerable<Event> Drain()
    {
        List<Event> snapshot = new List<Event>(this._events);
        this._events.Clear();
        return snapshot;
    }

    #endregion
}