// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ServiceCollectionExtensions.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="ServiceCollectionExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     The navyblue cors options.
/// </summary>
public sealed class NavyblueCorsOptions
{
    /// <summary>
    ///     Gets or sets the policy name.
    /// </summary>
    public string PolicyName { get; set; } = "NavyblueCors";

    /// <summary>
    ///     Gets or sets the origins.
    /// </summary>
    public string[] Origins { get; set; } = [];

    /// <summary>
    ///     Gets or sets the headers.
    /// </summary>
    public string[] Headers { get; set; } = ["*"];

    /// <summary>
    ///     Gets or sets the methods.
    /// </summary>
    public string[] Methods { get; set; } = ["*"];

    /// <summary>
    ///     Gets or sets a value indicating whether allow credentials.
    /// </summary>
    public bool AllowCredentials { get; set; }
}

/// <summary>
///     The navyblue service collection extensions.
/// </summary>
public static class NavyblueServiceCollectionExtensions
{
    /// <summary>
    ///     Add navyblue cors.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configure">The configure.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IServiceCollection</returns>
    public static IServiceCollection AddNavyblueCors(this IServiceCollection services, Action<NavyblueCorsOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        NavyblueCorsOptions options = new NavyblueCorsOptions();
        configure?.Invoke(options);

        services.AddCors(cors =>
        {
            cors.AddPolicy(options.PolicyName, builder =>
            {
                if (options.Origins.Length > 0)
                {
                    builder.WithOrigins(options.Origins);
                }
                else
                {
                    builder.AllowAnyOrigin();
                }

                if (options.Headers is ["*"])
                {
                    builder.AllowAnyHeader();
                }
                else
                {
                    builder.WithHeaders(options.Headers);
                }

                if (options.Methods is ["*"])
                {
                    builder.AllowAnyMethod();
                }
                else
                {
                    builder.WithMethods(options.Methods);
                }

                if (options.AllowCredentials)
                {
                    builder.AllowCredentials();
                }
            });
        });

        return services;
    }

    /// <summary>
    ///     Add navyblue health checks.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IServiceCollection</returns>
    public static IServiceCollection AddNavyblueHealthChecks(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddHealthChecks();
        return services;
    }
}

/// <summary>
///     The navyblue application builder extensions.
/// </summary>
public static class NavyblueApplicationBuilderExtensions
{
    /// <summary>
    ///     Use navyblue cors.
    /// </summary>
    /// <param name="app">The app.</param>
    /// <param name="policyName">The policy name.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IApplicationBuilder</returns>
    public static IApplicationBuilder UseNavyblueCors(this IApplicationBuilder app, string policyName = "NavyblueCors")
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseCors(policyName);
    }
}