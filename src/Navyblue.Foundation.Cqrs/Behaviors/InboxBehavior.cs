using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
    public class InboxBehavior<TRequest, TResponse> : IRequestBehavior<TRequest, TResponse>, IOrderedBehavior
        where TRequest : IRequest<TResponse>
    {
        private readonly IInboxStore _store;

        public InboxBehavior(IInboxStore store)
        {
            this._store = store;
        }

        public int Order => -800;

        public async Task<TResponse> Handle(TRequest request, System.Func<Task<TResponse>> next)
        {
            if (this._store != null && !string.IsNullOrEmpty(request.Id))
            {
                if (await this._store.IsProcessedAsync(request.Id))
                {
                    if (await this._store.TryGetResponse<TResponse>(request.Id, out TResponse cached))
                        return cached;
                    return default;
                }

                TResponse response = await next();
                await this._store.SetResponse(request.Id, response);
                await this._store.MarkProcessedAsync(request.Id);
                return response;
            }

            return await next();
        }
    }
}