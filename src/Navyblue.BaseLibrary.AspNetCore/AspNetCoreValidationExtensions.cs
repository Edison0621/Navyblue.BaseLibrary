// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AspNetCoreValidationExtensions.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:50
// ****************************************************************************************************************************************
// <copyright file="AspNetCoreValidationExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Navyblue.BaseLibrary.Application;

namespace Navyblue.BaseLibrary.AspNetCore;

/// <summary>
///     The asp net core validation extensions.
/// </summary>
public static class AspNetCoreValidationExtensions
{
    /// <summary>
    ///     Add navyblue api behavior.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IMvcBuilder</returns>
    public static IMvcBuilder AddNavyblueApiBehavior(this IMvcBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                Dictionary<string, string[]> errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid value." : e.ErrorMessage).ToArray());

                string traceId = context.HttpContext.TraceIdentifier;
                ApiResult result = ApiResult.Fail(
                    BusinessCode.ValidationError,
                    "Request validation failed.",
                    traceId,
                    new ErrorInfo(BusinessCode.ValidationError.ToString(), "Request validation failed.", errors));

                return new BadRequestObjectResult(result);
            };
        });

        return builder;
    }
}

/// <summary>
///     The http context extensions.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    ///     Get trace id.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string GetTraceId(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Response.Headers.TryGetValue("X-Trace-Id", out StringValues value) && !string.IsNullOrWhiteSpace(value)
            ? value.ToString()
            : context.TraceIdentifier;
    }

    /// <summary>
    ///     Get tenant id.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="headerName">The header name.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string? GetTenantId(this HttpContext context, string headerName = "X-Tenant-Id")
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Request.Headers[headerName].FirstOrDefault() ?? context.User.FindFirst("tenant_id")?.Value;
    }
}