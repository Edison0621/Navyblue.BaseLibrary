// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : HttpRequestContext.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="HttpRequestContext.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Navyblue.BaseLibrary.Diagnostics;

namespace Navyblue.BaseLibrary.AspNetCore;

/// <summary>
///     The http request context interface.
/// </summary>
public interface IHttpRequestContext
{
    /// <summary>
    ///     Gets the client ip.
    /// </summary>
    /// <value>
    ///     The client ip.
    /// </value>
    string? ClientIp { get; }

    /// <summary>
    ///     Gets the correlation identifier.
    /// </summary>
    /// <value>
    ///     The correlation identifier.
    /// </value>
    string? CorrelationId { get; }

    /// <summary>
    ///     Gets a value indicating whether this instance is authenticated.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
    /// </value>
    bool IsAuthenticated { get; }

    /// <summary>
    ///     Gets the method.
    /// </summary>
    /// <value>
    ///     The method.
    /// </value>
    string Method { get; }

    /// <summary>
    ///     Gets the path.
    /// </summary>
    /// <value>
    ///     The path.
    /// </value>
    PathString Path { get; }

    /// <summary>
    ///     Gets the tenant identifier.
    /// </summary>
    /// <value>
    ///     The tenant identifier.
    /// </value>
    string? TenantId { get; }

    /// <summary>
    ///     Gets the trace identifier.
    /// </summary>
    /// <value>
    ///     The trace identifier.
    /// </value>
    string TraceId { get; }

    /// <summary>
    ///     Gets the user agent.
    /// </summary>
    /// <value>
    ///     The user agent.
    /// </value>
    string? UserAgent { get; }

    /// <summary>
    ///     Gets the user identifier.
    /// </summary>
    /// <value>
    ///     The user identifier.
    /// </value>
    string? UserId { get; }

    /// <summary>
    ///     Gets the name of the user.
    /// </summary>
    /// <value>
    ///     The name of the user.
    /// </value>
    string? UserName { get; }
}

/// <summary>
///     The http request context.
/// </summary>
/// <seealso cref="Navyblue.BaseLibrary.AspNetCore.IHttpRequestContext" />
/// <param name="accessor">The accessor.</param>
/// <param name="options">The options.</param>
public sealed class HttpRequestContext(IHttpContextAccessor accessor, NavyblueAspNetCoreOptions options) : IHttpRequestContext
{
    /// <summary>
    ///     Gets the context.
    /// </summary>
    /// <value>
    ///     The context.
    /// </value>
    private HttpContext? Context => accessor.HttpContext;

    #region IHttpRequestContext Members

    /// <summary>
    ///     Gets the client ip.
    /// </summary>
    /// <value>
    ///     The client ip.
    /// </value>
    public string? ClientIp => this.Context.GetClientIp(options.ClientIpHeaderName);

    /// <summary>
    ///     Gets the correlation id.
    /// </summary>
    /// <value>
    ///     The correlation identifier.
    /// </value>
    public string? CorrelationId => this.Context?.Request.Headers[options.TraceHeaderName].FirstOrDefault() ?? this.TraceId;

    /// <summary>
    ///     Gets a value indicating whether authenticated.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
    /// </value>
    public bool IsAuthenticated => this.Context?.User.Identity?.IsAuthenticated == true;

    /// <summary>
    ///     Gets the method.
    /// </summary>
    /// <value>
    ///     The method.
    /// </value>
    public string Method => this.Context?.Request.Method ?? string.Empty;

    /// <summary>
    ///     Gets the path.
    /// </summary>
    /// <value>
    ///     The path.
    /// </value>
    public PathString Path => this.Context?.Request.Path ?? PathString.Empty;

    /// <summary>
    ///     Gets the tenant id.
    /// </summary>
    /// <value>
    ///     The tenant identifier.
    /// </value>
    public string? TenantId => this.Context?.Request.Headers[options.TenantHeaderName].FirstOrDefault() ?? this.Context?.User.FindFirst("tenant_id")?.Value;

    /// <summary>
    ///     Gets the trace id.
    /// </summary>
    /// <value>
    ///     The trace identifier.
    /// </value>
    public string TraceId => CorrelationContext.Current ?? this.Context?.TraceIdentifier ?? Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");

    /// <summary>
    ///     Gets the user agent.
    /// </summary>
    /// <value>
    ///     The user agent.
    /// </value>
    public string? UserAgent => this.Context?.Request.Headers.UserAgent.FirstOrDefault();

    /// <summary>
    ///     Gets the user id.
    /// </summary>
    /// <value>
    ///     The user identifier.
    /// </value>
    public string? UserId => this.Context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? this.Context?.User.FindFirst("sub")?.Value ?? this.Context?.User.FindFirst("user_id")?.Value;

    /// <summary>
    ///     Gets the user name.
    /// </summary>
    /// <value>
    ///     The name of the user.
    /// </value>
    public string? UserName => this.Context?.User.Identity?.Name ?? this.Context?.User.FindFirst(ClaimTypes.Name)?.Value;

    #endregion
}

/// <summary>
///     The request context middleware.
/// </summary>
/// <param name="next">The next.</param>
/// <param name="logger">The logger.</param>
public sealed class RequestContextMiddleware(RequestDelegate next, ILogger<RequestContextMiddleware> logger)
{
    /// <summary>
    ///     Invokes the asynchronous.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="requestContext">The request context.</param>
    /// <returns>
    ///     A Task
    /// </returns>
    public async Task InvokeAsync(HttpContext context, IHttpRequestContext requestContext)
    {
        context.Items[RequestContextItems.TraceId] = requestContext.TraceId;
        context.Items[RequestContextItems.TenantId] = requestContext.TenantId;
        context.Items[RequestContextItems.UserId] = requestContext.UserId;
        context.Items[RequestContextItems.ClientIp] = requestContext.ClientIp;

        using (logger.BeginScope(new Dictionary<string, object?>
               {
                   ["TraceId"] = requestContext.TraceId,
                   ["TenantId"] = requestContext.TenantId,
                   ["UserId"] = requestContext.UserId,
                   ["ClientIp"] = requestContext.ClientIp
               }))
        {
            await next(context).ConfigureAwait(false);
        }
    }
}

/// <summary>
///     The request context items.
/// </summary>
public static class RequestContextItems
{
    /// <summary>
    ///     The client ip.
    /// </summary>
    public const string ClientIp = "Navyblue.ClientIp";

    /// <summary>
    ///     The tenant id.
    /// </summary>
    public const string TenantId = "Navyblue.TenantId";

    /// <summary>
    ///     Trace id.
    /// </summary>
    public const string TraceId = "Navyblue.TraceId";

    /// <summary>
    ///     The user id.
    /// </summary>
    public const string UserId = "Navyblue.UserId";
}