using Navyblue.Foundation.Cqrs;

namespace Navyblue.Samples.Infrastructure.Persistence;

/// <summary>
///     No-op <see cref="ICqrsUnitOfWork" /> for the sample. The in-memory repositories are
///     atomic per operation, so the transaction behavior has nothing to coordinate.
///     Register this so <c>TransactionBehavior&lt;,&gt;</c> (registered by AddNavyblueCqrs) can resolve its dependency.
/// </summary>
public sealed class NullUnitOfWork : ICqrsUnitOfWork
{
    public Task BeginAsync() => Task.CompletedTask;

    public Task CommitAsync() => Task.CompletedTask;

    public Task RollbackAsync() => Task.CompletedTask;
}
