using NavyblueWebApi.Domain.Authentication;

namespace NavyblueWebApi.Application.Authentication;

/// <summary>
///     Persistence for <see cref="RefreshToken" /> rows (hashed values only).
/// </summary>
public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);

    ValueTask<RefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RefreshToken>> ListActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    void Update(RefreshToken token);
}
