// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : QueryHandler.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:22
// ******************************************************************************************************
// <copyright file="QueryHandler.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

namespace Navyblue.Foundation.Cqrs
{
    /// <summary>
    ///     Base handler for query requests
    /// </summary>
    /// <typeparam name="QueryRequest">Query Type</typeparam>
    /// <typeparam name="QueryResponse">Response Type</typeparam>
    public abstract class QueryHandler<QueryRequest, QueryResponse> :
        RequestHandler<QueryRequest, QueryResponse> where QueryRequest : Query<QueryResponse>
    {
    }
}