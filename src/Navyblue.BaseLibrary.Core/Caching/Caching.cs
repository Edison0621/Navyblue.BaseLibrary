// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Caching.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="Caching.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Caching;

/// <summary>
///     The cache options.
/// </summary>
public sealed class CacheOptions
{
    /// <summary>
    ///     Gets or sets the prefix.
    /// </summary>
    public string Prefix { get; set; } = "navyblue";

    /// <summary>
    ///     Gets or sets the default expiration.
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(20);
}

/// <summary>
///     The cache entry options.
/// </summary>
public sealed record CacheEntryOptions(TimeSpan? AbsoluteExpirationRelativeToNow = null, TimeSpan? SlidingExpiration = null);

/// <summary>
///     The cache provider interface.
/// </summary>
public interface ICacheProvider
{
    /// <summary>
    ///     Get and return a valuetask of type <typeparamref name="T" /> asynchronously.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="key">The key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[ValueTask<T?>]]></returns>
    ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask</returns>
    ValueTask SetAsync<T>(string key, T value, CacheEntryOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask</returns>
    ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get or set asynchronously.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="key">The key.</param>
    /// <param name="factory">The factory.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[ValueTask<T>]]></returns>
    ValueTask<T> GetOrSetAsync<T>(string key, Func<CancellationToken, ValueTask<T>> factory, CacheEntryOptions? options = null, CancellationToken cancellationToken = default);
}

/// <summary>
///     The distributed cache provider interface.
/// </summary>
public interface IDistributedCacheProvider : ICacheProvider;

/// <summary>
///     The cache key builder.
/// </summary>
public sealed class CacheKeyBuilder
{
    private readonly List<string> _segments = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="CacheKeyBuilder" /> class.
    /// </summary>
    /// <param name="prefix">The prefix.</param>
    public CacheKeyBuilder(string prefix) => this.Add(prefix);

    /// <summary>
    /// </summary>
    /// <param name="segment">The segment.</param>
    /// <returns>A CacheKeyBuilder</returns>
    public CacheKeyBuilder Add(string? segment)
    {
        if (!string.IsNullOrWhiteSpace(segment)) this._segments.Add(segment.Trim().Replace(' ', '-').ToLowerInvariant());
        return this;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="value">The value.</param>
    /// <returns>A CacheKeyBuilder</returns>
    public CacheKeyBuilder Add<T>(T value) => this.Add(value?.ToString());

    /// <summary>
    ///     Converts to the string.
    /// </summary>
    /// <returns>A string</returns>
    public override string ToString() => string.Join(':', this._segments);
}