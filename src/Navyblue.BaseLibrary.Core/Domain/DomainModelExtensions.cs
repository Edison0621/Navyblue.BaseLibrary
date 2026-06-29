namespace Navyblue.BaseLibrary.Domain;

public static class DomainModelExtensions
{
    public static bool HasDomainEvents(this object? entity)
    {
        return entity is IHasDomainEvents source && source.DomainEvents.Count > 0;
    }

    public static IReadOnlyCollection<IDomainEvent> GetDomainEvents(this object? entity)
    {
        return entity is IHasDomainEvents source ? source.DomainEvents : [];
    }

    public static void EnsureNotDeleted(this ISoftDelete entity, string message = "Entity has been deleted.")
    {
        ArgumentNullException.ThrowIfNull(entity);
        CheckRule.Against(entity.IsDeleted, message, "entity_deleted");
    }

    public static void MarkAsDeleted(this ISoftDelete entity, string? deletedBy = null, DateTimeOffset? deletedAt = null)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = true;
        entity.DeletedBy = deletedBy;
        entity.DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }

    public static void Restore(this ISoftDelete entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = false;
        entity.DeletedBy = null;
        entity.DeletedAt = null;
    }
}
