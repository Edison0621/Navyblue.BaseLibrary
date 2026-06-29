namespace Navyblue.BaseLibrary.Domain;

public static class AggregateEventCollector
{
    public static IReadOnlyList<IDomainEvent> CollectFrom(IEnumerable<object> entities, bool clearAfterCollect = true)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var events = new List<IDomainEvent>();
        foreach (var entity in entities.OfType<IHasDomainEvents>())
        {
            events.AddRange(entity.DomainEvents);
            if (clearAfterCollect)
            {
                entity.ClearDomainEvents();
            }
        }

        return events;
    }

    public static IReadOnlyList<DomainEventEnvelope> ToEnvelopes(IEnumerable<object> aggregates, string? correlationId = null, string? causationId = null, bool clearAfterCollect = true)
    {
        ArgumentNullException.ThrowIfNull(aggregates);

        var envelopes = new List<DomainEventEnvelope>();
        foreach (var aggregate in aggregates)
        {
            if (aggregate is not IHasDomainEvents source)
            {
                continue;
            }

            var aggregateType = aggregate.GetType().Name;
            var aggregateId = aggregate is IEntity entity ? string.Join(',', entity.GetKeys().Where(x => x is not null)) : null;
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

