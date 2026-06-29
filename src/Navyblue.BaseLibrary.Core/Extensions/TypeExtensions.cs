// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : TypeExtensions.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-29  13:02
// ****************************************************************************************************************************************
// <copyright file="TypeExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Reflection;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
///     The type extensions.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    ///     Checks if is nullable type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A bool</returns>
    public static bool IsNullableType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return Nullable.GetUnderlyingType(type) is not null;
    }

    /// <summary>
    ///     Unwraps nullable type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A Type</returns>
    public static Type UnwrapNullableType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    /// <summary>
    ///     Checks if is assignable converts to generic type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="genericTypeDefinition">The generic type definition.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A bool</returns>
    public static bool IsAssignableToGenericType(this Type type, Type genericTypeDefinition)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(genericTypeDefinition);

        if (!genericTypeDefinition.IsGenericTypeDefinition)
        {
            throw new ArgumentException("Type must be a generic type definition.", nameof(genericTypeDefinition));
        }

        return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericTypeDefinition)
               || IsGenericBaseType(type, genericTypeDefinition);
    }

    /// <summary>
    ///     Get friendly name.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string GetFriendlyName(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        string name = type.Name;
        int tickIndex = name.IndexOf('`', StringComparison.Ordinal);
        if (tickIndex > 0)
        {
            name = name[..tickIndex];
        }

        return $"{name}<{string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName))}>";
    }

    /// <summary>
    ///     Get concrete types assignable converts to.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="assembly">The assembly.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><![CDATA[IEnumerable<Type>]]></returns>
    public static IEnumerable<Type> GetConcreteTypesAssignableTo<T>(this Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        return assembly.DefinedTypes
            .Where(t => !t.IsAbstract && !t.IsInterface && typeof(T).IsAssignableFrom(t))
            .Select(t => t.AsType());
    }

    private static bool IsGenericBaseType(Type type, Type genericTypeDefinition)
    {
        while (type.BaseType is not null)
        {
            type = type.BaseType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                return true;
            }
        }

        return false;
    }
}