// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : QueryModels.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="QueryModels.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Application;

/// <summary>
/// </summary>
public enum SortDirection
{
    /// <summary>
    ///     The ascending
    /// </summary>
    Ascending,

    /// <summary>
    ///     The descending
    /// </summary>
    Descending
}

/// <summary>
/// </summary>
public enum FilterOperator
{
    /// <summary>
    ///     The equal
    /// </summary>
    Equal,

    /// <summary>
    ///     The not equal
    /// </summary>
    NotEqual,

    /// <summary>
    ///     Determines whether this instance contains the object.
    /// </summary>
    Contains,

    /// <summary>
    ///     The starts with
    /// </summary>
    StartsWith,

    /// <summary>
    ///     The ends with
    /// </summary>
    EndsWith,

    /// <summary>
    ///     The greater than
    /// </summary>
    GreaterThan,

    /// <summary>
    ///     The greater than or equal
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    ///     The less than
    /// </summary>
    LessThan,

    /// <summary>
    ///     The less than or equal
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    ///     The in
    /// </summary>
    In,

    /// <summary>
    ///     The between
    /// </summary>
    Between
}

/// <summary>
/// </summary>
public sealed record PageRequest(int PageIndex = 1, int PageSize = 20)
{
    /// <summary>
    ///     Gets the skip.
    /// </summary>
    /// <value>
    ///     The skip.
    /// </value>
    public int Skip => Math.Max(this.PageIndex - 1, 0) * this.PageSize;

    /// <summary>
    ///     Gets the take.
    /// </summary>
    /// <value>
    ///     The take.
    /// </value>
    public int Take => this.PageSize;

    /// <summary>
    ///     Normalizes the specified maximum page size.
    /// </summary>
    /// <param name="maxPageSize">Maximum size of the page.</param>
    /// <returns></returns>
    public PageRequest Normalize(int maxPageSize = 200)
    {
        int pageIndex = this.PageIndex <= 0 ? 1 : this.PageIndex;
        int pageSize = this.PageSize <= 0 ? 20 : Math.Min(this.PageSize, maxPageSize);
        return new PageRequest(pageIndex, pageSize);
    }
}

/// <summary>
/// </summary>
public sealed record SortDescriptor(string Field, SortDirection Direction = SortDirection.Ascending);

/// <summary>
/// </summary>
public sealed record FilterDescriptor(string Field, FilterOperator Operator, string? Value, string? SecondValue = null);

/// <summary>
/// </summary>
public sealed class QueryRequest
{
    /// <summary>
    ///     Gets the page.
    /// </summary>
    /// <value>
    ///     The page.
    /// </value>
    public PageRequest Page { get; init; } = new();

    /// <summary>
    ///     Gets the sorts.
    /// </summary>
    /// <value>
    ///     The sorts.
    /// </value>
    public IList<SortDescriptor> Sorts { get; init; } = [];

    /// <summary>
    ///     Gets the filters.
    /// </summary>
    /// <value>
    ///     The filters.
    /// </value>
    public IList<FilterDescriptor> Filters { get; init; } = [];

    /// <summary>
    ///     Gets the keyword.
    /// </summary>
    /// <value>
    ///     The keyword.
    /// </value>
    public string? Keyword { get; init; }
}

/// <summary>
/// </summary>
public static class PageResultFactory
{
    /// <summary>
    ///     Converts to pageresult.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items">The items.</param>
    /// <param name="total">The total.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static PageResult<T> ToPageResult<T>(this IReadOnlyList<T> items, long total, PageRequest request)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(request);
        PageRequest page = request.Normalize();
        return new PageResult<T>(items, total, page.PageIndex, page.PageSize);
    }
}