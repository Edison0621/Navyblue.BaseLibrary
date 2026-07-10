// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Domain.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="Domain.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics.CodeAnalysis;

namespace Navyblue.Foundation.Domain;

/// <summary>
///     The entity interface.
/// </summary>
public interface IEntity
{
    /// <summary>
    ///     Get the keys.
    /// </summary>
    /// <returns>
    ///     An array of     object?
    /// </returns>
    object?[] GetKeys();
}

/// <summary>
///     The entity interface.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IEntity<TKey> : IEntity
{
    /// <summary>
    ///     Gets the identifier.
    /// </summary>
    /// <value>
    ///     The identifier.
    /// </value>
    TKey Id { get; }
}

/// <summary>
///     The aggregate root interface.
/// </summary>
public interface IAggregateRoot;

/// <summary>
///     The aggregate root interface.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public interface IAggregateRoot<TKey> : IAggregateRoot, IEntity<TKey>;

/// <summary>
///     The creation audited interface.
/// </summary>
public interface ICreationAudited
{
    /// <summary>
    ///     Gets or sets the created at.
    /// </summary>
    /// <value>
    ///     The created at.
    /// </value>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the created by.
    /// </summary>
    /// <value>
    ///     The created by.
    /// </value>
    string? CreatedBy { get; set; }
}

/// <summary>
///     The modification audited interface.
/// </summary>
public interface IModificationAudited
{
    /// <summary>
    ///     Gets or sets the modified at.
    /// </summary>
    /// <value>
    ///     The modified at.
    /// </value>
    DateTimeOffset? ModifiedAt { get; set; }

    /// <summary>
    ///     Gets or sets the modified by.
    /// </summary>
    /// <value>
    ///     The modified by.
    /// </value>
    string? ModifiedBy { get; set; }
}

/// <summary>
///     The soft delete interface.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    ///     Gets or sets a value indicating whether this instance is deleted.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
    /// </value>
    bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the deleted at.
    /// </summary>
    /// <value>
    ///     The deleted at.
    /// </value>
    DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    ///     Gets or sets the deleted by.
    /// </summary>
    /// <value>
    ///     The deleted by.
    /// </value>
    string? DeletedBy { get; set; }
}

/// <summary>
///     The tenant interface.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public interface ITenant<TKey>
{
    /// <summary>
    ///     Gets or sets the tenant identifier.
    /// </summary>
    /// <value>
    ///     The tenant identifier.
    /// </value>
    TKey? TenantId { get; set; }
}

/// <summary>
///     Has concurrency stamp interface.
/// </summary>
public interface IHasConcurrencyStamp
{
    /// <summary>
    ///     Gets or sets the concurrency stamp.
    /// </summary>
    /// <value>
    ///     The concurrency stamp.
    /// </value>
    string ConcurrencyStamp { get; set; }
}

/// <summary>
///     Has extra properties interface.
/// </summary>
public interface IHasExtraProperties
{
    /// <summary>
    ///     Gets the extra properties.
    /// </summary>
    /// <value>
    ///     The extra properties.
    /// </value>
    IDictionary<string, object?> ExtraProperties { get; }
}

/// <summary>
///     Has version interface.
/// </summary>
public interface IHasVersion
{
    /// <summary>
    ///     Gets the version.
    /// </summary>
    /// <value>
    ///     The version.
    /// </value>
    long Version { get; }
}

/// <summary>
///     The entity.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <seealso cref="Navyblue.Foundation.Domain.IEntity&lt;TKey&gt;" />
public abstract class Entity<TKey> : IEntity<TKey>, IEquatable<Entity<TKey>>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Entity{TKey}" /> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    protected Entity(TKey id) => this.Id = id;

    #region IEntity<TKey> Members

    /// <summary>
    ///     Gets or sets the id.
    /// </summary>
    /// <value>
    ///     The identifier.
    /// </value>
    public TKey Id { get; protected set; }

    /// <summary>
    ///     Get the keys.
    /// </summary>
    /// <returns>
    ///     An array of object?
    /// </returns>
    public virtual object?[] GetKeys() => [this.Id];

    #endregion

    #region IEquatable<Entity<TKey>> Members

    /// <summary>
    ///     Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns>
    ///     A bool
    /// </returns>
    public bool Equals(Entity<TKey>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetUnproxiedType(this) != GetUnproxiedType(other)) return false;
        if (this.IsTransient() || other.IsTransient()) return false;
        return EqualityComparer<TKey>.Default.Equals(this.Id, other.Id);
    }

    #endregion

    /// <summary>
    ///     Checks if is transient.
    /// </summary>
    /// <returns>
    ///     A bool
    /// </returns>
    public virtual bool IsTransient() => EqualityComparer<TKey>.Default.Equals(this.Id, default!);

    /// <summary>
    ///     Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns>
    ///     A bool
    /// </returns>
    public override bool Equals(object? obj) => obj is Entity<TKey> other && this.Equals(other);

    /// <summary>
    ///     Get hash code.
    /// </summary>
    /// <returns>
    ///     An int
    /// </returns>
    [SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
    public override int GetHashCode()
    {
        return this.IsTransient()
            ? base.GetHashCode()
            : HashCode.Combine(GetUnproxiedType(this), this.Id);
    }

    /// <summary>
    ///     Implements the == operator.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    ///     A bool
    /// </returns>
    public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right) => Equals(left, right);

    /// <summary>
    ///     Implements the != operator.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    ///     A bool
    /// </returns>
    public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right) => !Equals(left, right);

    /// <summary>
    ///     Gets the type of the unproxied.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    private static Type GetUnproxiedType(object obj)
    {
        Type type = obj.GetType();
        string typeString = type.ToString();
        return typeString.Contains("Castle.Proxies.", StringComparison.Ordinal) && type.BaseType is not null
            ? type.BaseType
            : type;
    }
}

/// <summary>
///     The aggregate root.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <seealso cref="Navyblue.Foundation.Domain.Entity&lt;TKey&gt;" />
/// <seealso cref="Navyblue.Foundation.Domain.IAggregateRoot&lt;TKey&gt;" />
/// <seealso cref="Navyblue.Foundation.Domain.IHasDomainEvents" />
/// <seealso cref="Navyblue.Foundation.Domain.IHasVersion" />
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>, IHasDomainEvents, IHasVersion
{
    /// <summary>
    ///     The domain events
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="AggregateRoot{TKey}" /> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    protected AggregateRoot(TKey id) : base(id)
    {
    }

    #region IHasDomainEvents Members

    /// <summary>
    ///     Gets the domain events.
    /// </summary>
    /// <value>
    ///     The domain events.
    /// </value>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => this._domainEvents.AsReadOnly();

    /// <summary>
    ///     Clear domain events.
    /// </summary>
    public void ClearDomainEvents() => this._domainEvents.Clear();

    #endregion

    #region IHasVersion Members

    /// <summary>
    ///     Gets the version.
    /// </summary>
    /// <value>
    ///     The version.
    /// </value>
    public long Version { get; private set; }

    #endregion

    /// <summary>
    ///     Adds the domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    /// <exception cref="ArgumentNullException"></exception>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        this._domainEvents.Add(domainEvent);
    }

    /// <summary>
    ///     Removes the domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    /// <exception cref="ArgumentNullException"></exception>
    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        this._domainEvents.Remove(domainEvent);
    }

    /// <summary>
    ///     Increments the version.
    /// </summary>
    protected void IncrementVersion() => this.Version++;
}

/// <summary>
///     The full audited entity.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <seealso cref="Navyblue.Foundation.Domain.Entity&lt;TKey&gt;" />
/// <seealso cref="Navyblue.Foundation.Domain.ICreationAudited" />
/// <seealso cref="Navyblue.Foundation.Domain.IModificationAudited" />
/// <seealso cref="Navyblue.Foundation.Domain.ISoftDelete" />
/// <seealso cref="Navyblue.Foundation.Domain.IHasConcurrencyStamp" />
public abstract class FullAuditedEntity<TKey> : Entity<TKey>, ICreationAudited, IModificationAudited, ISoftDelete, IHasConcurrencyStamp
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FullAuditedEntity{TKey}" /> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    protected FullAuditedEntity(TKey id) : base(id)
    {
    }

    #region ICreationAudited Members

    /// <summary>
    ///     Gets or sets the created at.
    /// </summary>
    /// <value>
    ///     The created at.
    /// </value>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the created by.
    /// </summary>
    /// <value>
    ///     The created by.
    /// </value>
    public string? CreatedBy { get; set; }

    #endregion

    #region IHasConcurrencyStamp Members

    /// <summary>
    ///     Gets or sets the concurrency stamp.
    /// </summary>
    /// <value>
    ///     The concurrency stamp.
    /// </value>
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString("N");

    #endregion

    #region IModificationAudited Members

    /// <summary>
    ///     Gets or sets the modified at.
    /// </summary>
    /// <value>
    ///     The modified at.
    /// </value>
    public DateTimeOffset? ModifiedAt { get; set; }

    /// <summary>
    ///     Gets or sets the modified by.
    /// </summary>
    /// <value>
    ///     The modified by.
    /// </value>
    public string? ModifiedBy { get; set; }

    #endregion

    #region ISoftDelete Members

    /// <summary>
    ///     Gets or sets a value indicating whether deleted.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
    /// </value>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the deleted at.
    /// </summary>
    /// <value>
    ///     The deleted at.
    /// </value>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    ///     Gets or sets the deleted by.
    /// </summary>
    /// <value>
    ///     The deleted by.
    /// </value>
    public string? DeletedBy { get; set; }

    #endregion
}

/// <summary>
///     The full audited aggregate root.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <seealso cref="Navyblue.Foundation.Domain.AggregateRoot&lt;TKey&gt;" />
/// <seealso cref="Navyblue.Foundation.Domain.ICreationAudited" />
/// <seealso cref="Navyblue.Foundation.Domain.IModificationAudited" />
/// <seealso cref="Navyblue.Foundation.Domain.ISoftDelete" />
/// <seealso cref="Navyblue.Foundation.Domain.IHasConcurrencyStamp" />
public abstract class FullAuditedAggregateRoot<TKey> : AggregateRoot<TKey>, ICreationAudited, IModificationAudited, ISoftDelete, IHasConcurrencyStamp
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FullAuditedAggregateRoot{TKey}" /> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    protected FullAuditedAggregateRoot(TKey id) : base(id)
    {
    }

    #region ICreationAudited Members

    /// <summary>
    ///     Gets or sets the created at.
    /// </summary>
    /// <value>
    ///     The created at.
    /// </value>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the created by.
    /// </summary>
    /// <value>
    ///     The created by.
    /// </value>
    public string? CreatedBy { get; set; }

    #endregion

    #region IHasConcurrencyStamp Members

    /// <summary>
    ///     Gets or sets the concurrency stamp.
    /// </summary>
    /// <value>
    ///     The concurrency stamp.
    /// </value>
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString("N");

    #endregion

    #region IModificationAudited Members

    /// <summary>
    ///     Gets or sets the modified at.
    /// </summary>
    /// <value>
    ///     The modified at.
    /// </value>
    public DateTimeOffset? ModifiedAt { get; set; }

    /// <summary>
    ///     Gets or sets the modified by.
    /// </summary>
    /// <value>
    ///     The modified by.
    /// </value>
    public string? ModifiedBy { get; set; }

    #endregion

    #region ISoftDelete Members

    /// <summary>
    ///     Gets or sets a value indicating whether deleted.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
    /// </value>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the deleted at.
    /// </summary>
    /// <value>
    ///     The deleted at.
    /// </value>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    ///     Gets or sets the deleted by.
    /// </summary>
    /// <value>
    ///     The deleted by.
    /// </value>
    public string? DeletedBy { get; set; }

    #endregion
}

/// <summary>
///     The domain event interface.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    ///     Gets the event identifier.
    /// </summary>
    /// <value>
    ///     The event identifier.
    /// </value>
    Guid EventId { get; }

    /// <summary>
    ///     Gets the occurred at.
    /// </summary>
    /// <value>
    ///     The occurred at.
    /// </value>
    DateTimeOffset OccurredAt { get; }
}

/// <summary>
///     The domain event.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Domain.IDomainEvent" />
public abstract record DomainEvent(Guid EventId, DateTimeOffset OccurredAt) : IDomainEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DomainEvent" /> class.
    /// </summary>
    protected DomainEvent() : this(Guid.NewGuid(), DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
///     The domain event envelope.
/// </summary>
public sealed record DomainEventEnvelope(IDomainEvent Event, string AggregateType, string? AggregateId, long? AggregateVersion, string? CorrelationId = null, string? CausationId = null);

/// <summary>
///     Has domain events interface.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    ///     Gets the domain events.
    /// </summary>
    /// <value>
    ///     The domain events.
    /// </value>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    ///     Clear domain events.
    /// </summary>
    void ClearDomainEvents();
}

/// <summary>
///     The domain event handler interface.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    ///     Handles the asynchronous.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A ValueTask
    /// </returns>
    ValueTask HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

/// <summary>
///     The domain event dispatcher interface.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    ///     Dispatches the asynchronous.
    /// </summary>
    /// <param name="domainEvents">The domain events.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A ValueTask
    /// </returns>
    ValueTask DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

/// <summary>
///     The framework exception.
/// </summary>
/// <seealso cref="System.Exception" />
public abstract class FrameworkException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FrameworkException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="code">The code.</param>
    /// <param name="innerException">The inner exception.</param>
    protected FrameworkException(string message, string code, Exception? innerException = null) : base(message, innerException) => this.Code = code;

    /// <summary>
    ///     Gets the code.
    /// </summary>
    /// <value>
    ///     The code.
    /// </value>
    public string Code { get; }

    /// <summary>
    ///     Gets the data items.
    /// </summary>
    /// <value>
    ///     The data items.
    /// </value>
    public IDictionary<string, object?> DataItems { get; } = new Dictionary<string, object?>();
}

/// <summary>
///     The business exception.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Domain.FrameworkException" />
/// <param name="message">The message.</param>
/// <param name="code">The code.</param>
/// <param name="innerException">The inner exception.</param>
public sealed class BusinessException(string message, string code = "business_error", Exception? innerException = null) : FrameworkException(message, code, innerException);

/// <summary>
///     The domain rule violation exception.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Domain.FrameworkException" />
/// <param name="message">The message.</param>
/// <param name="code">The code.</param>
public sealed class DomainRuleViolationException(string message, string code = "domain_rule_violation") : FrameworkException(message, code);

/// <summary>
///     The validation exception.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Domain.FrameworkException" />
/// <param name="message">The message.</param>
/// <param name="errors">The errors.</param>
public sealed class ValidationException(string message, IReadOnlyDictionary<string, string[]>? errors = null) : FrameworkException(message, "validation_error")
{
    /// <summary>
    ///     Gets the errors.
    /// </summary>
    /// <value>
    ///     The errors.
    /// </value>
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors ?? new Dictionary<string, string[]>();
}

/// <summary>
///     The not found exception.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Domain.FrameworkException" />
/// <param name="message">The message.</param>
/// <param name="code">The code.</param>
public sealed class NotFoundException(string message, string code = "not_found") : FrameworkException(message, code);

/// <summary>
///     The forbidden exception.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Domain.FrameworkException" />
/// <param name="message">The message.</param>
public sealed class ForbiddenException(string message = "Forbidden") : FrameworkException(message, "forbidden");

/// <summary>
///     The unauthorized exception.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Domain.FrameworkException" />
/// <param name="message">The message.</param>
public sealed class UnauthorizedException(string message = "Unauthorized") : FrameworkException(message, "unauthorized");

/// <summary>
///     The infrastructure exception.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Domain.FrameworkException" />
/// <param name="message">The message.</param>
/// <param name="code">The code.</param>
/// <param name="innerException">The inner exception.</param>
public sealed class InfrastructureException(string message, string code = "infrastructure_error", Exception? innerException = null) : FrameworkException(message, code, innerException);