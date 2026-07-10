using Navyblue.Samples.Domain.Users;

namespace Navyblue.Samples.Application.Users;

/// <summary>
///     Persistence abstraction for the <see cref="User" /> aggregate.
///     Implemented by the infrastructure layer; the application layer depends only on this contract.
/// </summary>
public interface IUserRepository
{
    ValueTask<User?> FindAsync(long id, CancellationToken cancellationToken = default);

    ValueTask<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    void Update(User user);

    void Remove(User user);
}
