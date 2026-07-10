// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : OutboxBehavior.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="OutboxBehavior.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     The outbox behavior.
/// </summary>
/// <typeparam name="TRequest" />
/// <typeparam name="TResponse" />
public class OutboxBehavior<TRequest, TResponse> : IRequestBehavior<TRequest, TResponse>, IOrderedBehavior
    where TRequest : IRequest<TResponse>
{
    private readonly IRequestEventCollector _collector;
    private readonly IOutbox _outbox;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OutboxBehavior" /> class.
    /// </summary>
    /// <param name="collector">The collector.</param>
    /// <param name="outbox">The outbox.</param>
    public OutboxBehavior(IRequestEventCollector collector, IOutbox outbox)
    {
        this._collector = collector;
        this._outbox = outbox;
    }

    #region IOrderedBehavior Members

    /// <summary>
    ///     Gets the order.
    /// </summary>
    public int Order => -900;

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
        TResponse response = await next();
        if (this._outbox != null)
        {
            IEnumerable<Event> events = this._collector?.Drain();
            if (events != null)
                await this._outbox.SaveAsync(events);
        }

        return response;
    }

    #endregion
}