// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : CoreServiceCollectionExtensions.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:51
// ****************************************************************************************************************************************
// <copyright file="CoreServiceCollectionExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Navyblue.BaseLibrary.Domain;
using Navyblue.BaseLibrary.Events;
using Navyblue.BaseLibrary.Primitives;

namespace Navyblue.BaseLibrary.DependencyInjection;

/// <summary>
///     The navyblue core options.
/// </summary>
public sealed class NavyblueCoreOptions
{
    /// <summary>
    ///     Gets or sets the worker id.
    /// </summary>
    public long WorkerId { get; set; }

    /// <summary>
    ///     Gets or sets the data center id.
    /// </summary>
    public long DataCenterId { get; set; }

    /// <summary>
    ///     Gets or sets  a value indicating whether to register in memory event bus.
    /// </summary>
    public bool RegisterInMemoryEventBus { get; set; } = true;
}

/// <summary>
///     The core service collection extensions.
/// </summary>
public static class CoreServiceCollectionExtensions
{
    /// <summary>
    ///     Add navyblue core.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configure">The configure.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IServiceCollection</returns>
    public static IServiceCollection AddNavyblueCore(this IServiceCollection services, Action<NavyblueCoreOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        NavyblueCoreOptions options = new NavyblueCoreOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IIdGenerator<long>>(_ => new SnowflakeIdGenerator(options.WorkerId, options.DataCenterId));
        services.AddScoped<IDomainEventDispatcher, LocalDomainEventDispatcher>();

        if (options.RegisterInMemoryEventBus)
        {
            services.AddScoped<IEventBus, InMemoryEventBus>();
        }

        return services;
    }
}