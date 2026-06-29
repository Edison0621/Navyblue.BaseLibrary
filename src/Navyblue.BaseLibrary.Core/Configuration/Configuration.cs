// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Configuration.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-29  13:02
// ****************************************************************************************************************************************
// <copyright file="Configuration.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.ComponentModel.DataAnnotations;

namespace Navyblue.BaseLibrary.Configuration;

/// <summary>
///     The framework options.
/// </summary>
public sealed class FrameworkOptions
{
    /// <summary>
    ///     The section name.
    /// </summary>
    public const string SectionName = "Navyblue";

    /// <summary>
    ///     Gets or sets  a value indicating whether to enable exception handling.
    /// </summary>
    public bool EnableExceptionHandling { get; set; } = true;

    /// <summary>
    ///     Gets or sets  a value indicating whether to enable trace id.
    /// </summary>
    public bool EnableTraceId { get; set; } = true;

    /// <summary>
    ///     Gets or sets  a value indicating whether to enable request logging.
    /// </summary>
    public bool EnableRequestLogging { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether wrap api result.
    /// </summary>
    public bool WrapApiResult { get; set; }
}

/// <summary>
///     The redis options.
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
///     The messaging options.
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
///     The audit options.
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
///     The multi tenant options.
/// </summary>
public sealed class MultiTenantOptions
{
    /// <summary>
    ///     The section name.
    /// </summary>
    public const string SectionName = "Navyblue:MultiTenant";

    /// <summary>
    ///     Gets or sets a value indicating whether enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    ///     Gets or sets the header name.
    /// </summary>
    public string HeaderName { get; set; } = "X-Tenant-Id";
}

/// <summary>
///     The id generator options.
/// </summary>
public sealed class IdGeneratorOptions
{
    /// <summary>
    ///     The section name.
    /// </summary>
    public const string SectionName = "Navyblue:IdGenerator";

    /// <summary>
    ///     Gets or sets the worker id.
    /// </summary>
    [Range(0, 31)]
    public int WorkerId { get; set; }

    /// <summary>
    ///     Gets or sets the data center id.
    /// </summary>
    [Range(0, 31)]
    public int DataCenterId { get; set; }
}