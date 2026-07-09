// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AspNetCoreTesting.cs
// Created          : 2026-07-09  16:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  16:06
// ****************************************************************************************************************************************
// <copyright file="AspNetCoreTesting.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Navyblue.Foundation.AspNetCore;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.Testing;

/// <summary>
///     In-memory <see cref="IAuditLogSink" /> that records audit entries for assertions.
/// </summary>
public sealed class InMemoryAuditLogSink : IAuditLogSink
{
    private readonly ConcurrentQueue<AuditLogEntry> _entries = new();

    /// <summary>
    ///     Gets recorded audit entries.
    /// </summary>
    public IReadOnlyList<AuditLogEntry> Entries => this._entries.ToArray();

    /// <inheritdoc />
    public ValueTask WriteAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);
        this._entries.Enqueue(entry);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Clears recorded entries.
    /// </summary>
    public void Clear()
    {
        while (this._entries.TryDequeue(out _))
        {
        }
    }
}

/// <summary>
///     Mutable fake <see cref="IHttpRequestContext" /> for unit tests.
/// </summary>
public sealed class FakeHttpRequestContext : IHttpRequestContext
{
    /// <inheritdoc />
    public string? ClientIp { get; set; } = "127.0.0.1";

    /// <inheritdoc />
    public string? CorrelationId { get; set; } = "test-correlation";

    /// <inheritdoc />
    public bool IsAuthenticated { get; set; } = true;

    /// <inheritdoc />
    public string Method { get; set; } = "GET";

    /// <inheritdoc />
    public PathString Path { get; set; } = "/";

    /// <inheritdoc />
    public string? TenantId { get; set; } = "test-tenant";

    /// <inheritdoc />
    public string TraceId { get; set; } = "test-trace";

    /// <inheritdoc />
    public string? UserAgent { get; set; } = "Navyblue.Test";

    /// <inheritdoc />
    public string? UserId { get; set; } = "test-user";

    /// <inheritdoc />
    public string? UserName { get; set; } = "Test User";
}

/// <summary>
///     Mutable fake <see cref="ITenantIdAccessor" /> for tests.
/// </summary>
public sealed class FakeTenantIdAccessor : ITenantIdAccessor
{
    /// <inheritdoc />
    public string? TenantId { get; set; } = "test-tenant";
}

/// <summary>
///     Helpers for building <see cref="DefaultHttpContext" /> in tests.
/// </summary>
public static class HttpContextTestHelper
{
    /// <summary>
    ///     Creates an HTTP context with optional user, tenant header, and correlation header.
    /// </summary>
    public static DefaultHttpContext Create(
        ClaimsPrincipal? user = null,
        string? tenantId = null,
        string? correlationId = null,
        string method = "GET",
        string path = "/",
        string? tenantHeaderName = null,
        string? traceHeaderName = null)
    {
        DefaultHttpContext context = new()
        {
            User = user ?? new ClaimsPrincipal(new ClaimsIdentity()),
            Request =
            {
                Method = method,
                Path = path
            }
        };

        NavyblueAspNetCoreOptions options = new();
        string tenantHeader = tenantHeaderName ?? options.TenantHeaderName;
        string traceHeader = traceHeaderName ?? options.TraceHeaderName;

        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            context.Request.Headers[tenantHeader] = tenantId;
        }

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            context.Request.Headers[traceHeader] = correlationId;
        }

        return context;
    }

    /// <summary>
    ///     Creates an authenticated HTTP context from <see cref="TestClaimsPrincipal" /> defaults.
    /// </summary>
    public static DefaultHttpContext CreateAuthenticated(
        string userId = "test-user",
        string userName = "Test User",
        string[]? roles = null,
        string? tenantId = "test-tenant",
        string? merchantId = null)
    {
        ClaimsPrincipal principal = TestClaimsPrincipal.Create(userId, userName, roles, tenantId, merchantId);
        return Create(principal, tenantId);
    }
}

/// <summary>
///     Test helpers for JWT issuance using a deterministic signing key.
/// </summary>
public static class JwtTestHelper
{
    /// <summary>
    ///     Default HMAC signing key for tests (32+ characters).
    /// </summary>
    public const string DefaultSigningKey = "Navyblue.Foundation.Testing.JwtSigningKey.32+";

    /// <summary>
    ///     Creates <see cref="JwtOptions" /> suitable for unit tests.
    /// </summary>
    public static JwtOptions CreateOptions(
        string? signingKey = null,
        string issuer = "Navyblue.Test",
        string audience = "Navyblue.Test.Api",
        TimeSpan? expire = null)
    {
        return new JwtOptions
        {
            SigningKey = signingKey ?? DefaultSigningKey,
            Issuer = issuer,
            Audience = audience,
            Expire = expire ?? TimeSpan.FromHours(1),
            RequireHttpsMetadata = false,
            MapInboundClaims = false
        };
    }

    /// <summary>
    ///     Creates a real <see cref="IJwtTokenService" /> backed by test options.
    /// </summary>
    public static IJwtTokenService CreateTokenService(Action<JwtOptions>? configure = null)
    {
        JwtOptions options = CreateOptions();
        configure?.Invoke(options);
        Guard.NotNullOrWhiteSpace(options.SigningKey, nameof(options.SigningKey));
        return new JwtTokenService(options);
    }
}
