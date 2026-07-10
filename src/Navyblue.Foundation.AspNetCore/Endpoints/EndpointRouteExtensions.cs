// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : EndpointRouteExtensions.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="EndpointRouteExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Navyblue.Foundation.Application;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
/// </summary>
public sealed record ApplicationInfo(string Name, string Version, string Environment, DateTimeOffset ServerTime);

/// <summary>
/// </summary>
public static class EndpointRouteExtensions
{
    /// <summary>
    ///     Maps the navyblue information.
    /// </summary>
    /// <param name="endpoints">The endpoints.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="applicationName">Name of the application.</param>
    /// <param name="version">The version.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static IEndpointRouteBuilder MapNavyblueInfo(this IEndpointRouteBuilder endpoints, string pattern = "/_navyblue/info", string? applicationName = null, string? version = null)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        endpoints.MapGet(pattern, (HttpContext context) =>
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            ApplicationInfo info = new ApplicationInfo(applicationName ?? AppDomain.CurrentDomain.FriendlyName, version ?? typeof(EndpointRouteExtensions).Assembly.GetName().Version?.ToString() ?? "1.0.0", environment, DateTimeOffset.UtcNow);
            return NavyblueResults.Ok(info, traceId: context.GetTraceId());
        });
        return endpoints;
    }

    /// <summary>
    ///     Maps the navyblue ping.
    /// </summary>
    /// <param name="endpoints">The endpoints.</param>
    /// <param name="pattern">The pattern.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static IEndpointRouteBuilder MapNavybluePing(this IEndpointRouteBuilder endpoints, string pattern = "/_navyblue/ping")
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        endpoints.MapGet(pattern, (HttpContext context) => NavyblueResults.Ok(new { pong = true, time = DateTimeOffset.UtcNow }, traceId: context.GetTraceId()));
        return endpoints;
    }

    /// <summary>
    ///     Maps the navyblue get page.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="endpoints">The endpoints.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="handler">The handler.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static RouteHandlerBuilder MapNavyblueGetPage<T>(this IEndpointRouteBuilder endpoints, string pattern, Func<QueryRequest, CancellationToken, Task<PageResult<T>>> handler)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        ArgumentNullException.ThrowIfNull(handler);
        return endpoints.MapGet(pattern, async (HttpContext context, CancellationToken cancellationToken) =>
        {
            QueryRequest query = context.Request.GetQueryRequest();
            PageResult<T> page = await handler(query, cancellationToken).ConfigureAwait(false);
            return NavyblueResults.Ok(page, traceId: context.GetTraceId());
        });
    }
}