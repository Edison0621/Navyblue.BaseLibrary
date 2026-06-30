// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ApiKeyMiddleware.cs
// Created          : 2026-06-29  16:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:50
// ****************************************************************************************************************************************
// <copyright file="ApiKeyMiddleware.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Navyblue.BaseLibrary.AspNetCore;

/// <summary>
///     The api key options.
/// </summary>
public sealed class ApiKeyOptions
{
    /// <summary>
    ///     Gets or sets the header name.
    /// </summary>
    public string HeaderName { get; set; } = "X-Api-Key";

    /// <summary>
    ///     Gets the allowed keys.
    /// </summary>
    public ISet<string> AllowedKeys { get; } = new HashSet<string>(StringComparer.Ordinal);

    /// <summary>
    ///     Gets the excluded paths.
    /// </summary>
    public ISet<PathString> ExcludedPaths { get; } = new HashSet<PathString>();
}

/// <summary>
///     The api key middleware.
/// </summary>
/// <param name="next">The next.</param>
/// <param name="options">The options.</param>
public sealed class ApiKeyMiddleware(RequestDelegate next, ApiKeyOptions options)
{
    /// <summary>
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A Task</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (options.AllowedKeys.Count == 0 || options.ExcludedPaths.Any(p => context.Request.Path.StartsWithSegments(p)))
        {
            await next(context).ConfigureAwait(false);
            return;
        }

        string? key = context.Request.Headers[options.HeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(key) || !options.AllowedKeys.Contains(key))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid API key.").ConfigureAwait(false);
            return;
        }

        await next(context).ConfigureAwait(false);
    }
}

/// <summary>
///     The api key application builder extensions.
/// </summary>
public static class ApiKeyApplicationBuilderExtensions
{
    /// <summary>
    ///     Add navyblue api key.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configure">The configure.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IServiceCollection</returns>
    public static IServiceCollection AddNavyblueApiKey(this IServiceCollection services, Action<ApiKeyOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);
        ApiKeyOptions options = new ApiKeyOptions();
        configure(options);
        services.AddSingleton(options);
        return services;
    }

    /// <summary>
    ///     Use navyblue api key.
    /// </summary>
    /// <param name="app">The app.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IApplicationBuilder</returns>
    public static IApplicationBuilder UseNavyblueApiKey(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<ApiKeyMiddleware>();
    }
}