using System.Collections.Concurrent;
using Navyblue.Samples.Application.Users;
using Navyblue.Samples.Domain.Users;

namespace Navyblue.Samples.Infrastructure.Users;

/// <summary>
///     Thread-safe in-memory <see cref="IUserRepository" /> backed by a <see cref="ConcurrentDictionary{TKey,TValue}" />.
///     Sufficient for the sample and unit tests; replace with a real EF Core repository in production.
/// </summary>
public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<long, User> _store = new();

    public ValueTask<User?> FindAsync(long id, CancellationToken cancellationToken = default)
    {
        this._store.TryGetValue(id, out User? user);
        return ValueTask.FromResult(user);
    }

    public ValueTask<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        User? user = this._store.Values.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        return ValueTask.FromResult(user);
    }

    public Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<User> snapshot = this._store.Values.OrderBy(u => u.Id).ToList();
        return Task.FromResult(snapshot);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        if (!this._store.TryAdd(user.Id, user))
            throw new InvalidOperationException($"User '{user.Id}' already exists in the in-memory store.");
        return Task.CompletedTask;
    }

    public void Update(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        this._store[user.Id] = user;
    }

    public void Remove(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        this._store.TryRemove(user.Id, out _);
    }
}
