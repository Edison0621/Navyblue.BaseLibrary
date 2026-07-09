// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernExpressionExtensions.cs
// Created          : 2026-06-30
// ****************************************************************************************************************************************

#nullable enable
using System.Linq.Expressions;

namespace Navyblue.BaseLibrary.Extensions;

public static class ModernExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        if (left == null) throw new ArgumentNullException(nameof(left));
        if (right == null) throw new ArgumentNullException(nameof(right));
        return left.Compose(right, Expression.AndAlso);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        if (left == null) throw new ArgumentNullException(nameof(left));
        if (right == null) throw new ArgumentNullException(nameof(right));
        return left.Compose(right, Expression.OrElse);
    }

    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        return Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters);
    }

    public static Expression<Func<T, bool>> WhereIf<T>(this Expression<Func<T, bool>> expression, bool condition, Expression<Func<T, bool>> predicate)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return condition ? expression.And(predicate) : expression;
    }

    public static Expression<Func<T, TResult>> ReplaceParameter<T, TResult>(this Expression<Func<T, TResult>> expression, ParameterExpression parameter)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        if (parameter == null) throw new ArgumentNullException(nameof(parameter));
        Expression body = new ParameterReplaceVisitor(expression.Parameters[0], parameter).Visit(expression.Body)!;
        return Expression.Lambda<Func<T, TResult>>(body, parameter);
    }

    private static Expression<Func<T, bool>> Compose<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, Func<Expression, Expression, Expression> merge)
    {
        ParameterExpression parameter = left.Parameters[0];
        Expression rightBody = new ParameterReplaceVisitor(right.Parameters[0], parameter).Visit(right.Body)!;
        return Expression.Lambda<Func<T, bool>>(merge(left.Body, rightBody), parameter);
    }

    private sealed class ParameterReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public ParameterReplaceVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _from ? _to : base.VisitParameter(node);
        }
    }
}
