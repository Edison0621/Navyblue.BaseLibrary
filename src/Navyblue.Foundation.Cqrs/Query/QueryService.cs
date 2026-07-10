// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : QueryService.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="QueryService.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Single point of entry for serving all query requests
/// </summary>
public class QueryService : IQueryService
{
    private readonly IRequestHandlerResolver _handlerResolver;
    private readonly IDictionary<Type, object> _queryHandlers;

    /// <summary>
    ///     Constructs the query Service
    /// </summary>
    /// <param name="handlerResolver">Resolves Request Handler for every reqeust</param>
    public QueryService(IRequestHandlerResolver handlerResolver)
    {
        this._queryHandlers = new ConcurrentDictionary<Type, object>();
        this._handlerResolver = handlerResolver;
    }

    #region IQueryService Members

    /// <summary>
    ///     Executes the Query
    /// </summary>
    /// <typeparam name="TResponse">Reponse Type of the query result</typeparam>
    /// <param name="query">Query Request</param>
    /// <returns>Response from executing the query</returns>
    public Task<TResponse> Query<TResponse>(Query<TResponse> query)
    {
        Type queryType = query.GetType();

        if (!this._queryHandlers.ContainsKey(queryType))
        {
            // Creates a Query Handler Wrapper
            object queryHandlerWrapper = Activator.CreateInstance(typeof(QueryHandlerWrapper<,>)
                .MakeGenericType(queryType, typeof(TResponse)));
            this._queryHandlers.Add(queryType, queryHandlerWrapper);
        }

        IQueryHandlerWrapper<TResponse> queryHandler = (IQueryHandlerWrapper<TResponse>)this._queryHandlers[queryType];
        return queryHandler.Handle(query, this._handlerResolver);
    }

    #endregion
}