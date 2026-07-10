using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
    public class InMemoryInboxStore : IInboxStore
    {
        private readonly ConcurrentDictionary<string, object> _responses = new ConcurrentDictionary<string, object>();
        private readonly ConcurrentDictionary<string, bool> _processed = new ConcurrentDictionary<string, bool>();

        public Task<bool> TryGetResponse<TResponse>(string requestId, out TResponse response)
        {
            if (this._responses.TryGetValue(requestId, out object obj) && obj is TResponse res)
            {
                response = res;
                return Task.FromResult(true);
            }

            response = default;
            return Task.FromResult(false);
        }

        public Task SetResponse<TResponse>(string requestId, TResponse response)
        {
            this._responses[requestId] = response;
            return Task.CompletedTask;
        }

        public Task<bool> IsProcessedAsync(string requestId)
        {
            return Task.FromResult(this._processed.TryGetValue(requestId, out bool done) && done);
        }

        public Task MarkProcessedAsync(string requestId)
        {
            this._processed[requestId] = true;
            return Task.CompletedTask;
        }
    }
}