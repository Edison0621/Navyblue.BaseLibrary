namespace Navyblue.BaseLibrary.Domain;

public interface IEntity
{
    object?[] GetKeys();
}

public interface IEntity<TKey> : IEntity
{
    TKey Id { get; }
}

public interface IAggregateRoot;

public interface IAggregateRoot<TKey> : IAggregateRoot, IEntity<TKey>;

public interface ICreationAudited
{
    DateTimeOffset CreatedAt { get; set; }
    string? CreatedBy { get; set; }
}

public interface IModificationAudited
{
    DateTimeOffset? ModifiedAt { get; set; }
    string? ModifiedBy { get; set; }
}

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTimeOffset? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}

public interface ITenant<TKey>
{
    TKey? TenantId { get; set; }
}

public interface IHasConcurrencyStamp
{
    string ConcurrencyStamp { get; set; }
}

public interface IHasExtraProperties
{
    IDictionary<string, object?> ExtraProperties { get; }
}

public interface IHasVersion
{
    long Version { get; }
}

public abstract class Entity<TKey> : IEntity<TKey>, IEquatable<Entity<TKey>>
{
    protected Entity(TKey id) => Id = id;

    public TKey Id { get; protected set; }

    public virtual object?[] GetKeys() => [Id];

    public virtual bool IsTransient() => EqualityComparer<TKey>.Default.Equals(Id, default!);

    public bool Equals(Entity<TKey>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetUnproxiedType(this) != GetUnproxiedType(other)) return false;
        if (IsTransient() || other.IsTransient()) return false;
        return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj) => obj is Entity<TKey> other && Equals(other);

    public override int GetHashCode()
    {
        return IsTransient()
            ? base.GetHashCode()
            : HashCode.Combine(GetUnproxiedType(this), Id);
    }

    public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right) => Equals(left, right);
    public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right) => !Equals(left, right);

    private static Type GetUnproxiedType(object obj)
    {
        var type = obj.GetType();
        var typeString = type.ToString();
        return typeString.Contains("Castle.Proxies.", StringComparison.Ordinal) && type.BaseType is not null
            ? type.BaseType
            : type;
    }
}

public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>, IHasDomainEvents, IHasVersion
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(TKey id) : base(id)
    {
    }

    public long Version { get; private set; }
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Remove(domainEvent);
    }

    protected void IncrementVersion() => Version++;

    public void ClearDomainEvents() => _domainEvents.Clear();
}

public abstract class FullAuditedEntity<TKey> : Entity<TKey>, ICreationAudited, IModificationAudited, ISoftDelete, IHasConcurrencyStamp
{
    protected FullAuditedEntity(TKey id) : base(id)
    {
    }

    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString("N");
}

public abstract class FullAuditedAggregateRoot<TKey> : AggregateRoot<TKey>, ICreationAudited, IModificationAudited, ISoftDelete, IHasConcurrencyStamp
{
    protected FullAuditedAggregateRoot(TKey id) : base(id)
    {
    }

    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString("N");
}

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
}

public abstract record DomainEvent(Guid EventId, DateTimeOffset OccurredAt) : IDomainEvent
{
    protected DomainEvent() : this(Guid.NewGuid(), DateTimeOffset.UtcNow)
    {
    }
}

public sealed record DomainEventEnvelope(IDomainEvent Event, string AggregateType, string? AggregateId, long? AggregateVersion, string? CorrelationId = null, string? CausationId = null);

public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    ValueTask HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

public interface IDomainEventDispatcher
{
    ValueTask DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

public abstract class FrameworkException : Exception
{
    protected FrameworkException(string message, string code, Exception? innerException = null) : base(message, innerException) => Code = code;
    public string Code { get; }
    public IDictionary<string, object?> DataItems { get; } = new Dictionary<string, object?>();
}

public sealed class BusinessException(string message, string code = "business_error", Exception? innerException = null) : FrameworkException(message, code, innerException);
public sealed class DomainRuleViolationException(string message, string code = "domain_rule_violation") : FrameworkException(message, code);
public sealed class ValidationException(string message, IReadOnlyDictionary<string, string[]>? errors = null) : FrameworkException(message, "validation_error") { public IReadOnlyDictionary<string, string[]> Errors { get; } = errors ?? new Dictionary<string, string[]>(); }
public sealed class NotFoundException(string message, string code = "not_found") : FrameworkException(message, code);
public sealed class ForbiddenException(string message = "Forbidden") : FrameworkException(message, "forbidden");
public sealed class UnauthorizedException(string message = "Unauthorized") : FrameworkException(message, "unauthorized");
public sealed class InfrastructureException(string message, string code = "infrastructure_error", Exception? innerException = null) : FrameworkException(message, code, innerException);
