// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : DomainServiceRegistration.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="DomainServiceRegistration.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Navyblue.Foundation.Domain;

/// <summary>
///     The domain service registration.
/// </summary>
public static class DomainServiceRegistration
{
    /// <summary>
    ///     Add domain services from.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="assemblies">The assemblies.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IServiceCollection</returns>
    public static IServiceCollection AddDomainServicesFrom(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        foreach (TypeInfo implementationType in assemblies
                     .SelectMany(a => a.DefinedTypes)
                     .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IDomainService).IsAssignableFrom(t)))
        {
            IEnumerable<Type> serviceTypes = implementationType.ImplementedInterfaces
                .Where(i => i != typeof(IDomainService) && typeof(IDomainService).IsAssignableFrom(i))
                .DefaultIfEmpty(implementationType.AsType());

            foreach (Type serviceType in serviceTypes)
            {
                services.AddScoped(serviceType, implementationType.AsType());
            }
        }

        return services;
    }

    /// <summary>
    ///     Add domain event handlers from.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="assemblies">The assemblies.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>An IServiceCollection</returns>
    public static IServiceCollection AddDomainEventHandlersFrom(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        foreach (TypeInfo implementationType in assemblies
                     .SelectMany(a => a.DefinedTypes)
                     .Where(t => !t.IsAbstract && !t.IsInterface))
        {
            foreach (Type handlerInterface in implementationType.ImplementedInterfaces.Where(IsDomainEventHandler))
            {
                services.AddScoped(handlerInterface, implementationType.AsType());
            }
        }

        return services;

        static bool IsDomainEventHandler(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>);
        }
    }
}