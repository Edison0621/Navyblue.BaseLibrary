// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : TransactionBehavior.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="TransactionBehavior.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     The transaction behavior.
/// </summary>
/// <typeparam name="TRequest" />
/// <typeparam name="TResponse" />
public class TransactionBehavior<TRequest, TResponse> : IRequestBehavior<TRequest, TResponse>, IOrderedBehavior
    where TRequest : IRequest<TResponse>
{
    private readonly ICqrsUnitOfWork _uow;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TransactionBehavior" /> class.
    /// </summary>
    /// <param name="uow">The uow.</param>
    public TransactionBehavior(ICqrsUnitOfWork uow)
    {
        this._uow = uow;
    }

    #region IOrderedBehavior Members

    /// <summary>
    ///     Gets the order.
    /// </summary>
    public int Order => -1000;

    #endregion

    #region IRequestBehavior<TRequest,TResponse> Members

    /// <summary>
    ///     Handle and return a task of type tresponse.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The next.</param>
    /// <returns><![CDATA[Task<TResponse>]]></returns>
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

    #endregion
}