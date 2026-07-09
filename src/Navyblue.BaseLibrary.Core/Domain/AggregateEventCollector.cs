// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AggregateEventCollector.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="AggregateEventCollector.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Domain;

/// <summary>
///     The aggregate event collector.
/// </summary>
public static class AggregateEventCollector
{
    /// <summary>
    ///     Collect the from.
    /// </summary>
    /// <param name="entities">The entities.</param>
    /// <param name="clearAfterCollect">If true, clear after collect.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><![CDATA[IReadOnlyList<IDomainEvent>]]></returns>
    public static IReadOnlyList<IDomainEvent> CollectFrom(IEnumerable<object> entities, bool clearAfterCollect = true)
    {
        ArgumentNullException.ThrowIfNull(entities);

        List<IDomainEvent> events = new List<IDomainEvent>();
        foreach (IHasDomainEvents entity in entities.OfType<IHasDomainEvents>())
        {
            events.AddRange(entity.DomainEvents);
            if (clearAfterCollect)
            {
                entity.ClearDomainEvents();
            }
        }

        return events;
    }

    /// <summary>
    ///     Converts to the envelopes.
    /// </summary>
    /// <param name="aggregates">The aggregates.</param>
    /// <param name="correlationId">The correlation id.</param>
    /// <param name="causationId">The causation id.</param>
    /// <param name="clearAfterCollect">If true, clear after collect.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><![CDATA[IReadOnlyList<DomainEventEnvelope>]]></returns>
    public static IReadOnlyList<DomainEventEnvelope> ToEnvelopes(IEnumerable<object> aggregates, string? correlationId = null, string? causationId = null, bool clearAfterCollect = true)
    {
        ArgumentNullException.ThrowIfNull(aggregates);

        List<DomainEventEnvelope> envelopes = new List<DomainEventEnvelope>();
        foreach (object aggregate in aggregates)
        {
            if (aggregate is not IHasDomainEvents source)
            {
                continue;
            }

            string aggregateType = aggregate.GetType().Name;
            string? aggregateId = aggregate is IEntity entity ? string.Join(',', entity.GetKeys().Where(x => x is not null)) : null;
            long? version = aggregate is IHasVersion versioned ? versioned.Version : null;

            envelopes.AddRange(source.DomainEvents.Select(e => new DomainEventEnvelope(e, aggregateType, aggregateId, version, correlationId, causationId)));
            if (clearAfterCollect)
            {
                source.ClearDomainEvents();
            }
        }

        return envelopes;
    }
}