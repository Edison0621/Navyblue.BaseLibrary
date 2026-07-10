// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : RequestHandlerResolver.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="RequestHandlerResolver.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Cqrs.Exceptions;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Resolves a Request Handler by type
/// </summary>
public class RequestHandlerResolver : IRequestHandlerResolver
{
    private readonly Func<Type, object> _resolver;

    /// <summary>
    ///     Constructs the Resolver
    /// </summary>
    /// <param name="resolver">Delegate for resolving a handler</param>
    public RequestHandlerResolver(Func<Type, object> resolver)
    {
        this._resolver = resolver;
    }

    #region IRequestHandlerResolver Members

    /// <summary>
    ///     Returns an handler
    /// </summary>
    /// <typeparam name="T">Type of the handler</typeparam>
    /// <returns>Resolved concrete implementation</returns>
    /// <exception cref="HandlerNotFoundException">If no handler is registered for the given type</exception>
    /// <exception cref="HandlerResolutionException">Error ocurred when create the concrete implementation of the resolver</exception>
    public T Resolve<T>()
    {
        try
        {
            T handler = (T)this._resolver(typeof(T));
            if (handler == null)
                throw new HandlerNotFoundException(typeof(T));
            return handler;
        }
        catch (Exception error)
        {
            if (error is HandlerNotFoundException)
                throw;
            throw new HandlerResolutionException(typeof(T), error);
        }
    }

    /// <summary>
    ///     Returns a list of resolvers of the given type
    /// </summary>
    /// <typeparam name="T">Type of the resolver</typeparam>
    /// <returns>List of concrete implementations of the given type</returns>
    /// <exception cref="HandlerNotFoundException">If no handler is registered for the given type</exception>
    /// <exception cref="HandlerResolutionException">Error ocurred when create the concrete implementation of the resolver</exception>
    public IEnumerable<T> ResolveAll<T>()
    {
        try
        {
            IEnumerable<T> handlers = (IEnumerable<T>)this._resolver(typeof(IEnumerable<T>));
            if (handlers == null || !handlers.Any())
                throw new HandlerNotFoundException(typeof(T));
            return handlers;
        }
        catch (Exception error)
        {
            if (error is HandlerNotFoundException)
                throw;
            throw new HandlerResolutionException(typeof(IEnumerable<T>), error);
        }
    }

    #endregion
}