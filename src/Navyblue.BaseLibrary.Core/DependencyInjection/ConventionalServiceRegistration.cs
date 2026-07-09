// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ConventionalServiceRegistration.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="ConventionalServiceRegistration.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Navyblue.BaseLibrary.DependencyInjection;

/// <summary>
/// </summary>
public interface ITransientDependency;

/// <summary>
/// </summary>
public interface IScopedDependency;

/// <summary>
/// </summary>
public interface ISingletonDependency;

/// <summary>
/// </summary>
public static class ConventionalServiceRegistration
{
    /// <summary>
    ///     Adds the navyblue conventional services from.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static IServiceCollection AddNavyblueConventionalServicesFrom(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        foreach (TypeInfo implementationType in assemblies
                     .SelectMany(a => a.DefinedTypes)
                     .Where(t => !t.IsAbstract && !t.IsInterface))
        {
            ServiceLifetime? lifetime = GetLifetime(implementationType.AsType());
            if (lifetime is null)
            {
                continue;
            }

            Type[] serviceTypes = GetServiceTypes(implementationType.AsType()).ToArray();
            if (serviceTypes.Length == 0)
            {
                services.TryAdd(new ServiceDescriptor(implementationType.AsType(), implementationType.AsType(), lifetime.Value));
                continue;
            }

            foreach (Type serviceType in serviceTypes)
            {
                services.TryAdd(new ServiceDescriptor(serviceType, implementationType.AsType(), lifetime.Value));
            }
        }

        return services;
    }

    /// <summary>
    ///     Gets the lifetime.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    private static ServiceLifetime? GetLifetime(Type type)
    {
        if (typeof(ISingletonDependency).IsAssignableFrom(type)) return ServiceLifetime.Singleton;
        if (typeof(IScopedDependency).IsAssignableFrom(type)) return ServiceLifetime.Scoped;
        if (typeof(ITransientDependency).IsAssignableFrom(type)) return ServiceLifetime.Transient;
        return null;
    }

    /// <summary>
    ///     Gets the service types.
    /// </summary>
    /// <param name="implementationType">Type of the implementation.</param>
    /// <returns></returns>
    private static IEnumerable<Type> GetServiceTypes(Type implementationType)
    {
        return implementationType.GetInterfaces()
            .Where(i => i != typeof(ITransientDependency)
                        && i != typeof(IScopedDependency)
                        && i != typeof(ISingletonDependency)
                        && !i.Name.StartsWith("IEnumerable", StringComparison.Ordinal));
    }
}