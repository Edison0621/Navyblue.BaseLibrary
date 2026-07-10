using System.Collections.Concurrent;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Authentication;

namespace NavyblueWebApi.Infrastructure.Authentication;

/// <summary>
///     Thread-safe in-memory <see cref="IAuthRepository" /> keyed by user id.
/// </summary>
public sealed class InMemoryAuthRepository : IAuthRepository
{
    private readonly ConcurrentDictionary<long, Auth> _byUserId = new();

    public ValueTask<Auth?> FindByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        Auth? auth = this._byUserId.Values.FirstOrDefault(a =>
            string.Equals(a.Login, login, StringComparison.OrdinalIgnoreCase));
        return ValueTask.FromResult(auth);
    }

    public ValueTask<Auth?> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        this._byUserId.TryGetValue(userId, out Auth? auth);
        return ValueTask.FromResult(auth);
    }

    public Task AddAsync(Auth auth, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(auth);
        if (!this._byUserId.TryAdd(auth.UserId, auth))
            throw new InvalidOperationException($"Auth for user '{auth.UserId}' already exists in the in-memory store.");
        return Task.CompletedTask;
    }

    public void Update(Auth auth)
    {
        ArgumentNullException.ThrowIfNull(auth);
        this._byUserId[auth.UserId] = auth;
    }

    public void Remove(Auth auth)
    {
        ArgumentNullException.ThrowIfNull(auth);
        this._byUserId.TryRemove(auth.UserId, out _);
    }
}
