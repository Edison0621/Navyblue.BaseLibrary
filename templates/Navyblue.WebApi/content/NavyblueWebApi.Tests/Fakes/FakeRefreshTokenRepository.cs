using System.Collections.Concurrent;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Authentication;

namespace NavyblueWebApi.Tests.Fakes;

internal sealed class FakeRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ConcurrentDictionary<string, RefreshToken> _byHash = new(StringComparer.Ordinal);

    public Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        if (!_byHash.TryAdd(token.TokenHash, token))
            throw new InvalidOperationException("Duplicate refresh token hash.");
        return Task.CompletedTask;
    }

    public ValueTask<RefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        _byHash.TryGetValue(tokenHash, out RefreshToken? token);
        return ValueTask.FromResult(token);
    }

    public Task<IReadOnlyList<RefreshToken>> ListActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RefreshToken> list = _byHash.Values
            .Where(t => t.UserId == userId && t.IsActive())
            .ToList();
        return Task.FromResult(list);
    }

    public void Update(RefreshToken token) => _byHash[token.TokenHash] = token;
}
