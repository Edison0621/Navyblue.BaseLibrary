// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernTypeExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="ModernTypeExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Reflection;

namespace Navyblue.BaseLibrary.Extensions;

public static class ModernTypeExtensions
{
    public static bool IsNullableType(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        return Nullable.GetUnderlyingType(type) != null;
    }

    public static Type UnwrapNullableType(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    public static bool IsAssignableToGenericType(this Type type, Type genericTypeDefinition)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (genericTypeDefinition == null) throw new ArgumentNullException(nameof(genericTypeDefinition));
        if (!genericTypeDefinition.IsGenericTypeDefinition) throw new ArgumentException("Type must be a generic type definition.", nameof(genericTypeDefinition));

        return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericTypeDefinition) || IsGenericBaseType(type, genericTypeDefinition);
    }

    public static string GetFriendlyName(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (!type.IsGenericType) return type.Name;
        string name = type.Name;
        int tickIndex = name.IndexOf('`');
        if (tickIndex > 0) name = name.Substring(0, tickIndex);
        return name + "<" + string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName)) + ">";
    }

    public static IEnumerable<Type> GetConcreteTypesAssignableTo<T>(this Assembly assembly)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        return assembly.DefinedTypes.Where(t => !t.IsAbstract && !t.IsInterface && typeof(T).IsAssignableFrom(t)).Select(t => t.AsType());
    }

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