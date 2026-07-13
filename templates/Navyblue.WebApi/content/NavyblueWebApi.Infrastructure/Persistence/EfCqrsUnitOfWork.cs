using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Data;

namespace NavyblueWebApi.Infrastructure.Persistence;

/// <summary>
///     EF Core unit of work used by <c>TransactionBehavior&lt;,&gt;</c>.
///     Begins a MySQL transaction, persists changes on commit, rolls back on failure.
/// </summary>
public sealed class EfCqrsUnitOfWork : ICqrsUnitOfWork, IUnitOfWork
{
    private readonly AppDbContext _db;
    private IDbContextTransaction? _transaction;

    public EfCqrsUnitOfWork(AppDbContext db) => this._db = db;

    public async Task BeginAsync()
    {
        if (this._transaction is not null)
            return;

        this._transaction = await this._db.Database.BeginTransactionAsync().ConfigureAwait(false);
    }

    public async Task CommitAsync()
    {
        await this._db.SaveChangesAsync().ConfigureAwait(false);

        if (this._transaction is null)
            return;

        await this._transaction.CommitAsync().ConfigureAwait(false);
        await this._transaction.DisposeAsync().ConfigureAwait(false);
        this._transaction = null;
    }

    public async Task RollbackAsync()
    {
        if (this._transaction is null)
            return;

        await this._transaction.RollbackAsync().ConfigureAwait(false);
        await this._transaction.DisposeAsync().ConfigureAwait(false);
        this._transaction = null;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => this._db.SaveChangesAsync(cancellationToken);
}
