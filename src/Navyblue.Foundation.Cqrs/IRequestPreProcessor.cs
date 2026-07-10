// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IRequestPreProcessor.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IRequestPreProcessor.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Base interface any request pre-processor
/// </summary>
public interface IRequestPreProcessor<in TRequest>
{
    /// <summary>
    ///     Process the request before the handler the can recive it
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <returns>Completed Task</returns>
    Task Process(TRequest request);
}