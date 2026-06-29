// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Extensions.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-29  13:02
// ****************************************************************************************************************************************
// <copyright file="Extensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
///     The string extensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Checks if is null or white space.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A bool</returns>
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    ///     Checks if is not null or white space.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A bool</returns>
    public static bool IsNotNullOrWhiteSpace(this string? value) => !string.IsNullOrWhiteSpace(value);

    /// <summary>
    ///     As read only span.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><![CDATA[ReadOnlySpan<char>]]></returns>
    public static ReadOnlySpan<char> AsReadOnlySpan(this string? value) => value.AsSpan();

    /// <summary>
    ///     Converts to camel case.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string ToCamelCase(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return string.IsNullOrEmpty(value) ? value : char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    ///     Converts to pascal case.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string ToPascalCase(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return string.IsNullOrEmpty(value) ? value : char.ToUpperInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="maxLength">The max length.</param>
    /// <param name="suffix">The suffix.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns>A string</returns>
    public static string Truncate(this string value, int maxLength, string suffix = "")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);
        if (value.Length <= maxLength) return value;
        if (suffix.Length >= maxLength) return suffix[..maxLength];
        return value[..(maxLength - suffix.Length)] + suffix;
    }

    /// <summary>
    ///     Normalizes line endings.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="newline">The newline.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string NormalizeLineEndings(this string value, string newline = "\n")
    {
        ArgumentNullException.ThrowIfNull(value);
        return value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\r", "\n", StringComparison.Ordinal).Replace("\n", newline, StringComparison.Ordinal);
    }

    /// <summary>
    ///     Remove white space.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string RemoveWhiteSpace(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return string.Create(value.Length, value, static (span, source) =>
        {
            int position = 0;
            foreach (char ch in source)
            {
                if (!char.IsWhiteSpace(ch)) span[position++] = ch;
            }

            span[position..].Clear();
        }).TrimEnd('\0');
    }

    /// <summary>
    ///     Converts to snake case.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string</returns>
    public static string ToSnakeCase(this string value) => ToSeparatedCase(value, '_');

    /// <summary>
    ///     Converts to kebab case.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string</returns>
    public static string ToKebabCase(this string value) => ToSeparatedCase(value, '-');

    /// <summary>
    ///     Checks if starts with.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="prefix">The prefix.</param>
    /// <param name="comparison">The comparison.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string EnsureStartsWith(this string value, string prefix, StringComparison comparison = StringComparison.Ordinal)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefix);
        return value.StartsWith(prefix, comparison) ? value : prefix + value;
    }

    /// <summary>
    ///     Checks if ends with.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="suffix">The suffix.</param>
    /// <param name="comparison">The comparison.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A string</returns>
    public static string EnsureEndsWith(this string value, string suffix, StringComparison comparison = StringComparison.Ordinal)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(suffix);
        return value.EndsWith(suffix, comparison) ? value : value + suffix;
    }

    private static string ToSeparatedCase(string value, char separator)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.Length == 0) return value;

        StringBuilder builder = new StringBuilder(value.Length + 8);
        for (int i = 0; i < value.Length; i++)
        {
            char ch = value[i];
            if (char.IsWhiteSpace(ch) || ch is '-' or '_')
            {
                if (builder.Length > 0 && builder[^1] != separator) builder.Append(separator);
                continue;
            }

            if (char.IsUpper(ch) && i > 0 && builder.Length > 0 && builder[^1] != separator)
            {
                builder.Append(separator);
            }

            builder.Append(char.ToLowerInvariant(ch));
        }

        return builder.ToString().Trim(separator);
    }
}

/// <summary>
///     The enumerable extensions.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Checks if is null or empty.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="source">The source.</param>
    /// <returns>A bool</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source) => source is null || !source.Any();

    /// <summary>
    ///     Chunk the by.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="source">The source.</param>
    /// <param name="size">The size.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns><![CDATA[IEnumerable<IEnumerable<T>>]]></returns>
    public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);
        return source.Chunk(size);
    }

    /// <summary>
    ///     Converts to hash set.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="source">The source.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns><![CDATA[HashSet<T>]]></returns>
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer) => new(source, comparer);

    /// <summary>
    ///     Wheres not null.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="source">The source.</param>
    /// <returns><![CDATA[IEnumerable<T>]]></returns>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class => source.Where(static x => x is not null)!;
}

/// <summary>
///     The date time extensions.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    ///     Converts to unix time milliseconds.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A long</returns>
    public static long ToUnixTimeMilliseconds(this DateTimeOffset value) => value.ToUniversalTime().ToUnixTimeMilliseconds();

    /// <summary>
    ///     Start of day.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A DateTimeOffset</returns>
    public static DateTimeOffset StartOfDay(this DateTimeOffset value) => new(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset);

    /// <summary>
    ///     End of day.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A DateTimeOffset</returns>
    public static DateTimeOffset EndOfDay(this DateTimeOffset value) => value.StartOfDay().AddDays(1).AddTicks(-1);

    /// <summary>
    ///     Checks if is between.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <param name="inclusive">If true, inclusive.</param>
    /// <returns>A bool</returns>
    public static bool IsBetween(this DateTimeOffset value, DateTimeOffset start, DateTimeOffset end, bool inclusive = true) => inclusive ? value >= start && value <= end : value > start && value < end;
}

/// <summary>
///     The guid extensions.
/// </summary>
public static class GuidExtensions
{
    /// <summary>
    ///     Checks if is empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A bool</returns>
    public static bool IsEmpty(this Guid value) => value == Guid.Empty;

    /// <summary>
    ///     Checks if is not empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A bool</returns>
    public static bool IsNotEmpty(this Guid value) => value != Guid.Empty;

    /// <summary>
    ///     Converts to n string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string</returns>
    public static string ToNString(this Guid value) => value.ToString("N");
}

/// <summary>
///     The integer extensions.
/// </summary>
public static class IntExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="min">The min.</param>
    /// <param name="max">The max.</param>
    /// <returns>A bool</returns>
    public static bool Between(this int value, int min, int max) => value >= min && value <= max;

    /// <summary>
    ///     Clamp the converts to.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="min">The min.</param>
    /// <param name="max">The max.</param>
    /// <returns>An int</returns>
    public static int ClampTo(this int value, int min, int max) => Math.Clamp(value, min, max);
}

/// <summary>
///     The enum extensions.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    ///     Get the name.
    /// </summary>
    /// <typeparam name="TEnum" />
    /// <param name="value">The value.</param>
    /// <returns>A string</returns>
    public static string GetName<TEnum>(this TEnum value) where TEnum : struct, Enum => Enum.GetName(value) ?? value.ToString();

    /// <summary>
    ///     Has flag fast.
    /// </summary>
    /// <typeparam name="TEnum" />
    /// <param name="value">The value.</param>
    /// <param name="flag">The flag.</param>
    /// <returns>A bool</returns>
    public static bool HasFlagFast<TEnum>(this TEnum value, TEnum flag) where TEnum : struct, Enum => (Convert.ToUInt64(value) & Convert.ToUInt64(flag)) == Convert.ToUInt64(flag);
}