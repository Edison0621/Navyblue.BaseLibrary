// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernTypeExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:00
// ****************************************************************************************************************************************
// <copyright file="ModernTypeExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Reflection;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public static class ModernTypeExtensions
{
    /// <summary>
    ///     Determines whether [is nullable type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    ///     <c>true</c> if [is nullable type] [the specified type]; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">type</exception>
    public static bool IsNullableType(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        return Nullable.GetUnderlyingType(type) != null;
    }

    /// <summary>
    ///     Unwraps the type of the nullable.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">type</exception>
    public static Type UnwrapNullableType(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    /// <summary>
    ///     Determines whether [is assignable to generic type] [the specified generic type definition].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="genericTypeDefinition">The generic type definition.</param>
    /// <returns>
    ///     <c>true</c> if [is assignable to generic type] [the specified generic type definition]; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     type
    ///     or
    ///     genericTypeDefinition
    /// </exception>
    /// <exception cref="System.ArgumentException">Type must be a generic type definition. - genericTypeDefinition</exception>
    public static bool IsAssignableToGenericType(this Type type, Type genericTypeDefinition)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (genericTypeDefinition == null) throw new ArgumentNullException(nameof(genericTypeDefinition));
        if (!genericTypeDefinition.IsGenericTypeDefinition) throw new ArgumentException("Type must be a generic type definition.", nameof(genericTypeDefinition));

        return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericTypeDefinition) || IsGenericBaseType(type, genericTypeDefinition);
    }

    /// <summary>
    ///     Gets the name of the friendly.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">type</exception>
    public static string GetFriendlyName(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (!type.IsGenericType) return type.Name;
        string name = type.Name;
        int tickIndex = name.IndexOf('`');
        if (tickIndex > 0) name = name.Substring(0, tickIndex);
        return name + "<" + string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName)) + ">";
    }

    /// <summary>
    ///     Gets the concrete types assignable to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assembly">The assembly.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">assembly</exception>
    public static IEnumerable<Type> GetConcreteTypesAssignableTo<T>(this Assembly assembly)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        return assembly.DefinedTypes.Where(t => !t.IsAbstract && !t.IsInterface && typeof(T).IsAssignableFrom(t)).Select(t => t.AsType());
    }

    /// <summary>
    ///     Determines whether [is generic base type] [the specified type].
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="genericTypeDefinition">The generic type definition.</param>
    /// <returns>
    ///     <c>true</c> if [is generic base type] [the specified type]; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsGenericBaseType(Type type, Type genericTypeDefinition)
    {
        while (type.BaseType != null)
        {
            type = type.BaseType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition) return true;
        }

        return false;
    }
}