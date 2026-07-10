using System;
using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
    // ReSharper disable once TypeParameterCanBeVariant
    public interface IRequestBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next);
    }

    public interface IOrderedBehavior
    {
        int Order { get; }
    }
}