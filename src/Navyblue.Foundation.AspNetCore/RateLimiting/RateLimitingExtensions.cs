// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : RateLimitingExtensions.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="RateLimitingExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#if NET7_0_OR_GREATER
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     The navyblue rate limit options.
/// </summary>
public sealed class NavyblueRateLimitOptions
{
    /// <summary>
    ///     Gets or sets the policy name.
    /// </summary>
    public string PolicyName { get; set; } = "NavyblueFixedWindow";

    /// <summary>
    ///     Gets or sets the permit limit.
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    ///     Gets or sets the queue limit.
    /// </summary>
    public int QueueLimit { get; set; } = 0;

    /// <summary>
    ///     Gets or sets the window.
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
}

/// <summary>
///     The rate limiting extensions.
/// </summary>
public static class RateLimitingExtensions
{
    /// <summary>
    ///     Add navyblue rate limiting.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configure">The configure.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IServiceCollection</returns>
    public static IServiceCollection AddNavyblueRateLimiting(this IServiceCollection services, Action<NavyblueRateLimitOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        NavyblueRateLimitOptions options = new NavyblueRateLimitOptions();
        configure?.Invoke(options);

        services.AddRateLimiter(rateLimiter =>
        {
            rateLimiter.AddFixedWindowLimiter(options.PolicyName, limiter =>
            {
                limiter.PermitLimit = options.PermitLimit;
                limiter.QueueLimit = options.QueueLimit;
                limiter.Window = options.Window;
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });

        return services;
    }

    /// <summary>
    ///     Use navyblue rate limiting.
    /// </summary>
    /// <param name="app">The app.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IApplicationBuilder</returns>
    public static IApplicationBuilder UseNavyblueRateLimiting(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseRateLimiter();
    }
}
#endif
