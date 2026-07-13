using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Authentication;

namespace NavyblueWebApi.Tests.Fakes;

internal sealed class FakeAuthRepository : IAuthRepository
{
    private readonly Dictionary<long, Auth> _byUserId = new();

    public ValueTask<Auth?> FindByLoginAsync(string login, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(_byUserId.Values.FirstOrDefault(a =>
            string.Equals(a.Login, login, StringComparison.OrdinalIgnoreCase)));

    public ValueTask<Auth?> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        _byUserId.TryGetValue(userId, out Auth? auth);
        return ValueTask.FromResult(auth);
    }

    public Task AddAsync(Auth auth, CancellationToken cancellationToken = default)
    {
        _byUserId[auth.UserId] = auth;
        return Task.CompletedTask;
    }

    public void Update(Auth auth) => _byUserId[auth.UserId] = auth;

    public void Remove(Auth auth) => _byUserId.Remove(auth.UserId);
}
