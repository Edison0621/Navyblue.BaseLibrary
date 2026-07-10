// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : InboxBehavior.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="InboxBehavior.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <seealso cref="Navyblue.Foundation.Cqrs.IRequestBehavior&lt;TRequest, TResponse&gt;" />
/// <seealso cref="Navyblue.Foundation.Cqrs.IOrderedBehavior" />
public class InboxBehavior<TRequest, TResponse> : IRequestBehavior<TRequest, TResponse>, IOrderedBehavior
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    ///     The store
    /// </summary>
    private readonly IInboxStore _store;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InboxBehavior{TRequest, TResponse}" /> class.
    /// </summary>
    /// <param name="store">The store.</param>
    public InboxBehavior(IInboxStore store)
    {
        this._store = store;
    }

    #region IOrderedBehavior Members

    /// <summary>
    ///     Gets the order.
    /// </summary>
    /// <value>
    ///     The order.
    /// </value>
    public int Order => -800;

    #endregion

    #region IRequestBehavior<TRequest,TResponse> Members

    /// <summary>
    ///     Handles the specified request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The next.</param>
    /// <returns></returns>
    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next)
    {
        if (this._store != null && !string.IsNullOrEmpty(request.Id))
        {
            if (await this._store.IsProcessedAsync(request.Id))
            {
                if (await this._store.TryGetResponse(request.Id, out TResponse cached))
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

    #endregion
}