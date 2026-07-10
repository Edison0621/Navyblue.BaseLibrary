using System.Text.Json;
using Navyblue.Foundation.Cqrs.Exceptions;
using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Query service for executing queries from a serialized remote payload.
/// </summary>
public class RemoteQueryService : IRemoteQueryService
{
    private readonly IRequestHandlerResolver _handlerResolver;
    private readonly IDictionary<string, Tuple<Type, Type>> _queries;
    private readonly Dictionary<Type, object> _queryHandlers;

    public RemoteQueryService(IRequestHandlerResolver handlerResolver, IDictionary<string, Tuple<Type, Type>> queries)
    {
        this._queryHandlers = new Dictionary<Type, object>();
        this._handlerResolver = handlerResolver;
        this._queries = queries?.ToDictionary(pair => pair.Key.ToLowerInvariant(), pair => pair.Value)
                       ?? new Dictionary<string, Tuple<Type, Type>>();
    }

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
}
