// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernValidationExtensions.cs
// Created          : 2026-06-30  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:04
// ****************************************************************************************************************************************
// <copyright file="ModernValidationExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public static class ModernValidationExtensions
{
    /// <summary>
    ///     Throws if blank.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="paramName">Name of the parameter.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException">Value cannot be null, empty, or whitespace.</exception>
    public static string ThrowIfBlank(this string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName);
        }

        return value;
    }

    /// <summary>
    ///     Throws if default.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="paramName">Name of the parameter.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException">Value cannot be the default value.</exception>
    public static T ThrowIfDefault<T>(this T value, string paramName) where T : struct, IEquatable<T>
    {
        if (value.Equals(default))
        {
            throw new ArgumentException("Value cannot be the default value.", paramName);
        }

        return value;
    }

    /// <summary>
    ///     Throws if out of range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <param name="paramName">Name of the parameter.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Value must be between {min} and {max}.</exception>
    public static T ThrowIfOutOfRange<T>(this T value, T min, T max, string paramName) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, $"Value must be between {min} and {max}.");
        }

        return value;
    }

    /// <summary>
    ///     Throws if null or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values">The values.</param>
    /// <param name="paramName">Name of the parameter.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    /// <exception cref="System.ArgumentException">Collection cannot be empty.</exception>
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

    /// <summary>
    ///     Throws if.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="paramName">Name of the parameter.</param>
    /// <param name="message">The message.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">predicate</exception>
    /// <exception cref="System.ArgumentException"></exception>
    public static T ThrowIf<T>(this T value, Func<T, bool> predicate, string paramName, string message)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (predicate(value))
        {
            throw new ArgumentException(message, paramName);
        }

        return value;
    }

    /// <summary>
    ///     Determines whether the specified minimum is between.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <param name="includeBounds">if set to <c>true</c> [include bounds].</param>
    /// <returns>
    ///     <c>true</c> if the specified minimum is between; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsBetween<T>(this T value, T min, T max, bool includeBounds = true) where T : IComparable<T>
    {
        int lower = value.CompareTo(min);
        int upper = value.CompareTo(max);
        return includeBounds ? lower >= 0 && upper <= 0 : lower > 0 && upper < 0;
    }
}