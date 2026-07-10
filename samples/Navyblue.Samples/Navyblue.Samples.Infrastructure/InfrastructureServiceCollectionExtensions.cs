using Navyblue.Samples.Application.Authentication;
using Navyblue.Samples.Application.Users;
using Navyblue.Samples.Infrastructure.Authentication;
using Navyblue.Samples.Infrastructure.Persistence;
using Navyblue.Samples.Infrastructure.Users;
using Navyblue.Foundation.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace Navyblue.Samples.Infrastructure;

/// <summary>
///     Registers the in-memory infrastructure services used by the sample.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Repositories are singletons so seeded data and writes persist across scoped requests within a run.
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IAuthRepository, InMemoryAuthRepository>();

        // No-op unit of work so the Cqrs TransactionBehavior can resolve its dependency.
        services.AddScoped<ICqrsUnitOfWork, NullUnitOfWork>();

        return services;
    }
}
