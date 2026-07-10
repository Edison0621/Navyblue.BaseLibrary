using System.Collections.Generic;

namespace Navyblue.Foundation.Cqrs.Internal
{
    internal class RequestEventCollector : IRequestEventCollector
    {
        private readonly List<Event> _events = new List<Event>();

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
    }
}