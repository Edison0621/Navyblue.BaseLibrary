// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : RemoteQueryService.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="RemoteQueryService.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Text.Json;
using Navyblue.Foundation.Cqrs.Exceptions;
using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// Query service for executing queries from a serialized remote payload.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Cqrs.IRemoteQueryService" />
public class RemoteQueryService : IRemoteQueryService
{
    private readonly IRequestHandlerResolver _handlerResolver;
    private readonly IDictionary<string, Tuple<Type, Type>> _queries;
    private readonly Dictionary<Type, object> _queryHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteQueryService"/> class.
    /// </summary>
    /// <param name="handlerResolver">The handler resolver.</param>
    /// <param name="queries">The queries.</param>
    public RemoteQueryService(IRequestHandlerResolver handlerResolver, IDictionary<string, Tuple<Type, Type>> queries)
    {
        this._queryHandlers = new Dictionary<Type, object>();
        this._handlerResolver = handlerResolver;
        this._queries = queries?.ToDictionary(pair => pair.Key.ToLowerInvariant(), pair => pair.Value)
                        ?? new Dictionary<string, Tuple<Type, Type>>();
    }

    #region IRemoteQueryService Members

    /// <summary>
    /// Queries the specified query name.
    /// </summary>
    /// <param name="queryName">Name of the query.</param>
    /// <param name="serializedQuery">The serialized query.</param>
    /// <returns></returns>
    /// <exception cref="HandlerNotFoundException"></exception>
    /// <exception cref="InvalidOperationException">Failed to deserialize query '{queryName}'.</exception>
    public Task<object> Query(string queryName, string serializedQuery)
    {
        if (!this._queries.ContainsKey(queryName.ToLowerInvariant()))
            throw new HandlerNotFoundException(queryName.ToLowerInvariant());

        Type queryType = this._queries[queryName.ToLowerInvariant()].Item1;
        Type queryResponseType = this._queries[queryName.ToLowerInvariant()].Item2;

        if (!this._queryHandlers.ContainsKey(queryType))
        {
            object queryHandlerWrapper = Activator.CreateInstance(typeof(RemoteQueryHandlerWrapper<,>).MakeGenericType(queryType, queryResponseType))!;
            this._queryHandlers.Add(queryType, queryHandlerWrapper);
        }

        IRemoteQueryHandlerWrapper? queryHandler = this._queryHandlers[queryType] as IRemoteQueryHandlerWrapper;
        object query = JsonSerializer.Deserialize(serializedQuery, queryType)
                       ?? throw new InvalidOperationException($"Failed to deserialize query '{queryName}'.");
        return queryHandler?.Handle(query, this._handlerResolver)
               ?? Task.FromResult<object>(null!);
    }

    #endregion
}