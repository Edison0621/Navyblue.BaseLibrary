// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Modules.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="Modules.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Navyblue.BaseLibrary.Modularity;

/// <summary>
///     The framework module interface.
/// </summary>
public interface IFrameworkModule
{
    /// <summary>
    ///     Configures the services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}

/// <summary>
///     The module base.
/// </summary>
public abstract class ModuleBase : IFrameworkModule
{
    #region IFrameworkModule Members

    /// <summary>
    ///     Configures the services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
    }

    #endregion
}

/// <summary>
///     The module dependency attribute.
/// </summary>
/// <param name="moduleType">The module type.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ModuleDependencyAttribute(Type moduleType) : Attribute
{
    /// <summary>
    ///     Gets the module type.
    /// </summary>
    public Type ModuleType { get; } = moduleType;
}

/// <summary>
///     The module options.
/// </summary>
public sealed class ModuleOptions
{
    /// <summary>
    ///     Gets the assemblies.
    /// </summary>
    public IList<Assembly> Assemblies { get; } = [];

    /// <summary>
    ///     Gets the disabled modules.
    /// </summary>
    public ISet<Type> DisabledModules { get; } = new HashSet<Type>();
}

/// <summary>
///     The module service collection extensions.
/// </summary>
public static class ModuleServiceCollectionExtensions
{
    /// <summary>
    ///     Add navyblue modules.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>An IServiceCollection</returns>
    public static IServiceCollection AddNavyblueModules(this IServiceCollection services, IConfiguration configuration, Action<ModuleOptions>? configure = null)
    {
        ModuleOptions options = new ModuleOptions();
        configure?.Invoke(options);
        Type[] moduleTypes = options.Assemblies.SelectMany(a => a.DefinedTypes).Where(t => !t.IsAbstract && typeof(IFrameworkModule).IsAssignableFrom(t)).Select(t => t.AsType()).Where(t => !options.DisabledModules.Contains(t)).ToArray();
        foreach (Type moduleType in moduleTypes)
            if (Activator.CreateInstance(moduleType) is IFrameworkModule module)
            {
                module.ConfigureServices(services, configuration);
                services.AddSingleton(typeof(IFrameworkModule), module);
            }

        return services;
    }
}