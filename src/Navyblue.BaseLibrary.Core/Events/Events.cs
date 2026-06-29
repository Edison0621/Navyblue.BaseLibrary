namespace Navyblue.BaseLibrary.Events;

public interface IIntegrationEvent { Guid EventId { get; } DateTimeOffset OccurredAt { get; } string EventName { get; } }
public abstract record IntegrationEvent(Guid EventId, DateTimeOffset OccurredAt) : IIntegrationEvent { protected IntegrationEvent() : this(Guid.NewGuid(), DateTimeOffset.UtcNow) { } public virtual string EventName => GetType().Name; }
public interface IEventBus { ValueTask PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default) where TEvent : class, IIntegrationEvent; ValueTask SubscribeAsync<TEvent, TConsumer>(CancellationToken cancellationToken = default) where TEvent : class, IIntegrationEvent where TConsumer : IMessageConsumer<TEvent>; }
public interface IMessagePublisher { ValueTask PublishAsync<TMessage>(TMessage message, string? topic = null, CancellationToken cancellationToken = default) where TMessage : class; }
public interface IMessageConsumer<in TMessage> where TMessage : class { ValueTask ConsumeAsync(TMessage message, CancellationToken cancellationToken = default); }
public sealed record OutboxMessage(Guid Id, string EventName, string Payload, DateTimeOffset OccurredAt, DateTimeOffset? ProcessedAt = null, string? Error = null);
public interface IOutboxStore { ValueTask EnqueueAsync(OutboxMessage message, CancellationToken cancellationToken = default); ValueTask<IReadOnlyList<OutboxMessage>> DequeueAsync(int maxCount, CancellationToken cancellationToken = default); ValueTask MarkProcessedAsync(Guid id, CancellationToken cancellationToken = default); ValueTask MarkFailedAsync(Guid id, string error, CancellationToken cancellationToken = default); }
