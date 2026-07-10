// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : InMemoryCacheProvider.cs
// Created          : 2026-07-09  16:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="InMemoryCacheProvider.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.Testing;

/// <summary>
///     In-memory <see cref="ICacheProvider" /> / <see cref="IDistributedCacheProvider" /> for tests.
/// </summary>
public sealed class InMemoryCacheProvider(IClock? clock = null) : IDistributedCacheProvider
{
    private readonly IClock _clock = clock ?? new SystemClock();
    private readonly ConcurrentDictionary<string, CacheEntry> _entries = new(StringComparer.Ordinal);

    /// <summary>
    ///     Gets the number of stored entries (including expired ones not yet purged).
    /// </summary>
    public int Count => this._entries.Count;

    #region IDistributedCacheProvider Members

    /// <inheritdoc />
    public ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(key, nameof(key));
        if (!this.TryGet(key, out CacheEntry entry))
        {
            return ValueTask.FromResult<T?>(default);
        }

        return ValueTask.FromResult(entry.Value is T typed ? typed : default);
    }

    /// <inheritdoc />
    public ValueTask SetAsync<T>(string key, T value, CacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(key, nameof(key));
        DateTimeOffset? absolute = null;
        TimeSpan? sliding = options?.SlidingExpiration;
        if (options?.AbsoluteExpirationRelativeToNow is { } absoluteRelative)
        {
            absolute = this._clock.UtcNow.Add(absoluteRelative);
        }
        else if (sliding is { } slidingExpiry)
        {
            absolute = this._clock.UtcNow.Add(slidingExpiry);
        }

        this._entries[key] = new CacheEntry(value, absolute, sliding);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(key, nameof(key));
        this._entries.TryRemove(key, out _);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask<T> GetOrSetAsync<T>(string key, Func<CancellationToken, ValueTask<T>> factory, CacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(factory);
        if (this.TryGet(key, out CacheEntry entry) && entry.Value is T typed)
        {
            return typed;
        }

        T created = await factory(cancellationToken).ConfigureAwait(false);
        await this.SetAsync(key, created, options, cancellationToken).ConfigureAwait(false);
        return created;
    }

    #endregion

    /// <summary>
    ///     Clears all cache entries.
    /// </summary>
    public void Clear() => this._entries.Clear();

    private bool TryGet(string key, out CacheEntry entry)
    {
        if (this._entries.TryGetValue(key, out CacheEntry? found) && !this.IsExpired(found))
        {
            if (found.SlidingExpiration is { } sliding)
            {
                found.AbsoluteExpiration = this._clock.UtcNow.Add(sliding);
            }

            entry = found;
            return true;
        }

        if (found is not null)
        {
            this._entries.TryRemove(key, out _);
        }

        entry = null!;
        return false;
    }

    private bool IsExpired(CacheEntry entry) => entry.AbsoluteExpiration is { } expiry && expiry <= this._clock.UtcNow;

    #region Nested type: CacheEntry

    private sealed class CacheEntry(object? value, DateTimeOffset? absoluteExpiration, TimeSpan? slidingExpiration)
    {
        public object? Value { get; } = value;
        public DateTimeOffset? AbsoluteExpiration { get; set; } = absoluteExpiration;
        public TimeSpan? SlidingExpiration { get; } = slidingExpiration;
    }

    #endregion
}