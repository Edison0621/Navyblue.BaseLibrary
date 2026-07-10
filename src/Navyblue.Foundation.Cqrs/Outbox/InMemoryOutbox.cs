using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
    public class InMemoryOutbox : IOutbox, IOutboxDrain
    {
        private readonly ConcurrentQueue<Event> _queue = new ConcurrentQueue<Event>();

        public Task SaveAsync(Event @event)
        {
            if (@event != null)
                this._queue.Enqueue(@event);
            return Task.CompletedTask;
        }

        public Task SaveAsync(IEnumerable<Event> events)
        {
            if (events != null)
            {
                foreach (Event e in events)
                    this._queue.Enqueue(e);
            }

            return Task.CompletedTask;
        }

        public IEnumerable<Event> Drain()
        {
            List<Event> list = new List<Event>();
            while (this._queue.TryDequeue(out Event e))
                list.Add(e);
            return list;
        }
    }
}