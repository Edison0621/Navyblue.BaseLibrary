// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : TenantResolutionMiddleware.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="TenantResolutionMiddleware.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Http;

namespace Navyblue.BaseLibrary.AspNetCore;

/// <summary>
///     The tenant id accessor interface.
/// </summary>
public interface ITenantIdAccessor
{
    /// <summary>
    ///     Gets or sets the tenant identifier.
    /// </summary>
    /// <value>
    ///     The tenant identifier.
    /// </value>
    string? TenantId { get; set; }
}

/// <summary>
///     The tenant id accessor.
/// </summary>
/// <seealso cref="Navyblue.BaseLibrary.AspNetCore.ITenantIdAccessor" />
public sealed class TenantIdAccessor : ITenantIdAccessor
{
    /// <summary>
    ///     The current tenant
    /// </summary>
    private static readonly AsyncLocal<string?> _currentTenant = new();

    #region ITenantIdAccessor Members

    /// <summary>
    ///     Gets or sets the tenant id.
    /// </summary>
    /// <value>
    ///     The tenant identifier.
    /// </value>
    public string? TenantId
    {
        get => _currentTenant.Value;
        set => _currentTenant.Value = value;
    }

    #endregion
}

/// <summary>
///     The tenant resolution middleware.
/// </summary>
/// <param name="next">The next.</param>
/// <param name="options">The options.</param>
public sealed class TenantResolutionMiddleware(RequestDelegate next, NavyblueAspNetCoreOptions options)
{
    /// <summary>
    ///     Invokes the asynchronous.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="accessor">The accessor.</param>
    /// <returns>
    ///     A Task
    /// </returns>
    public async Task InvokeAsync(HttpContext context, ITenantIdAccessor accessor)
    {
        string? tenantId = context.Request.Headers[options.TenantHeaderName].FirstOrDefault()
                           ?? context.User.FindFirst("tenant_id")?.Value
                           ?? context.Request.Query["tenantId"].FirstOrDefault();

        accessor.TenantId = tenantId;
        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            context.Items[RequestContextItems.TenantId] = tenantId;
        }

        await next(context).ConfigureAwait(false);
    }
}