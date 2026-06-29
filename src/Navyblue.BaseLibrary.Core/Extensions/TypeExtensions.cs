using System.Reflection;

namespace Navyblue.BaseLibrary.Extensions;

public static class TypeExtensions
{
    public static bool IsNullableType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return Nullable.GetUnderlyingType(type) is not null;
    }

    public static Type UnwrapNullableType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return Nullable.GetUnderlyingType(type) ?? type;
    }

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

    public static string GetFriendlyName(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        var name = type.Name;
        var tickIndex = name.IndexOf('`', StringComparison.Ordinal);
        if (tickIndex > 0)
        {
            name = name[..tickIndex];
        }

        return $"{name}<{string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName))}>";
    }

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
