// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Configuration.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="Configuration.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.ComponentModel.DataAnnotations;

namespace Navyblue.Foundation.Configuration;

/// <summary>
///     Redis adapter options (used by future Redis packages).
/// </summary>
public sealed class RedisOptions
{
    /// <summary>
    ///     The section name.
    /// </summary>
    public const string SectionName = "Navyblue:Redis";

    /// <summary>
    ///     Gets or sets the connection string.
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the instance name.
    /// </summary>
    public string InstanceName { get; set; } = "navyblue";
}

/// <summary>
///     Messaging adapter options (used by future messaging packages).
/// </summary>
public sealed class MessagingOptions
{
    /// <summary>
    ///     The section name.
    /// </summary>
    public const string SectionName = "Navyblue:Messaging";

    /// <summary>
    ///     Gets or sets the provider.
    /// </summary>
    public string Provider { get; set; } = "InMemory";

    /// <summary>
    ///     Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }
}

/// <summary>
///     Domain/application audit options (host-agnostic).
/// </summary>
public sealed class AuditOptions
{
    /// <summary>
    ///     The section name.
    /// </summary>
    public const string SectionName = "Navyblue:Audit";

    /// <summary>
    ///     Gets or sets a value indicating whether enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether save user name.
    /// </summary>
    public bool SaveUserName { get; set; } = true;
}

/// <summary>
///     Multi-tenant feature options (host-agnostic).
///     HTTP header name lives in <c>NavyblueAspNetCoreOptions.TenantHeaderName</c>.
/// </summary>
public sealed class MultiTenantOptions
{
    /// <summary>
    ///     The section name.
    /// </summary>
    public const string SectionName = "Navyblue:MultiTenant";

    /// <summary>
    ///     Gets or sets a value indicating whether multi-tenancy is enabled.
    /// </summary>
    public bool Enabled { get; set; }
}