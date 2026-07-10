// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : IQueryService.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:22
// ******************************************************************************************************
// <copyright file="IQueryService.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System.Threading.Tasks;

namespace Navyblue.Foundation.Cqrs
{
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
}