// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : QueryBindingExtensions.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="QueryBindingExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Navyblue.Foundation.Application;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
/// </summary>
public static class QueryBindingExtensions
{
    /// <summary>
    ///     Gets the page request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="defaultPageSize">Default size of the page.</param>
    /// <param name="maxPageSize">Maximum size of the page.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static PageRequest GetPageRequest(this HttpRequest request, int defaultPageSize = 20, int maxPageSize = 200)
    {
        ArgumentNullException.ThrowIfNull(request);
        int pageIndex = request.Query.TryGetValue("pageIndex", out StringValues pageIndexValue) && int.TryParse(pageIndexValue, out int pi) ? pi : 1;
        int pageSize = request.Query.TryGetValue("pageSize", out StringValues pageSizeValue) && int.TryParse(pageSizeValue, out int ps) ? ps : defaultPageSize;
        return new PageRequest(pageIndex, pageSize).Normalize(maxPageSize);
    }

    /// <summary>
    ///     Gets the query request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="defaultPageSize">Default size of the page.</param>
    /// <param name="maxPageSize">Maximum size of the page.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static QueryRequest GetQueryRequest(this HttpRequest request, int defaultPageSize = 20, int maxPageSize = 200)
    {
        ArgumentNullException.ThrowIfNull(request);
        QueryRequest query = new QueryRequest
        {
            Page = request.GetPageRequest(defaultPageSize, maxPageSize),
            Keyword = request.Query.TryGetValue("keyword", out StringValues keyword) ? keyword.FirstOrDefault() : null
        };

        foreach (string? sort in request.Query["sort"].Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            string[] parts = sort!.Split(':', 2, StringSplitOptions.TrimEntries);
            query.Sorts.Add(new SortDescriptor(parts[0], parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending));
        }

        return query;
    }

    /// <summary>
    ///     Gets the int32.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static int? GetInt32(this IQueryCollection query, string name)
    {
        ArgumentNullException.ThrowIfNull(query);
        return query.TryGetValue(name, out StringValues value) && int.TryParse(value.FirstOrDefault(), out int parsed) ? parsed : null;
    }

    /// <summary>
    ///     Gets the boolean.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static bool? GetBoolean(this IQueryCollection query, string name)
    {
        ArgumentNullException.ThrowIfNull(query);
        return query.TryGetValue(name, out StringValues value) && bool.TryParse(value.FirstOrDefault(), out bool parsed) ? parsed : null;
    }

    /// <summary>
    ///     Gets the unique identifier.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static Guid? GetGuid(this IQueryCollection query, string name)
    {
        ArgumentNullException.ThrowIfNull(query);
        return query.TryGetValue(name, out StringValues value) && Guid.TryParse(value.FirstOrDefault(), out Guid parsed) ? parsed : null;
    }
}