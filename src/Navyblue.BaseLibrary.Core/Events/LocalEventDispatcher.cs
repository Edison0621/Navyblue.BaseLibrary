// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : LocalEventDispatcher.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-29  13:02
// ****************************************************************************************************************************************
// <copyright file="LocalEventDispatcher.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Navyblue.BaseLibrary.Domain;

namespace Navyblue.BaseLibrary.Events;

/// <summary>
///     The local domain event dispatcher.
/// </summary>
/// <param name="serviceProvider">The service provider.</param>
public sealed class LocalDomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    #region IDomainEventDispatcher Members

    /// <summary>
    /// </summary>
    /// <param name="domainEvents">The domain events.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A ValueTask</returns>
    public async ValueTask DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        foreach (IDomainEvent domainEvent in domainEvents)
        {
            Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            foreach (object? handler in serviceProvider.GetServices(handlerType))
            {
                MethodInfo? method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
                if (method?.Invoke(handler, [domainEvent, cancellationToken]) is ValueTask task)
                {
                    await task.ConfigureAwait(false);
                }
            }
        }
    }

    #endregion
}

/// <summary>
///     The in memory event bus.
/// </summary>
/// <param name="serviceProvider">The service provider.</param>
public sealed class InMemoryEventBus(IServiceProvider serviceProvider) : IEventBus
{
    #region IEventBus Members

    /// <summary>
    /// </summary>
    /// <typeparam name="TEvent" />
    /// <param name="integrationEvent">The integration event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask</returns>
    public async ValueTask PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default) where TEvent : class, IIntegrationEvent
    {
        foreach (IMessageConsumer<TEvent> consumer in serviceProvider.GetServices<IMessageConsumer<TEvent>>())
        {
            await consumer.ConsumeAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TEvent" />
    /// <typeparam name="TConsumer" />
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask</returns>
    public ValueTask SubscribeAsync<TEvent, TConsumer>(CancellationToken cancellationToken = default)
        where TEvent : class, IIntegrationEvent
        where TConsumer : IMessageConsumer<TEvent>
    {
        return ValueTask.CompletedTask;
    }

    #endregion
}