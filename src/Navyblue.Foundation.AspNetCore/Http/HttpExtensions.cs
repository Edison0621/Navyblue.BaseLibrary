// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : HttpExtensions.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="HttpExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     The http request extensions.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    ///     Checks if is ajax request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A bool</returns>
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return string.Equals(request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Get display url without query string.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string GetDisplayUrlWithoutQueryString(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}";
    }

    /// <summary>
    ///     Accepts json.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A bool</returns>
    public static bool AcceptsJson(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return request.Headers.Accept.Any(x => x?.Contains("json", StringComparison.OrdinalIgnoreCase) == true);
    }

    /// <summary>
    ///     Get header value.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="name">The name.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <returns>A string</returns>
    public static string? GetHeaderValue(this HttpRequest request, string name)
    {
        ArgumentNullException.ThrowIfNull(request);
        Guard.NotNullOrWhiteSpace(name, nameof(name));
        return request.Headers.TryGetValue(name, out StringValues value) ? value.FirstOrDefault() : null;
    }
}

/// <summary>
///     The http context client extensions.
/// </summary>
public static class HttpContextClientExtensions
{
    /// <summary>
    ///     Get client ip.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="forwardedForHeaderName">The forwarded for header name.</param>
    /// <returns>A string</returns>
    public static string? GetClientIp(this HttpContext? context, string forwardedForHeaderName = "X-Forwarded-For")
    {
        if (context is null)
        {
            return null;
        }

        StringValues forwarded = context.Request.Headers[forwardedForHeaderName];
        string? firstForwarded = forwarded.FirstOrDefault()?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return !string.IsNullOrWhiteSpace(firstForwarded)
            ? firstForwarded
            : context.Connection.RemoteIpAddress?.ToString();
    }

    /// <summary>
    ///     Checks if is local request.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A bool</returns>
    public static bool IsLocalRequest(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        IPAddress? remote = context.Connection.RemoteIpAddress;
        IPAddress? local = context.Connection.LocalIpAddress;
        return remote is null || local is null || Equals(remote, local) || IPAddress.IsLoopback(remote);
    }

    /// <summary>
    ///     Get request item.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="key">The key.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string? GetRequestItem(this HttpContext context, string key)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Items.TryGetValue(key, out object? value) ? value?.ToString() : null;
    }
}

/// <summary>
///     The http response extensions.
/// </summary>
public static class HttpResponseExtensions
{
    /// <summary>
    ///     Writes api result asynchronously.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="result">The result.</param>
    /// <param name="statusCode">The status code.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A Task</returns>
    public static Task WriteApiResultAsync(this HttpResponse response, ApiResult result, int? statusCode = null, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(result);
        if (statusCode.HasValue)
        {
            response.StatusCode = statusCode.Value;
        }

        response.ContentType = "application/json; charset=utf-8";
        return response.WriteAsJsonAsync(result, options, cancellationToken);
    }
}