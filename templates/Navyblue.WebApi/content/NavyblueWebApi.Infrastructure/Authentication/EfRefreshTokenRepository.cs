using Microsoft.EntityFrameworkCore;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Infrastructure.Persistence;

namespace NavyblueWebApi.Infrastructure.Authentication;

public sealed class EfRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public EfRefreshTokenRepository(AppDbContext db) => this._db = db;

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);
        await this._db.RefreshTokens.AddAsync(token, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<RefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);
        return await this._db.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<RefreshToken>> ListActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        return await this._db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > now)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public void Update(RefreshToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        this._db.RefreshTokens.Update(token);
    }
}
