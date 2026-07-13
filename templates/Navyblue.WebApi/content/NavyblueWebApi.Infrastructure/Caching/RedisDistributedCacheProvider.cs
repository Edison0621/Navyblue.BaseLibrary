using System.Text.Json;
using Microsoft.Extensions.Options;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Configuration;
using StackExchange.Redis;

namespace NavyblueWebApi.Infrastructure.Caching;

/// <summary>
///     StackExchange.Redis implementation of <see cref="IDistributedCacheProvider" />.
/// </summary>
public sealed class RedisDistributedCacheProvider : IDistributedCacheProvider
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IConnectionMultiplexer _multiplexer;
    private readonly string _prefix;
    private readonly TimeSpan _defaultExpiration;

    public RedisDistributedCacheProvider(
        IConnectionMultiplexer multiplexer,
        IOptions<RedisOptions> redisOptions,
        IOptions<CacheOptions>? cacheOptions = null)
    {
        this._multiplexer = multiplexer;
        RedisOptions redis = redisOptions.Value;
        string instance = string.IsNullOrWhiteSpace(redis.InstanceName) ? "navyblue" : redis.InstanceName.Trim();
        CacheOptions cache = cacheOptions?.Value ?? new CacheOptions();
        this._prefix = string.IsNullOrWhiteSpace(cache.Prefix)
            ? $"{instance}:"
            : $"{instance}:{cache.Prefix.Trim()}:";
        this._defaultExpiration = cache.DefaultExpiration <= TimeSpan.Zero
            ? TimeSpan.FromMinutes(20)
            : cache.DefaultExpiration;
    }

    public async ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        RedisValue value = await this.Database.StringGetAsync(this.Prefixed(key)).ConfigureAwait(false);
        if (value.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>((byte[])value!, SerializerOptions);
    }

    public async ValueTask SetAsync<T>(string key, T value, CacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        TimeSpan expiry = options?.AbsoluteExpirationRelativeToNow
            ?? options?.SlidingExpiration
            ?? this._defaultExpiration;

        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(value, SerializerOptions);
        await this.Database.StringSetAsync(this.Prefixed(key), payload, expiry).ConfigureAwait(false);
    }

    public async ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        await this.Database.KeyDeleteAsync(this.Prefixed(key)).ConfigureAwait(false);
    }

    public async ValueTask<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        CacheEntryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(factory);

        RedisValue existing = await this.Database.StringGetAsync(this.Prefixed(key)).ConfigureAwait(false);
        if (!existing.IsNullOrEmpty)
        {
            T? cached = JsonSerializer.Deserialize<T>((byte[])existing!, SerializerOptions);
            if (cached is not null)
                return cached;
        }

        T created = await factory(cancellationToken).ConfigureAwait(false);
        await this.SetAsync(key, created, options, cancellationToken).ConfigureAwait(false);
        return created;
    }

    private IDatabase Database => this._multiplexer.GetDatabase();

    private string Prefixed(string key) => this._prefix + key;
}
