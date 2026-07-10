// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IInboxStore.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IInboxStore.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// </summary>
public interface IInboxStore
{
    /// <summary>
    ///     Tries the get response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="requestId">The request identifier.</param>
    /// <param name="response">The response.</param>
    /// <returns></returns>
    Task<bool> TryGetResponse<TResponse>(string requestId, out TResponse response);

    /// <summary>
    ///     Sets the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="requestId">The request identifier.</param>
    /// <param name="response">The response.</param>
    /// <returns></returns>
    Task SetResponse<TResponse>(string requestId, TResponse response);

    /// <summary>
    ///     Determines whether [is processed asynchronous] [the specified request identifier].
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <returns></returns>
    Task<bool> IsProcessedAsync(string requestId);

    /// <summary>
    ///     Marks the processed asynchronous.
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <returns></returns>
    Task MarkProcessedAsync(string requestId);
}