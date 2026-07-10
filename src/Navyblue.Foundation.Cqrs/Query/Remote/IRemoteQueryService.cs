// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IRemoteQueryService.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IRemoteQueryService.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// 
/// </summary>
public interface IRemoteQueryService
{
    /// <summary>
    /// Queries the specified query name.
    /// </summary>
    /// <param name="queryName">Name of the query.</param>
    /// <param name="serializedQuery">The serialized query.</param>
    /// <returns></returns>
    Task<object> Query(string queryName, string serializedQuery);
}