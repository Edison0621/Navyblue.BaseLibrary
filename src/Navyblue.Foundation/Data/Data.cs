// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Data.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="Data.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Linq.Expressions;

namespace Navyblue.Foundation.Data;

/// <summary>
///     The page query.
/// </summary>
public sealed record PageQuery(int PageIndex = 1, int PageSize = 20)
{
    /// <summary>
    ///     Gets the skip.
    /// </summary>
    public int Skip => Math.Max(this.PageIndex - 1, 0) * this.PageSize;
}

/// <summary>
///     The page data.
/// </summary>
/// <typeparam name="T" />
public sealed record PageData<T>(IReadOnlyList<T> Items, long Total, int PageIndex, int PageSize);

/// <summary>
///     The read only repository interface.
/// </summary>
/// <typeparam name="TEntity" />
public interface IReadOnlyRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// </summary>
    IQueryable<TEntity> Queryable { get; }

    /// <summary>
    ///     Finds and return a valuetask of type tentity asynchronously.
    /// </summary>
    /// <param name="keys">The keys.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[ValueTask<TEntity?>]]></returns>
    ValueTask<TEntity?> FindAsync(object?[] keys, CancellationToken cancellationToken = default);

    /// <summary>
    ///     List and return a task of a list of tentities asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[Task<List<TEntity>>]]></returns>
    Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Pages and return a task of type pagedata asynchronously.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[Task<PageData<TEntity>>]]></returns>
    Task<PageData<TEntity>> PageAsync(PageQuery query, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

/// <summary>
///     The repository interface.
/// </summary>
/// <typeparam name="TEntity" />
public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task</returns>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Add the range asynchronously.
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task</returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Update(TEntity entity);

    /// <summary>
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Remove(TEntity entity);
}

/// <summary>
///     The repository interface.
/// </summary>
/// <typeparam name="TEntity" />
/// <typeparam name="TKey" />
// ReSharper disable once TypeParameterCanBeVariant
public interface IRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : class
{
    /// <summary>
    ///     Finds and return a valuetask of type tentity asynchronously.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[ValueTask<TEntity?>]]></returns>
    ValueTask<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);
}

/// <summary>
///     The unit of work interface.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    ///     Save the changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[Task<int>]]></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
///     The data filter interface.
/// </summary>
public interface IDataFilter
{
    /// <summary>
    ///     Checks if is enabled.
    /// </summary>
    /// <typeparam name="TFilter" />
    /// <returns>A bool</returns>
    bool IsEnabled<TFilter>();

    /// <summary>
    /// </summary>
    /// <typeparam name="TFilter" />
    /// <returns>An IDisposable</returns>
    IDisposable Disable<TFilter>();

    /// <summary>
    /// </summary>
    /// <typeparam name="TFilter" />
    /// <returns>An IDisposable</returns>
    IDisposable Enable<TFilter>();
}

/// <summary>
///     The soft delete filter interface.
/// </summary>
public interface ISoftDeleteFilter;

/// <summary>
///     The multi tenant filter interface.
/// </summary>
public interface IMultiTenantFilter;