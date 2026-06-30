// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : PaginatedList.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:50
// ****************************************************************************************************************************************
// <copyright file="PaginatedList.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary;

/// <summary>
///     Interface IPaginatedList
/// </summary>
/// <typeparam name="TEntity">The type of the tt entity.</typeparam>
public interface IPaginatedList<TEntity>
{
    /// <summary>
    ///     Gets a value indicating whether this instance has next page.
    /// </summary>
    /// <value><c>true</c> if this instance has next page; otherwise, <c>false</c>.</value>
    bool HasNextPage { get; }

    /// <summary>
    ///     Gets the items.
    /// </summary>
    /// <value>The items.</value>
    IEnumerable<TEntity> Items { get; set; }

    /// <summary>
    ///     Gets the index of the page.
    /// </summary>
    /// <value>The index of the page.</value>
    int PageIndex { get; set; }

    /// <summary>
    ///     Gets the size of the page.
    /// </summary>
    /// <value>The size of the page.</value>
    int PageSize { get; set; }

    /// <summary>
    ///     Gets the total count.
    /// </summary>
    /// <value>The total count.</value>
    int TotalCount { get; set; }

    /// <summary>
    ///     Gets the total page count.
    /// </summary>
    /// <value>The total page count.</value>
    int TotalPageCount { get; set; }

    /// <summary>
    ///     Convert to another paginated list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="selector">The selector.</param>
    /// <returns>IPaginatedList&lt;T&gt;.</returns>
    IPaginatedList<T> ToPaginated<T>(Func<TEntity, T> selector);
}

/// <summary>
///     PaginatedList.
/// </summary>
/// <typeparam name="TEntity">The type of the t entity.</typeparam>
public class PaginatedList<TEntity> : IPaginatedList<TEntity>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PaginatedList{TEntity}" /> class.
    /// </summary>
    public PaginatedList()
    {
        this.Items = new List<TEntity>();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PaginatedList{TEntity}" /> class.
    /// </summary>
    /// <param name="pageIndex">Index of the page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="totalCount">The total count.</param>
    /// <param name="source">The source.</param>
    public PaginatedList(int pageIndex, int pageSize, int totalCount, IEnumerable<TEntity> source)
    {
        this.Items = source;

        this.PageIndex = pageIndex;
        this.PageSize = pageSize;
        this.TotalCount = totalCount;
        this.TotalPageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    #region IPaginatedList<TEntity> Members

    /// <summary>
    ///     Gets a value indicating whether this instance has next page.
    /// </summary>
    /// <value><c>true</c> if this instance has next page; otherwise, <c>false</c>.</value>
    public bool HasNextPage => this.PageIndex < this.TotalPageCount - 1;

    /// <summary>
    ///     Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public IEnumerable<TEntity> Items { get; set; }

    /// <summary>
    ///     Gets the index of the page.
    /// </summary>
    /// <value>The index of the page.</value>
    public int PageIndex { get; set; }

    /// <summary>
    ///     Gets the size of the page.
    /// </summary>
    /// <value>The size of the page.</value>
    public int PageSize { get; set; }

    /// <summary>
    ///     Gets the total count.
    /// </summary>
    /// <value>The total count.</value>
    public int TotalCount { get; set; }

    /// <summary>
    ///     Gets the total page count.
    /// </summary>
    /// <value>The total page count.</value>
    public int TotalPageCount { get; set; }

    /// <summary>
    ///     Convert to another paginated list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="selector">The selector.</param>
    /// <returns>IPaginatedList&lt;T&gt;.</returns>
    public IPaginatedList<T> ToPaginated<T>(Func<TEntity, T> selector)
    {
        return new PaginatedList<T>(this.PageIndex, this.PageSize, this.TotalCount, this.Items.Select(selector));
    }

    #endregion
}