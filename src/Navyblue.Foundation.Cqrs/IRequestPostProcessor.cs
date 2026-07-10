// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IRequestPostProcessor.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IRequestPostProcessor.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Base interface for post-processing a request
/// </summary>
/// <typeparam name="TRequest">Type of the Request received by the handler</typeparam>
/// <typeparam name="TResponse">Type of the Response sent by the handler</typeparam>
public interface IRequestPostProcessor<in TRequest, in TResponse>
{
    /// <summary>
    ///     Processes the request and response after handler has completed execution
    /// </summary>
    /// <param name="request">Request received by the handler</param>
    /// <param name="response">Response sent by the handler</param>
    /// <returns>Completed Task</returns>
    Task Process(TRequest request, TResponse response);
}