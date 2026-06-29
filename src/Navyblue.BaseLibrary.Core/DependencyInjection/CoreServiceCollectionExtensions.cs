using Microsoft.Extensions.DependencyInjection;
using Navyblue.BaseLibrary.Domain;
using Navyblue.BaseLibrary.Events;
using Navyblue.BaseLibrary.Primitives;

namespace Navyblue.BaseLibrary.DependencyInjection;

public sealed class NavyblueCoreOptions
{
    public long WorkerId { get; set; }
    public long DataCenterId { get; set; }
    public bool RegisterInMemoryEventBus { get; set; } = true;
}

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddNavyblueCore(this IServiceCollection services, Action<NavyblueCoreOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        var options = new NavyblueCoreOptions();
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
