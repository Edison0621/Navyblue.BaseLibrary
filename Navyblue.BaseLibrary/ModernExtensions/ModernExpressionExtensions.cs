// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernExpressionExtensions.cs
// Created          : 2026-06-30  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:00
// ****************************************************************************************************************************************
// <copyright file="ModernExpressionExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Linq.Expressions;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public static class ModernExpressionExtensions
{
    /// <summary>
    ///     Ands the specified right.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     left
    ///     or
    ///     right
    /// </exception>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        if (left == null) throw new ArgumentNullException(nameof(left));
        if (right == null) throw new ArgumentNullException(nameof(right));
        return left.Compose(right, Expression.AndAlso);
    }

    /// <summary>
    ///     Ors the specified right.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     left
    ///     or
    ///     right
    /// </exception>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        if (left == null) throw new ArgumentNullException(nameof(left));
        if (right == null) throw new ArgumentNullException(nameof(right));
        return left.Compose(right, Expression.OrElse);
    }

    /// <summary>
    ///     Nots the specified expression.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">expression</exception>
    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        return Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters);
    }

    /// <summary>
    ///     Wheres if.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression">The expression.</param>
    /// <param name="condition">if set to <c>true</c> [condition].</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     expression
    ///     or
    ///     predicate
    /// </exception>
    public static Expression<Func<T, bool>> WhereIf<T>(this Expression<Func<T, bool>> expression, bool condition, Expression<Func<T, bool>> predicate)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return condition ? expression.And(predicate) : expression;
    }

    /// <summary>
    ///     Replaces the parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     expression
    ///     or
    ///     parameter
    /// </exception>
    public static Expression<Func<T, TResult>> ReplaceParameter<T, TResult>(this Expression<Func<T, TResult>> expression, ParameterExpression parameter)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        if (parameter == null) throw new ArgumentNullException(nameof(parameter));
        Expression body = new ParameterReplaceVisitor(expression.Parameters[0], parameter).Visit(expression.Body)!;
        return Expression.Lambda<Func<T, TResult>>(body, parameter);
    }

    /// <summary>
    ///     Composes the specified right.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <param name="merge">The merge.</param>
    /// <returns></returns>
    private static Expression<Func<T, bool>> Compose<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, Func<Expression, Expression, Expression> merge)
    {
        ParameterExpression parameter = left.Parameters[0];
        Expression rightBody = new ParameterReplaceVisitor(right.Parameters[0], parameter).Visit(right.Body)!;
        return Expression.Lambda<Func<T, bool>>(merge(left.Body, rightBody), parameter);
    }

    #region Nested type: ParameterReplaceVisitor

    /// <summary>
    /// </summary>
    /// <seealso cref="System.Linq.Expressions.ExpressionVisitor" />
    private sealed class ParameterReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParameterReplaceVisitor" /> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public ParameterReplaceVisitor(ParameterExpression from, ParameterExpression to)
        {
            this._from = from;
            this._to = to;
        }

        /// <summary>
        ///     Visits the <see cref="T:System.Linq.Expressions.ParameterExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == this._from ? this._to : base.VisitParameter(node);
        }
    }

    #endregion
}