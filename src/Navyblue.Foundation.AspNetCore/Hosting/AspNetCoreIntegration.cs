// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AspNetCoreIntegration.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="AspNetCoreIntegration.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Diagnostics;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     The navyblue asp net core options.
/// </summary>
public sealed class NavyblueAspNetCoreOptions
{
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
    ///     Gets or sets the trace header name.
    /// </summary>
    public string TraceHeaderName { get; set; } = "X-Trace-Id";

    /// <summary>
    ///     Gets or sets the tenant header name.
    /// </summary>
    public string TenantHeaderName { get; set; } = "X-Tenant-Id";

    /// <summary>
    ///     Gets or sets the client ip header name.
    /// </summary>
    public string ClientIpHeaderName { get; set; } = "X-Forwarded-For";

    /// <summary>
    ///     Gets or sets a value indicating whether security headers are enabled.
    /// </summary>
    public bool EnableSecurityHeaders { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether request context should be populated.
    /// </summary>
    public bool EnableRequestContext { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether tenant resolution is enabled.
    /// </summary>
    public bool EnableTenantResolution { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether audit logging is enabled.
    /// </summary>
    public bool EnableAuditLogging { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether MVC actions should auto-wrap results as <c>ApiResult</c>
    ///     when <c>AddControllers</c> / <c>AddMvc</c> is present. Requires calling
    ///     <see cref="NavyblueAspNetCoreExtensions.AddNavyblueFramework" /> after MVC registration,
    ///     or call <c>AddNavyblueApiResultWrapping()</c> explicitly.
    /// </summary>
    public bool WrapApiResult { get; set; }

    /// <summary>
    ///     When true (default with <see cref="EnableTraceId" />), uses CorrelationId middleware
    ///     (header read/write, logging scope, CorrelationContext) instead of the simple TraceId middleware.
    /// </summary>
    public bool EnableCorrelationId { get; set; } = true;

    /// <summary>
    ///     When true, missing correlation/trace header returns 400.
    /// </summary>
    public bool EnforceCorrelationHeader { get; set; }
}

/// <summary>
///     The http current user.
/// </summary>
/// <param name="accessor">The accessor.</param>
public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal Principal => accessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());

    #region ICurrentUser Members

    /// <summary>
    ///     Gets the user id.
    /// </summary>
    public string? UserId => this.FindClaimValue(ClaimTypes.NameIdentifier) ?? this.FindClaimValue("sub") ?? this.FindClaimValue("user_id");

    /// <summary>
    ///     Gets the user name.
    /// </summary>
    public string? UserName => this.Principal.Identity?.Name ?? this.FindClaimValue(ClaimTypes.Name);

    /// <summary>
    ///     Gets a value indicating whether authenticated.
    /// </summary>
    public bool IsAuthenticated => this.Principal.Identity?.IsAuthenticated == true;

    /// <summary>
    ///     Gets the roles.
    /// </summary>
    public IReadOnlyCollection<string> Roles => this.Principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();

    /// <summary>
    ///     Gets the claims.
    /// </summary>
    public IReadOnlyCollection<Claim> Claims => this.Principal.Claims.ToArray();

    /// <summary>
    ///     Checks if is in role.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <returns>A bool</returns>
    public bool IsInRole(string role) => this.Principal.IsInRole(role);

    /// <summary>
    ///     Find claim value.
    /// </summary>
    /// <param name="claimType">The claim type.</param>
    /// <returns>A string</returns>
    public string? FindClaimValue(string claimType) => this.Principal.FindFirst(claimType)?.Value;

    #endregion
}

/// <summary>
///     The http current tenant.
/// </summary>
/// <param name="accessor">The accessor.</param>
public sealed class HttpCurrentTenant(IHttpContextAccessor accessor) : ICurrentTenant
{
    #region ICurrentTenant Members

    /// <summary>
    ///     Gets the tenant id.
    /// </summary>
    public string? TenantId
    {
        get
        {
            HttpContext? context = accessor.HttpContext;
            if (context is null)
            {
                return null;
            }

            NavyblueAspNetCoreOptions options = context.RequestServices.GetService<NavyblueAspNetCoreOptions>() ?? new NavyblueAspNetCoreOptions();
            return context.Request.Headers[options.TenantHeaderName].FirstOrDefault()
                   ?? context.User.FindFirst("tenant_id")?.Value
                   ?? context.User.FindFirst("tenantId")?.Value;
        }
    }

    /// <summary>
    ///     Gets the tenant name.
    /// </summary>
    public string? TenantName => accessor.HttpContext?.User.FindFirst("tenant_name")?.Value;

    /// <summary>
    ///     Gets a value indicating whether available.
    /// </summary>
    public bool IsAvailable => !string.IsNullOrWhiteSpace(this.TenantId);

    #endregion
}

/// <summary>
///     The trace id middleware.
/// </summary>
/// <param name="next">The next.</param>
/// <param name="options">The options.</param>
public sealed class TraceIdMiddleware(RequestDelegate next, NavyblueAspNetCoreOptions options)
{
    /// <summary>
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A Task</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        string? traceId = context.Request.Headers[options.TraceHeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(traceId)) traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        context.Response.Headers[options.TraceHeaderName] = traceId;
        using (CorrelationContext.BeginScope(traceId)) await next(context).ConfigureAwait(false);
    }
}

/// <summary>
///     The request logging middleware.
/// </summary>
/// <param name="next">The next.</param>
/// <param name="logger">The logger.</param>
public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    /// <summary>
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A Task</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        OperationTimer timer = OperationTimer.StartNew();
        await next(context).ConfigureAwait(false);
        logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms TraceId={TraceId}", context.Request.Method, context.Request.Path, context.Response.StatusCode, timer.Elapsed.TotalMilliseconds, CorrelationContext.Current ?? context.TraceIdentifier);
    }
}

/// <summary>
///     The navyblue asp net core extensions.
/// </summary>
public static class NavyblueAspNetCoreExtensions
{
    /// <summary>
    ///     Add navyblue framework.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>An IServiceCollection</returns>
    public static IServiceCollection AddNavyblueFramework(this IServiceCollection services, Action<NavyblueAspNetCoreOptions>? configure = null)
    {
        NavyblueAspNetCoreOptions options = new();
        configure?.Invoke(options);
        services.AddSingleton(options);
        services.AddHttpContextAccessor();
        services.AddScoped<IHttpRequestContext, HttpRequestContext>();
        services.AddScoped<IExceptionResponseMapper, DefaultExceptionResponseMapper>();
        services.AddScoped<ITenantIdAccessor, TenantIdAccessor>();
        services.AddScoped<IAuditLogSink, LoggingAuditLogSink>();
        services.AddScoped<IPageMetadataAccessor, PageMetadataAccessor>();
        services.AddScoped<ICurrentUser, HttpCurrentUser>();
        services.AddScoped<ICurrentTenant, HttpCurrentTenant>();

        if (options.EnableTraceId || options.EnableCorrelationId)
        {
            services.AddNavyblueCorrelationId(cid =>
            {
                cid.RequestHeader = options.TraceHeaderName;
                cid.ResponseHeader = options.TraceHeaderName;
                cid.EnforceHeader = options.EnforceCorrelationHeader;
                cid.AdditionalRequestHeaders =
                [
                    "X-Correlation-ID", "X-Request-ID", "X-CorrelationId", "Correlation-Id", "Request-Id", "X-Trace-Id"
                ];
            });
            services.AddNavyblueCorrelationIdForwarding();
        }

        if (options.WrapApiResult)
        {
            services.Configure<MvcOptions>(mvc => mvc.Filters.Add<ApiResultWrappingFilter>());
        }

        return services;
    }

    /// <summary>
    ///     Use navyblue framework.
    /// </summary>
    /// <param name="app">The app.</param>
    /// <returns>An IApplicationBuilder</returns>
    public static IApplicationBuilder UseNavyblueFramework(this IApplicationBuilder app)
    {
        NavyblueAspNetCoreOptions options = app.ApplicationServices.GetRequiredService<NavyblueAspNetCoreOptions>();
        if (options.EnableExceptionHandling) app.UseExceptionHandler(errorApp => errorApp.Run(WriteErrorAsync));
        if (options.EnableTraceId || options.EnableCorrelationId)
            app.UseNavyblueCorrelationId();
        if (options.EnableTenantResolution) app.UseMiddleware<TenantResolutionMiddleware>();
        if (options.EnableRequestContext) app.UseMiddleware<RequestContextMiddleware>();
        if (options.EnableSecurityHeaders) app.UseMiddleware<SecurityHeadersMiddleware>();
        if (options.EnableRequestLogging) app.UseMiddleware<RequestLoggingMiddleware>();
        if (options.EnableAuditLogging) app.UseMiddleware<AuditLoggingMiddleware>();
        return app;
    }

    private static async Task WriteErrorAsync(HttpContext context)
    {
        Exception? exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        IExceptionResponseMapper mapper = context.RequestServices.GetRequiredService<IExceptionResponseMapper>();
        ExceptionResponse response = mapper.Map(exception, context);
        context.Response.StatusCode = response.StatusCode;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsJsonAsync(response.Result).ConfigureAwait(false);
    }
}