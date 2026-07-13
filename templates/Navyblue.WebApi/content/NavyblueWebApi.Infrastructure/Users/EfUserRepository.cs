using Microsoft.EntityFrameworkCore;
using Navyblue.Foundation.Data;
using NavyblueWebApi.Application.Users;
using NavyblueWebApi.Domain.Users;
using NavyblueWebApi.Infrastructure.Persistence;

namespace NavyblueWebApi.Infrastructure.Users;

/// <summary>
///     EF Core implementation of <see cref="IUserRepository" /> (MySQL via Pomelo).
/// </summary>
public sealed class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public EfUserRepository(AppDbContext db) => this._db = db;

    public async ValueTask<User?> FindAsync(long id, CancellationToken cancellationToken = default)
        => await this._db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken).ConfigureAwait(false);

    public async ValueTask<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        return await this._db.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default)
        => await this._db.Users
            .OrderBy(u => u.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<PageData<User>> PageAsync(PageQuery page, string? keyword = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(page);
        int pageIndex = Math.Max(page.PageIndex, 1);
        int pageSize = Math.Clamp(page.PageSize, 1, 200);

        IQueryable<User> query = this._db.Users.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            string key = keyword.Trim();
            query = query.Where(u => u.Name.Contains(key) || u.Email.Contains(key));
        }

        long total = await query.LongCountAsync(cancellationToken).ConfigureAwait(false);
        List<User> items = await query
            .OrderBy(u => u.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PageData<User>(items, total, pageIndex, pageSize);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        await this._db.Users.AddAsync(user, cancellationToken).ConfigureAwait(false);
    }

    public void Update(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        this._db.Users.Update(user);
    }

    public void Remove(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        this._db.Users.Remove(user);
    }
}
