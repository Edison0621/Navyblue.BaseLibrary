using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Navyblue.BaseLibrary.Domain;

public static class DomainServiceRegistration
{
    public static IServiceCollection AddDomainServicesFrom(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        foreach (var implementationType in assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IDomainService).IsAssignableFrom(t)))
        {
            var serviceTypes = implementationType.ImplementedInterfaces
                .Where(i => i != typeof(IDomainService) && typeof(IDomainService).IsAssignableFrom(i))
                .DefaultIfEmpty(implementationType.AsType());

            foreach (var serviceType in serviceTypes)
            {
                services.AddScoped(serviceType, implementationType.AsType());
            }
        }

        return services;
    }

    public static IServiceCollection AddDomainEventHandlersFrom(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        foreach (var implementationType in assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(t => !t.IsAbstract && !t.IsInterface))
        {
            foreach (var handlerInterface in implementationType.ImplementedInterfaces.Where(IsDomainEventHandler))
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
