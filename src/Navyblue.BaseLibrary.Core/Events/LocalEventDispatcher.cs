using Microsoft.Extensions.DependencyInjection;
using Navyblue.BaseLibrary.Domain;

namespace Navyblue.BaseLibrary.Events;

public sealed class LocalDomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async ValueTask DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        foreach (var domainEvent in domainEvents)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            foreach (var handler in serviceProvider.GetServices(handlerType))
            {
                var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
                if (method?.Invoke(handler, [domainEvent, cancellationToken]) is ValueTask task)
                {
                    await task.ConfigureAwait(false);
                }
            }
        }
    }
}

public sealed class InMemoryEventBus(IServiceProvider serviceProvider) : IEventBus
{
    public async ValueTask PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default) where TEvent : class, IIntegrationEvent
    {
        foreach (var consumer in serviceProvider.GetServices<IMessageConsumer<TEvent>>())
        {
            await consumer.ConsumeAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
        }
    }

    public ValueTask SubscribeAsync<TEvent, TConsumer>(CancellationToken cancellationToken = default)
        where TEvent : class, IIntegrationEvent
        where TConsumer : IMessageConsumer<TEvent>
    {
        return ValueTask.CompletedTask;
    }
}
