// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : CacheAside.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-29  13:02
// ****************************************************************************************************************************************
// <copyright file="CacheAside.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Caching;

/// <summary>
///     The cache policy.
/// </summary>
public sealed class CachePolicy
{
    /// <summary>
    ///     Gets the absolute expiration relative converts to now.
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; init; }

    /// <summary>
    ///     Gets the sliding expiration.
    /// </summary>
    public TimeSpan? SlidingExpiration { get; init; }

    /// <summary>
    ///     Gets  a value indicating whether to cache null value.
    /// </summary>
    public bool CacheNullValue { get; init; } = true;

    /// <summary>
    ///     Converts to entry options.
    /// </summary>
    /// <returns>A CacheEntryOptions</returns>
    public CacheEntryOptions ToEntryOptions() => new(this.AbsoluteExpirationRelativeToNow, this.SlidingExpiration);
}

/// <summary>
///     The cache serializer interface.
/// </summary>
public interface ICacheSerializer
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="value">The value.</param>
    /// <returns>An array of byte</returns>
    byte[] Serialize<T>(T value);

    /// <summary>
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="bytes">The bytes.</param>
    /// <returns>A <typeparamref name="T" /></returns>
    T? Deserialize<T>(ReadOnlySpan<byte> bytes);
}

/// <summary>
///     The cache provider extensions.
/// </summary>
public static class CacheProviderExtensions
{
    /// <summary>
    ///     Get or create nullable asynchronously.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="cache">The cache.</param>
    /// <param name="key">The key.</param>
    /// <param name="factory">The factory.</param>
    /// <param name="policy">The policy.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <returns><![CDATA[ValueTask<T?>]]></returns>
    public static async ValueTask<T?> GetOrCreateNullableAsync<T>(this ICacheProvider cache, string key, Func<CancellationToken, ValueTask<T?>> factory, CachePolicy? policy = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(factory);

        CacheBox<T>? cached = await cache.GetAsync<CacheBox<T>>(key, cancellationToken).ConfigureAwait(false);
        if (cached is not null)
        {
            return cached.HasValue ? cached.Value : default;
        }

        T? value = await factory(cancellationToken).ConfigureAwait(false);
        policy ??= new CachePolicy();
        if (value is not null || policy.CacheNullValue)
        {
            await cache.SetAsync(key, new CacheBox<T>(value is not null, value), policy.ToEntryOptions(), cancellationToken).ConfigureAwait(false);
        }

        return value;
    }

    #region Nested type: CacheBox

    private sealed record CacheBox<T>(bool HasValue, T? Value);

    #endregion
}