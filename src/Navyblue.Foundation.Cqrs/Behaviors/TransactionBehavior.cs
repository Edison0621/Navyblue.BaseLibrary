using System;
using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
    public class TransactionBehavior<TRequest, TResponse> : IRequestBehavior<TRequest, TResponse>, IOrderedBehavior
        where TRequest : IRequest<TResponse>
    {
        private readonly ICqrsUnitOfWork _uow;

        public TransactionBehavior(ICqrsUnitOfWork uow)
        {
            this._uow = uow;
        }

        public int Order => -1000;

        public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
        {
            if (this._uow == null)
                return await next();

            await this._uow.BeginAsync();
            try
            {
                TResponse response = await next();
                await this._uow.CommitAsync();
                return response;
            }
            catch
            {
                try
                {
                    await this._uow.RollbackAsync();
                }
                catch
                {
                    // ignored
                }

                throw;
            }
        }
    }
}