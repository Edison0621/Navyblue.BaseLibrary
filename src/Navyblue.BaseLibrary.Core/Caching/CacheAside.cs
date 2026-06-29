namespace Navyblue.BaseLibrary.Caching;

public sealed class CachePolicy
{
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; init; }
    public TimeSpan? SlidingExpiration { get; init; }
    public bool CacheNullValue { get; init; } = true;

    public CacheEntryOptions ToEntryOptions() => new(AbsoluteExpirationRelativeToNow, SlidingExpiration);
}

public interface ICacheSerializer
{
    byte[] Serialize<T>(T value);
    T? Deserialize<T>(ReadOnlySpan<byte> bytes);
}

public static class CacheProviderExtensions
{
    public static async ValueTask<T?> GetOrCreateNullableAsync<T>(this ICacheProvider cache, string key, Func<CancellationToken, ValueTask<T?>> factory, CachePolicy? policy = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(factory);

        var cached = await cache.GetAsync<CacheBox<T>>(key, cancellationToken).ConfigureAwait(false);
        if (cached is not null)
        {
            return cached.HasValue ? cached.Value : default;
        }

        var value = await factory(cancellationToken).ConfigureAwait(false);
        policy ??= new CachePolicy();
        if (value is not null || policy.CacheNullValue)
        {
            await cache.SetAsync(key, new CacheBox<T>(value is not null, value), policy.ToEntryOptions(), cancellationToken).ConfigureAwait(false);
        }

        return value;
    }

    private sealed record CacheBox<T>(bool HasValue, T? Value);
}
