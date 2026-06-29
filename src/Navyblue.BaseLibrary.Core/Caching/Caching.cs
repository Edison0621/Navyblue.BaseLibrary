namespace Navyblue.BaseLibrary.Caching;

public sealed class CacheOptions { public string Prefix { get; set; } = "navyblue"; public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(20); }
public sealed record CacheEntryOptions(TimeSpan? AbsoluteExpirationRelativeToNow = null, TimeSpan? SlidingExpiration = null);
public interface ICacheProvider { ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default); ValueTask SetAsync<T>(string key, T value, CacheEntryOptions? options = null, CancellationToken cancellationToken = default); ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default); ValueTask<T> GetOrSetAsync<T>(string key, Func<CancellationToken, ValueTask<T>> factory, CacheEntryOptions? options = null, CancellationToken cancellationToken = default); }
public interface IDistributedCacheProvider : ICacheProvider;
public sealed class CacheKeyBuilder { private readonly List<string> _segments = []; public CacheKeyBuilder(string prefix) => Add(prefix); public CacheKeyBuilder Add(string? segment) { if (!string.IsNullOrWhiteSpace(segment)) _segments.Add(segment.Trim().Replace(' ', '-').ToLowerInvariant()); return this; } public CacheKeyBuilder Add<T>(T value) => Add(value?.ToString()); public override string ToString() => string.Join(':', _segments); }
