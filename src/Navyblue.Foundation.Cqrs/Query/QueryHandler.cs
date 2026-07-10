// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : QueryHandler.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="QueryHandler.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Base handler for query requests
/// </summary>
/// <typeparam name="QueryRequest">Query Type</typeparam>
/// <typeparam name="QueryResponse">Response Type</typeparam>
public abstract class QueryHandler<QueryRequest, QueryResponse> :
    RequestHandler<QueryRequest, QueryResponse> where QueryRequest : Query<QueryResponse>
{
}