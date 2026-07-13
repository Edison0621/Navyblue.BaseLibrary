using Microsoft.EntityFrameworkCore;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Infrastructure.Persistence;

namespace NavyblueWebApi.Infrastructure.Authentication;

/// <summary>
///     EF Core implementation of <see cref="IAuthRepository" /> (MySQL via Pomelo).
/// </summary>
public sealed class EfAuthRepository : IAuthRepository
{
    private readonly AppDbContext _db;

    public EfAuthRepository(AppDbContext db) => this._db = db;

    public async ValueTask<Auth?> FindByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(login);
        return await this._db.Auths
            .FirstOrDefaultAsync(a => a.Login == login, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<Auth?> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default)
        => await this._db.Auths.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken).ConfigureAwait(false);

    public async Task AddAsync(Auth auth, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(auth);
        await this._db.Auths.AddAsync(auth, cancellationToken).ConfigureAwait(false);
    }

    public void Update(Auth auth)
    {
        ArgumentNullException.ThrowIfNull(auth);
        this._db.Auths.Update(auth);
    }

    public void Remove(Auth auth)
    {
        ArgumentNullException.ThrowIfNull(auth);
        this._db.Auths.Remove(auth);
    }
}
