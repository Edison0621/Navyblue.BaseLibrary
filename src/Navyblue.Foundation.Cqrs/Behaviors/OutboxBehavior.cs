using System.Collections.Generic;
using System.Threading.Tasks;
using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs
{
    public class OutboxBehavior<TRequest, TResponse> : IRequestBehavior<TRequest, TResponse>, IOrderedBehavior
        where TRequest : IRequest<TResponse>
    {
        private readonly IRequestEventCollector _collector;
        private readonly IOutbox _outbox;

        public OutboxBehavior(IRequestEventCollector collector, IOutbox outbox)
        {
            this._collector = collector;
            this._outbox = outbox;
        }

        public int Order => -900;

        public async Task<TResponse> Handle(TRequest request, System.Func<Task<TResponse>> next)
        {
            TResponse response = await next();
            if (this._outbox != null)
            {
                IEnumerable<Event> events = this._collector?.Drain();
                if (events != null)
                    await this._outbox.SaveAsync(events);
            }

            return response;
        }
    }
}