// ReSharper disable All
// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ProblemDetailsExtensions.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="ProblemDetailsExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Navyblue.Foundation.Application;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     The problem details extensions.
/// </summary>
public static class ProblemDetailsExtensions
{
#if NET7_0_OR_GREATER
    /// <summary>
    ///     Add navyblue problem details.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IServiceCollection</returns>
    public static IServiceCollection AddNavyblueProblemDetails(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.GetTraceId();
                string? tenantId = context.HttpContext.GetTenantId();
                if (!string.IsNullOrWhiteSpace(tenantId))
                {
                    context.ProblemDetails.Extensions["tenantId"] = tenantId;
                }
            };
        });

        return services;
    }
#endif

    /// <summary>
    ///     Use navyblue status code pages.
    /// </summary>
    /// <param name="app">The app.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IApplicationBuilder</returns>
    public static IApplicationBuilder UseNavyblueStatusCodePages(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseStatusCodePages(async statusCodeContext =>
        {
            HttpContext httpContext = statusCodeContext.HttpContext;
            if (httpContext.Response.HasStarted || httpContext.Response.ContentLength.HasValue)
            {
                return;
            }

            int statusCode = httpContext.Response.StatusCode;
            if (statusCode < 400)
            {
                return;
            }

            BusinessCode code = statusCode switch
            {
                StatusCodes.Status401Unauthorized => BusinessCode.Unauthorized,
                StatusCodes.Status403Forbidden => BusinessCode.Forbidden,
                StatusCodes.Status404NotFound => BusinessCode.NotFound,
                _ => BusinessCode.UnexpectedError
            };

            await httpContext.Response.WriteApiResultAsync(
                ApiResult.Fail(code, ReasonPhrases.GetReasonPhrase(statusCode), httpContext.GetTraceId()),
                statusCode).ConfigureAwait(false);
        });
    }
}

internal static class ReasonPhrases
{
    public static string GetReasonPhrase(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "Bad Request",
        StatusCodes.Status401Unauthorized => "Unauthorized",
        StatusCodes.Status403Forbidden => "Forbidden",
        StatusCodes.Status404NotFound => "Not Found",
        StatusCodes.Status409Conflict => "Conflict",
        StatusCodes.Status429TooManyRequests => "Too Many Requests",
        StatusCodes.Status500InternalServerError => "Internal Server Error",
        _ => "HTTP Error"
    };
}