// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ServiceCollectionExtensions.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="ServiceCollectionExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Navyblue.Foundation.Cqrs.Internal;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     DI registration for Navyblue CQRS buses, handlers, and pipeline behaviors.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers command/query/event buses, in-memory inbox/outbox, pipeline behaviors,
    ///     and scans the given assemblies for handlers and processors.
    /// </summary>
    public static IServiceCollection AddNavyblueCqrs(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
            assemblies = [Assembly.GetCallingAssembly()];

        services.AddScoped<IRequestHandlerResolver>(sp => new RequestHandlerResolver(type => sp.GetService(type)));
        services.AddScoped<ICommandBus, CommandBus>();
        services.AddScoped<IQueryService, QueryService>();
        services.AddScoped<EventBus>();
        services.AddScoped<IRequestEventCollector>(_ => new RequestEventCollector());
        services.AddScoped<IDomainEventBus>(sp =>
            new CapturingEventBus(sp.GetRequiredService<EventBus>(), sp.GetRequiredService<IRequestEventCollector>()));

        services.AddSingleton<IOutbox, InMemoryOutbox>();
        services.AddSingleton<IOutboxDrain>(sp => (IOutboxDrain)sp.GetRequiredService<IOutbox>());
        services.AddSingleton<IInboxStore, InMemoryInboxStore>();
        services.AddSingleton<IHostedService, OutboxDispatcherHostedService>();

        services.AddScoped(typeof(IRequestBehavior<,>), typeof(TransactionBehavior<,>));
        services.AddScoped(typeof(IRequestBehavior<,>), typeof(OutboxBehavior<,>));
        services.AddScoped(typeof(IRequestBehavior<,>), typeof(InboxBehavior<,>));

        foreach (Assembly assembly in assemblies)
            RegisterFromAssembly(services, assembly);

        return services;
    }

    /// <summary>
    ///     Legacy alias for <see cref="AddNavyblueCqrs" />.
    /// </summary>
    [Obsolete("Use AddNavyblueCqrs instead.")]
    public static IServiceCollection AddCqrsMediatrLite(this IServiceCollection services, params Assembly[] assemblies)
        => services.AddNavyblueCqrs(assemblies);

    private static void RegisterFromAssembly(IServiceCollection services, Assembly assembly)
    {
        Type[] types = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .ToArray();

        foreach (Type type in types)
        {
            Type? commandBase = GetGenericBaseType(type, typeof(CommandHandler<,>));
            if (commandBase != null)
                services.AddScoped(commandBase, type);

            Type? queryBase = GetGenericBaseType(type, typeof(QueryHandler<,>));
            if (queryBase != null)
                services.AddScoped(queryBase, type);

            Type? eventBase = GetGenericBaseType(type, typeof(EventHandler<>));
            if (eventBase != null)
                services.AddScoped(eventBase, type);

            Type[] interfaces = type.GetInterfaces();

            if (interfaces.Contains(typeof(IGlobalRequestPreProcessor)))
                services.AddScoped(typeof(IGlobalRequestPreProcessor), type);

            if (interfaces.Contains(typeof(IGlobalRequestPostProcessor)))
                services.AddScoped(typeof(IGlobalRequestPostProcessor), type);

            foreach (Type iface in interfaces)
            {
                if (!iface.IsGenericType) continue;
                Type def = iface.GetGenericTypeDefinition();
                if (def == typeof(IRequestPreProcessor<>)
                    || def == typeof(IRequestPostProcessor<,>)
                    || def == typeof(IRequestBehavior<,>))
                {
                    services.AddScoped(iface, type);
                }
            }
        }
    }

    private static Type? GetGenericBaseType(Type type, Type openGeneric)
    {
        Type? current = type;
        while (current != null && current != typeof(object))
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == openGeneric)
                return current;
            current = current.BaseType;
        }

        return null;
    }
}