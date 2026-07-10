// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : FoundationServiceCollectionExtensions.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="FoundationServiceCollectionExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Events;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.DependencyInjection;

/// <summary>
///     Options for <see cref="FoundationServiceCollectionExtensions.AddNavyblueFoundation" />.
///     Configuration section: <c>Navyblue:IdGenerator</c> (WorkerId / DataCenterId).
/// </summary>
public sealed class NavyblueFoundationOptions
{
    /// <summary>
    ///     Configuration section name for id-generator related settings.
    /// </summary>
    public const string IdGeneratorSectionName = "Navyblue:IdGenerator";

    /// <summary>
    ///     Gets or sets the snowflake worker id (0-31).
    /// </summary>
    public long WorkerId { get; set; }

    /// <summary>
    ///     Gets or sets the snowflake data center id (0-31).
    /// </summary>
    public long DataCenterId { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to register the in-memory event bus.
    /// </summary>
    public bool RegisterInMemoryEventBus { get; set; } = true;
}

/// <summary>
///     Foundation DI registration extensions.
/// </summary>
public static class FoundationServiceCollectionExtensions
{
    /// <summary>
    ///     Registers Navyblue Foundation core services (clock, id generator, domain event dispatcher, optional in-memory event bus).
    /// </summary>
    public static IServiceCollection AddNavyblueFoundation(this IServiceCollection services, Action<NavyblueFoundationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        NavyblueFoundationOptions options = new();
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