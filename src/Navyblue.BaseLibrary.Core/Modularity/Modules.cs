using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Navyblue.BaseLibrary.Modularity;

public interface IFrameworkModule { void ConfigureServices(IServiceCollection services, IConfiguration configuration); }
public abstract class ModuleBase : IFrameworkModule { public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration) { } }
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)] public sealed class ModuleDependencyAttribute(Type moduleType) : Attribute { public Type ModuleType { get; } = moduleType; }
public sealed class ModuleOptions { public IList<System.Reflection.Assembly> Assemblies { get; } = []; public ISet<Type> DisabledModules { get; } = new HashSet<Type>(); }
public static class ModuleServiceCollectionExtensions
{
    public static IServiceCollection AddNavyblueModules(this IServiceCollection services, IConfiguration configuration, Action<ModuleOptions>? configure = null)
    {
        var options = new ModuleOptions(); configure?.Invoke(options);
        var moduleTypes = options.Assemblies.SelectMany(a => a.DefinedTypes).Where(t => !t.IsAbstract && typeof(IFrameworkModule).IsAssignableFrom(t)).Select(t => t.AsType()).Where(t => !options.DisabledModules.Contains(t)).ToArray();
        foreach (var moduleType in moduleTypes) if (Activator.CreateInstance(moduleType) is IFrameworkModule module) { module.ConfigureServices(services, configuration); services.AddSingleton(typeof(IFrameworkModule), module); }
        return services;
    }
}
