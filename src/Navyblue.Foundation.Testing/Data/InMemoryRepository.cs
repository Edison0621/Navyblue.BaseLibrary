// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : InMemoryRepository.cs
// Created          : 2026-07-09  16:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="InMemoryRepository.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Linq.Expressions;
using Navyblue.Foundation.Data;
using Navyblue.Foundation.Domain;

namespace Navyblue.Foundation.Testing;

/// <summary>
///     In-memory <see cref="IRepository{TEntity}" /> for unit tests.
/// </summary>
public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    private readonly List<TEntity> _items = [];
    private readonly object _sync = new();

    /// <summary>
    ///     Gets a snapshot of stored entities.
    /// </summary>
    public IReadOnlyList<TEntity> Items
    {
        get
        {
            lock (this._sync)
            {
                return this._items.ToArray();
            }
        }
    }

    #region IRepository<TEntity> Members

    /// <inheritdoc />
    public IQueryable<TEntity> Queryable
    {
        get
        {
            lock (this._sync)
            {
                return this._items.ToList().AsQueryable();
            }
        }
    }

    /// <inheritdoc />
    public ValueTask<TEntity?> FindAsync(object?[] keys, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        lock (this._sync)
        {
            TEntity? entity = this._items.FirstOrDefault(item => KeysEqual(item.GetKeys(), keys));
            return ValueTask.FromResult(entity);
        }
    }

    /// <inheritdoc />
    public Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        lock (this._sync)
        {
            IEnumerable<TEntity> query = this._items;
            if (predicate is not null)
            {
                query = query.AsQueryable().Where(predicate);
            }

            return Task.FromResult(query.ToList());
        }
    }

    /// <inheritdoc />
    public Task<PageData<TEntity>> PageAsync(PageQuery query, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        lock (this._sync)
        {
            IEnumerable<TEntity> source = this._items;
            if (predicate is not null)
            {
                source = source.AsQueryable().Where(predicate);
            }

            List<TEntity> filtered = source.ToList();
            List<TEntity> page = filtered.Skip(query.Skip).Take(query.PageSize).ToList();
            return Task.FromResult(new PageData<TEntity>(page, filtered.Count, query.PageIndex, query.PageSize));
        }
    }

    /// <inheritdoc />
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        lock (this._sync)
        {
            this._items.Add(entity);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entities);
        lock (this._sync)
        {
            this._items.AddRange(entities);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        lock (this._sync)
        {
            object?[] keys = entity.GetKeys();
            int index = this._items.FindIndex(item => KeysEqual(item.GetKeys(), keys));
            if (index < 0)
            {
                this._items.Add(entity);
                return;
            }

            this._items[index] = entity;
        }
    }

    /// <inheritdoc />
    public void Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        lock (this._sync)
        {
            object?[] keys = entity.GetKeys();
            this._items.RemoveAll(item => KeysEqual(item.GetKeys(), keys));
        }
    }

    #endregion

    /// <summary>
    ///     Clears all entities.
    /// </summary>
    public void Clear()
    {
        lock (this._sync)
        {
            this._items.Clear();
        }
    }

    private static bool KeysEqual(object?[] left, object?[] right)
    {
        if (left.Length != right.Length)
        {
            return false;
        }

        return !left.Where((t, i) => !Equals(t, right[i])).Any();
    }
}

/// <summary>
///     In-memory <see cref="IRepository{TEntity, TKey}" /> for unit tests.
/// </summary>
public sealed class InMemoryRepository<TEntity, TKey> : InMemoryRepository<TEntity>, IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    #region IRepository<TEntity,TKey> Members

    /// <inheritdoc />
    public ValueTask<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
        => this.FindAsync([id!], cancellationToken);

    #endregion
}

/// <summary>
///     In-memory <see cref="IUnitOfWork" /> that records save calls.
/// </summary>
public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    /// <summary>
    ///     Gets how many times <see cref="SaveChangesAsync" /> was called.
    /// </summary>
    public int SaveChangesCallCount { get; private set; }

    /// <summary>
    ///     Gets or sets the value returned by <see cref="SaveChangesAsync" />.
    /// </summary>
    public int SaveChangesResult { get; set; }

    #region IUnitOfWork Members

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.SaveChangesCallCount++;
        return Task.FromResult(this.SaveChangesResult);
    }

    #endregion

    /// <summary>
    ///     Resets call counters.
    /// </summary>
    public void Reset() => this.SaveChangesCallCount = 0;
}