// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : DomainModelExtensions.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="DomainModelExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Domain;

/// <summary>
///     The domain model extensions.
/// </summary>
public static class DomainModelExtensions
{
    /// <summary>
    ///     Has domain events.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>A bool</returns>
    public static bool HasDomainEvents(this object? entity)
    {
        return entity is IHasDomainEvents source && source.DomainEvents.Count > 0;
    }

    /// <summary>
    ///     Get domain events.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns><![CDATA[IReadOnlyCollection<IDomainEvent>]]></returns>
    public static IReadOnlyCollection<IDomainEvent> GetDomainEvents(this object? entity)
    {
        return entity is IHasDomainEvents source ? source.DomainEvents : [];
    }

    /// <summary>
    ///     Checks if is not deleted.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void EnsureNotDeleted(this ISoftDelete entity, string message = "Entity has been deleted.")
    {
        ArgumentNullException.ThrowIfNull(entity);
        CheckRule.Against(entity.IsDeleted, message, "entity_deleted");
    }

    /// <summary>
    ///     Mark as deleted.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="deletedBy">The deleted by.</param>
    /// <param name="deletedAt">The deleted at.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void MarkAsDeleted(this ISoftDelete entity, string? deletedBy = null, DateTimeOffset? deletedAt = null)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = true;
        entity.DeletedBy = deletedBy;
        entity.DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Restore(this ISoftDelete entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity.IsDeleted = false;
        entity.DeletedBy = null;
        entity.DeletedAt = null;
    }
}