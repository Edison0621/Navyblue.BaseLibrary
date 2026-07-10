// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IQueryService.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IQueryService.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Interface for the Query Service
/// </summary>
public interface IQueryService
{
    /// <summary>
    ///     Executes the query
    /// </summary>
    /// <typeparam name="TResponse">Type of ther query response</typeparam>
    /// <param name="query">Query request</param>
    /// <returns>Query Response</returns>
    Task<TResponse> Query<TResponse>(Query<TResponse> query);
}