using System.Collections.Generic;
using System.Threading.Tasks;
using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs
{
    public class CapturingEventBus : IDomainEventBus
    {
        private readonly EventBus _inner;
        private readonly IRequestEventCollector _collector;

        public CapturingEventBus(EventBus inner, IRequestEventCollector collector)
        {
            this._inner = inner;
            this._collector = collector;
        }

        public Task Send(Event @event)
        {
            this._collector.Add(@event);
            return this._inner.Send(@event);
        }

        public Task Send(IEnumerable<Event> events)
        {
            this._collector.AddRange(events);
            return this._inner.Send(events);
        }
    }
}