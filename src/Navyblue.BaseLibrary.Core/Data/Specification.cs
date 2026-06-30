// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Specification.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:51
// ****************************************************************************************************************************************
// <copyright file="Specification.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Linq.Expressions;

namespace Navyblue.BaseLibrary.Data;

/// <summary>
///     The specification interface.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISpecification<T>
{
    /// <summary>
    ///     Gets the criteria.
    /// </summary>
    /// <value>
    ///     The criteria.
    /// </value>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    ///     Gets the includes.
    /// </summary>
    /// <value>
    ///     The includes.
    /// </value>
    IReadOnlyList<Expression<Func<T, object?>>> Includes { get; }

    /// <summary>
    ///     Gets the order expressions.
    /// </summary>
    /// <value>
    ///     The order expressions.
    /// </value>
    IReadOnlyList<OrderExpression<T>> OrderExpressions { get; }

    /// <summary>
    ///     Gets the skip.
    /// </summary>
    /// <value>
    ///     The skip.
    /// </value>
    int? Skip { get; }

    /// <summary>
    ///     Gets the take.
    /// </summary>
    /// <value>
    ///     The take.
    /// </value>
    int? Take { get; }

    /// <summary>
    ///     Gets a value indicating whether [as no tracking].
    /// </summary>
    /// <value>
    ///     <c>true</c> if [as no tracking]; otherwise, <c>false</c>.
    /// </value>
    bool AsNoTracking { get; }
}

/// <summary>
///     The order expression.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed record OrderExpression<T>(Expression<Func<T, object?>> KeySelector, bool Descending = false);

/// <summary>
///     The specification.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="Navyblue.BaseLibrary.Data.ISpecification&lt;T&gt;" />
public abstract class Specification<T> : ISpecification<T>
{
    /// <summary>
    ///     The includes
    /// </summary>
    private readonly List<Expression<Func<T, object?>>> _includes = [];

    /// <summary>
    ///     The orders
    /// </summary>
    private readonly List<OrderExpression<T>> _orders = [];

    #region ISpecification<T> Members

    /// <summary>
    ///     Gets the criteria.
    /// </summary>
    /// <value>
    ///     The criteria.
    /// </value>
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <summary>
    ///     Gets the includes.
    /// </summary>
    /// <value>
    ///     The includes.
    /// </value>
    public IReadOnlyList<Expression<Func<T, object?>>> Includes => this._includes;

    /// <summary>
    ///     Gets the order expressions.
    /// </summary>
    /// <value>
    ///     The order expressions.
    /// </value>
    public IReadOnlyList<OrderExpression<T>> OrderExpressions => this._orders;

    /// <summary>
    ///     Gets the skip.
    /// </summary>
    /// <value>
    ///     The skip.
    /// </value>
    public int? Skip { get; private set; }

    /// <summary>
    ///     Gets the take.
    /// </summary>
    /// <value>
    ///     The take.
    /// </value>
    public int? Take { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether as no tracking.
    /// </summary>
    /// <value>
    ///     <c>true</c> if [as no tracking]; otherwise, <c>false</c>.
    /// </value>
    public bool AsNoTracking { get; private set; }

    #endregion

    /// <summary>
    ///     Wheres the specified criteria.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    protected void Where(Expression<Func<T, bool>> criteria) => this.Criteria = criteria;

    /// <summary>
    ///     Includes the specified include expression.
    /// </summary>
    /// <param name="includeExpression">The include expression.</param>
    protected void Include(Expression<Func<T, object?>> includeExpression) => this._includes.Add(includeExpression);

    /// <summary>
    ///     Orders the by.
    /// </summary>
    /// <param name="keySelector">The key selector.</param>
    protected void OrderBy(Expression<Func<T, object?>> keySelector) => this._orders.Add(new OrderExpression<T>(keySelector));

    /// <summary>
    ///     Orders the by descending.
    /// </summary>
    /// <param name="keySelector">The key selector.</param>
    protected void OrderByDescending(Expression<Func<T, object?>> keySelector) => this._orders.Add(new OrderExpression<T>(keySelector, true));

    /// <summary>
    ///     Pages the specified page index.
    /// </summary>
    /// <param name="pageIndex">Index of the page.</param>
    /// <param name="pageSize">Size of the page.</param>
    protected void Page(int pageIndex, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageIndex);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);
        this.Skip = (pageIndex - 1) * pageSize;
        this.Take = pageSize;
    }

    /// <summary>
    ///     Noes the tracking.
    /// </summary>
    protected void NoTracking() => this.AsNoTracking = true;
}

/// <summary>
///     The specification evaluator.
/// </summary>
public static class SpecificationEvaluator
{
    /// <summary>
    ///     Applies and returns an iqueryable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query">The query.</param>
    /// <param name="specification">The specification.</param>
    /// <returns>
    ///     <![CDATA[IQueryable<T>]]>
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IQueryable<T> Apply<T>(IQueryable<T> query, ISpecification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(specification);

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        query = specification.OrderExpressions.Aggregate(query, (current, order) => order.Descending ? current.OrderByDescending(order.KeySelector) : current.OrderBy(order.KeySelector));

        if (specification.Skip.HasValue)
        {
            query = query.Skip(specification.Skip.Value);
        }

        if (specification.Take.HasValue)
        {
            query = query.Take(specification.Take.Value);
        }

        return query;
    }
}