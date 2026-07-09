// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernValidationExtensions.cs
// Created          : 2026-06-30
// ****************************************************************************************************************************************

#nullable enable
namespace Navyblue.BaseLibrary.Extensions;

public static class ModernValidationExtensions
{
    public static string ThrowIfBlank(this string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName);
        }

        return value;
    }

    public static T ThrowIfDefault<T>(this T value, string paramName) where T : struct, IEquatable<T>
    {
        if (value.Equals(default))
        {
            throw new ArgumentException("Value cannot be the default value.", paramName);
        }

        return value;
    }

    public static T ThrowIfOutOfRange<T>(this T value, T min, T max, string paramName) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, $"Value must be between {min} and {max}.");
        }

        return value;
    }

    public static IEnumerable<T> ThrowIfNullOrEmpty<T>(this IEnumerable<T>? values, string paramName)
    {
        if (values == null)
        {
            throw new ArgumentNullException(paramName);
        }

        if (!values.Any())
        {
            throw new ArgumentException("Collection cannot be empty.", paramName);
        }

        return values;
    }

    public static T ThrowIf<T>(this T value, Func<T, bool> predicate, string paramName, string message)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (predicate(value))
        {
            throw new ArgumentException(message, paramName);
        }

        return value;
    }

    public static bool IsBetween<T>(this T value, T min, T max, bool includeBounds = true) where T : IComparable<T>
    {
        int lower = value.CompareTo(min);
        int upper = value.CompareTo(max);
        return includeBounds ? lower >= 0 && upper <= 0 : lower > 0 && upper < 0;
    }
}
