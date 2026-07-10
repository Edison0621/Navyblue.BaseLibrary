// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : InMemoryInboxStore.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="InMemoryInboxStore.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// </summary>
/// <seealso cref="Navyblue.Foundation.Cqrs.IInboxStore" />
public class InMemoryInboxStore : IInboxStore
{
    /// <summary>
    ///     The processed
    /// </summary>
    private readonly ConcurrentDictionary<string, bool> _processed = new ConcurrentDictionary<string, bool>();

    /// <summary>
    ///     The responses
    /// </summary>
    private readonly ConcurrentDictionary<string, object> _responses = new ConcurrentDictionary<string, object>();

    #region IInboxStore Members

    /// <summary>
    ///     Tries the get response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="requestId">The request identifier.</param>
    /// <param name="response">The response.</param>
    /// <returns></returns>
    public Task<bool> TryGetResponse<TResponse>(string requestId, out TResponse response)
    {
        if (this._responses.TryGetValue(requestId, out object obj) && obj is TResponse res)
        {
            response = res;
            return Task.FromResult(true);
        }

        response = default;
        return Task.FromResult(false);
    }

    /// <summary>
    ///     Sets the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="requestId">The request identifier.</param>
    /// <param name="response">The response.</param>
    /// <returns></returns>
    public Task SetResponse<TResponse>(string requestId, TResponse response)
    {
        this._responses[requestId] = response;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Determines whether [is processed asynchronous] [the specified request identifier].
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <returns></returns>
    public Task<bool> IsProcessedAsync(string requestId)
    {
        return Task.FromResult(this._processed.TryGetValue(requestId, out bool done) && done);
    }

    /// <summary>
    ///     Marks the processed asynchronous.
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <returns></returns>
    public Task MarkProcessedAsync(string requestId)
    {
        this._processed[requestId] = true;
        return Task.CompletedTask;
    }

    #endregion
}