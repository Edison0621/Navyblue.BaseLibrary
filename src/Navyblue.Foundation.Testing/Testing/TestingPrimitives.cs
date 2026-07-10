// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : TestingPrimitives.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="TestingPrimitives.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Claims;
using Navyblue.Foundation.Diagnostics;
using Navyblue.Foundation.Domain;

namespace Navyblue.Foundation.Testing;

/// <summary>
///     Controllable clock for tests. Implements <see cref="IClock" />.
/// </summary>
public sealed class TestClock : IClock
{
    /// <summary>
    ///     Gets or sets the UTC now value used by the clock.
    /// </summary>
    public DateTimeOffset UtcNow { get; set; } = DateTimeOffset.UtcNow;

    #region IClock Members

    /// <inheritdoc />
    public DateTimeOffset Now => this.UtcNow.ToLocalTime();

    DateTimeOffset IClock.UtcNow => this.UtcNow;

    #endregion

    /// <summary>
    ///     Advances the clock by the specified duration.
    /// </summary>
    public TestClock Advance(TimeSpan duration)
    {
        this.UtcNow = this.UtcNow.Add(duration);
        return this;
    }

    /// <summary>
    ///     Sets the UTC now value.
    /// </summary>
    public TestClock SetUtcNow(DateTimeOffset utcNow)
    {
        this.UtcNow = utcNow;
        return this;
    }
}

/// <summary>
///     Factory for test <see cref="ClaimsPrincipal" /> instances.
/// </summary>
public static class TestClaimsPrincipal
{
    /// <summary>
    ///     Creates a claims principal with common Navyblue claim conventions.
    /// </summary>
    public static ClaimsPrincipal Create(
        string userId = "test-user",
        string userName = "Test User",
        string[]? roles = null,
        string? tenantId = null,
        string? merchantId = null,
        IEnumerable<Claim>? additionalClaims = null)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, userName),
            new("sub", userId)
        ];

        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            claims.Add(new Claim("tenant_id", tenantId));
        }

        if (!string.IsNullOrWhiteSpace(merchantId))
        {
            claims.Add(new Claim("merchant_id", merchantId));
        }

        claims.AddRange((roles ?? []).Select(role => new Claim(ClaimTypes.Role, role)));
        if (additionalClaims is not null)
        {
            claims.AddRange(additionalClaims);
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }
}

/// <summary>
///     Helper for setting <see cref="CorrelationContext" /> in tests.
/// </summary>
public static class CorrelationTestScope
{
    /// <summary>
    ///     Begins a correlation scope for the duration of the returned disposable.
    /// </summary>
    public static IDisposable Begin(string correlationId = "test-correlation") => CorrelationContext.BeginScope(correlationId);
}