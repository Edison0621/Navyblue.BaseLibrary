using Navyblue.Samples.Domain.Authentication;

namespace Navyblue.Samples.Application.Authentication;

/// <summary>
///     Persistence abstraction for the <see cref="Auth" /> credential entity.
/// </summary>
public interface IAuthRepository
{
    ValueTask<Auth?> FindByLoginAsync(string login, CancellationToken cancellationToken = default);

    ValueTask<Auth?> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    Task AddAsync(Auth auth, CancellationToken cancellationToken = default);

    void Update(Auth auth);

    void Remove(Auth auth);
}
