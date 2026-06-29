using System.ComponentModel.DataAnnotations;

namespace Navyblue.BaseLibrary.Configuration;

public sealed class FrameworkOptions { public const string SectionName = "Navyblue"; public bool EnableExceptionHandling { get; set; } = true; public bool EnableTraceId { get; set; } = true; public bool EnableRequestLogging { get; set; } = true; public bool WrapApiResult { get; set; } }
public sealed class RedisOptions { public const string SectionName = "Navyblue:Redis"; [Required] public string ConnectionString { get; set; } = string.Empty; public string InstanceName { get; set; } = "navyblue"; }
public sealed class MessagingOptions { public const string SectionName = "Navyblue:Messaging"; public string Provider { get; set; } = "InMemory"; public string? ConnectionString { get; set; } }
public sealed class AuditOptions { public const string SectionName = "Navyblue:Audit"; public bool Enabled { get; set; } = true; public bool SaveUserName { get; set; } = true; }
public sealed class MultiTenantOptions { public const string SectionName = "Navyblue:MultiTenant"; public bool Enabled { get; set; } public string HeaderName { get; set; } = "X-Tenant-Id"; }
public sealed class IdGeneratorOptions { public const string SectionName = "Navyblue:IdGenerator"; [Range(0, 31)] public int WorkerId { get; set; } [Range(0, 31)] public int DataCenterId { get; set; } }
