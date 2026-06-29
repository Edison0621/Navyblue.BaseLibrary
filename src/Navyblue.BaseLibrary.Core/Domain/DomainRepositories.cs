namespace Navyblue.BaseLibrary.Domain;

public interface IAggregateRepository<TAggregate, TKey>
    where TAggregate : class, IAggregateRoot<TKey>
{
    ValueTask<TAggregate?> FindAsync(TKey id, CancellationToken cancellationToken = default);
    Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
    void Update(TAggregate aggregate);
    void Remove(TAggregate aggregate);
}

public interface IDomainEventUnitOfWork
{
    ValueTask DispatchDomainEventsAsync(CancellationToken cancellationToken = default);
}
