// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : QueryHandlerWrapper.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="QueryHandlerWrapper.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Runtime.CompilerServices;
using Navyblue.Foundation.Cqrs.Exceptions;

[assembly: InternalsVisibleTo("Navyblue.Foundation.Cqrs.Tests")]

namespace Navyblue.Foundation.Cqrs.Internal;

/// <summary>
///     Internal implementation for wrapping a Query Handler
/// </summary>
/// <typeparam name="TResponse">Type of response of the query</typeparam>
internal interface IQueryHandlerWrapper<TResponse>
{
    Task<TResponse> Handle(Query<TResponse> request, IRequestHandlerResolver requestHandlerResolver);
}

/// <summary>
///     Internal implementation for wrapping a Query Handler
/// </summary>
/// <typeparam name="TQuery">Type of the query</typeparam>
/// <typeparam name="TResponse">Type of response of the query</typeparam>
internal class QueryHandlerWrapper<TQuery, TResponse> : IQueryHandlerWrapper<TResponse> where TQuery : Query<TResponse>
{
    #region IQueryHandlerWrapper<TResponse> Members

    public async Task<TResponse> Handle(Query<TResponse> request, IRequestHandlerResolver requestHandlerResolver)
    {
        QueryHandler<TQuery, TResponse> handler = requestHandlerResolver.Resolve<QueryHandler<TQuery, TResponse>>();
        if (handler == null)
            throw new HandlerNotFoundException(typeof(QueryHandler<TQuery, TResponse>));

        RequestProcessingManager processingManager = new RequestProcessingManager(requestHandlerResolver);

        await processingManager.HandleRequestPreProcessing<TQuery, TResponse>((TQuery)request);

        IEnumerable<IRequestBehavior<TQuery, TResponse>> behaviors = null;
        try
        {
            behaviors = requestHandlerResolver.ResolveAll<IRequestBehavior<TQuery, TResponse>>();
        }
        catch (HandlerNotFoundException)
        {
        }

        Func<Task<TResponse>> next = () => handler.Handle((TQuery)request);

        if (behaviors != null)
        {
            IOrderedEnumerable<IRequestBehavior<TQuery, TResponse>> ordered = behaviors.OrderBy(b => (b as IOrderedBehavior)?.Order ?? 0);
            foreach (IRequestBehavior<TQuery, TResponse> behavior in ordered.Reverse())
            {
                Func<Task<TResponse>> innerNext = next;
                next = () => behavior.Handle((TQuery)request, innerNext);
            }
        }

        TResponse result = await next();
        await processingManager.HandleRequestPostProcessing((TQuery)request, result);
        return result;
    }

    #endregion
}