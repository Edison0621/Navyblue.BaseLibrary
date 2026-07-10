using System.Collections.Generic;

namespace Navyblue.Foundation.Cqrs.Internal
{
    public interface IRequestEventCollector
    {
        void Add(Event @event);
        void AddRange(IEnumerable<Event> events);
        IEnumerable<Event> Drain();
    }
}