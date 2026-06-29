using System.Linq.Expressions;

namespace Navyblue.BaseLibrary.Data;

public sealed record PageQuery(int PageIndex = 1, int PageSize = 20) { public int Skip => Math.Max(PageIndex - 1, 0) * PageSize; }
public sealed record PageData<T>(IReadOnlyList<T> Items, long Total, int PageIndex, int PageSize);
public interface IReadOnlyRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Queryable { get; }
    ValueTask<TEntity?> FindAsync(object?[] keys, CancellationToken cancellationToken = default);
    Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<PageData<TEntity>> PageAsync(PageQuery query, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
}
public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
public interface IRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : class { ValueTask<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default); }
public interface IUnitOfWork { Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); }
public interface IDataFilter { bool IsEnabled<TFilter>(); IDisposable Disable<TFilter>(); IDisposable Enable<TFilter>(); }
public interface ISoftDeleteFilter;
public interface IMultiTenantFilter;
