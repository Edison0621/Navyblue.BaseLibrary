// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : RuntimeServices.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-29  13:06
// ****************************************************************************************************************************************
// <copyright file="RuntimeServices.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Domain;

/// <summary>
///     The clock interface.
/// </summary>
public interface IClock
{
    /// <summary>
    ///     Gets the now.
    /// </summary>
    /// <value>
    ///     The now.
    /// </value>
    DateTimeOffset Now { get; }

    /// <summary>
    ///     Gets the UTC now.
    /// </summary>
    /// <value>
    ///     The UTC now.
    /// </value>
    DateTimeOffset UtcNow { get; }
}

/// <summary>
///     The system clock.
/// </summary>
/// <seealso cref="Navyblue.BaseLibrary.Domain.IClock" />
public sealed class SystemClock : IClock
{
    #region IClock Members

    /// <summary>
    ///     Gets the now.
    /// </summary>
    /// <value>
    ///     The now.
    /// </value>
    public DateTimeOffset Now => DateTimeOffset.Now;

    /// <summary>
    ///     Gets the utc now.
    /// </summary>
    /// <value>
    ///     The UTC now.
    /// </value>
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    #endregion
}

/// <summary>
///     The auditor interface.
/// </summary>
public interface IAuditor
{
    /// <summary>
    ///     Gets the current user identifier.
    /// </summary>
    /// <value>
    ///     The current user identifier.
    /// </value>
    string? CurrentUserId { get; }

    /// <summary>
    ///     Gets the name of the current user.
    /// </summary>
    /// <value>
    ///     The name of the current user.
    /// </value>
    string? CurrentUserName { get; }
}

/// <summary>
///     The tenant resolver interface.
/// </summary>
public interface ITenantResolver
{
    /// <summary>
    ///     Gets the current tenant identifier.
    /// </summary>
    /// <value>
    ///     The current tenant identifier.
    /// </value>
    string? CurrentTenantId { get; }
}

/// <summary>
///     The audit property setter interface.
/// </summary>
public interface IAuditPropertySetter
{
    /// <summary>
    ///     Set creation audit.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void SetCreationAudit(object entity);

    /// <summary>
    ///     Set modification audit.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void SetModificationAudit(object entity);

    /// <summary>
    ///     Set deletion audit.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void SetDeletionAudit(object entity);
}