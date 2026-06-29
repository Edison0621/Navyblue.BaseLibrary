using System.Linq.Expressions;

namespace Navyblue.BaseLibrary.Data;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    IReadOnlyList<Expression<Func<T, object?>>> Includes { get; }
    IReadOnlyList<OrderExpression<T>> OrderExpressions { get; }
    int? Skip { get; }
    int? Take { get; }
    bool AsNoTracking { get; }
}

public sealed record OrderExpression<T>(Expression<Func<T, object?>> KeySelector, bool Descending = false);

public abstract class Specification<T> : ISpecification<T>
{
    private readonly List<Expression<Func<T, object?>>> _includes = [];
    private readonly List<OrderExpression<T>> _orders = [];

    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public IReadOnlyList<Expression<Func<T, object?>>> Includes => _includes;
    public IReadOnlyList<OrderExpression<T>> OrderExpressions => _orders;
    public int? Skip { get; private set; }
    public int? Take { get; private set; }
    public bool AsNoTracking { get; private set; }

    protected void Where(Expression<Func<T, bool>> criteria) => Criteria = criteria;
    protected void Include(Expression<Func<T, object?>> includeExpression) => _includes.Add(includeExpression);
    protected void OrderBy(Expression<Func<T, object?>> keySelector) => _orders.Add(new OrderExpression<T>(keySelector));
    protected void OrderByDescending(Expression<Func<T, object?>> keySelector) => _orders.Add(new OrderExpression<T>(keySelector, true));
    protected void Page(int pageIndex, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageIndex);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);
        Skip = (pageIndex - 1) * pageSize;
        Take = pageSize;
    }

    protected void NoTracking() => AsNoTracking = true;
}

public static class SpecificationEvaluator
{
    public static IQueryable<T> Apply<T>(IQueryable<T> query, ISpecification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(specification);

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        foreach (var order in specification.OrderExpressions)
        {
            query = order.Descending ? query.OrderByDescending(order.KeySelector) : query.OrderBy(order.KeySelector);
        }

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
