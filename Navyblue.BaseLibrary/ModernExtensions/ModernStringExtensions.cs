// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernStringExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:00
// ****************************************************************************************************************************************
// <copyright file="ModernStringExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable

using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public static class ModernStringExtensions
{
    /// <summary>
    ///     Ases the read only span.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static ReadOnlySpan<char> AsReadOnlySpan(this string? value) => value.AsSpan();

    /// <summary>
    ///     Ensures the ends with.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="suffix">The suffix.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     value
    ///     or
    ///     suffix
    /// </exception>
    public static string EnsureEndsWith(this string value, string suffix, StringComparison comparison = StringComparison.Ordinal)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (suffix == null) throw new ArgumentNullException(nameof(suffix));
        return value.EndsWith(suffix, comparison) ? value : value + suffix;
    }

    /// <summary>
    ///     Ensures the starts with.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="prefix">The prefix.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     value
    ///     or
    ///     prefix
    /// </exception>
    public static string EnsureStartsWith(this string value, string prefix, StringComparison comparison = StringComparison.Ordinal)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (prefix == null) throw new ArgumentNullException(nameof(prefix));
        return value.StartsWith(prefix, comparison) ? value : prefix + value;
    }

    /// <summary>
    ///     Determines whether [is not null or white space].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     <c>true</c> if [is not null or white space] [the specified value]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNotNullOrWhiteSpace(this string? value) => !string.IsNullOrWhiteSpace(value);

    /// <summary>
    ///     Determines whether [is null or white space].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     <c>true</c> if [is null or white space] [the specified value]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    ///     Normalizes the line endings.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="newline">The newline.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">value</exception>
    public static string NormalizeLineEndings(this string value, string newline = "\n")
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\r", "\n", StringComparison.Ordinal).Replace("\n", newline, StringComparison.Ordinal);
    }

    /// <summary>
    ///     Removes the white space.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">value</exception>
    public static string RemoveWhiteSpace(this string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        StringBuilder builder = new StringBuilder(value.Length);
        foreach (char ch in value)
        {
            if (!char.IsWhiteSpace(ch)) builder.Append(ch);
        }

        return builder.ToString();
    }

    /// <summary>
    ///     Converts to camelcase.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">value</exception>
    public static string ToCamelCase(this string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return string.IsNullOrEmpty(value) ? value : char.ToLowerInvariant(value[0]) + value.Substring(1);
    }

    /// <summary>
    ///     Converts to kebabcase.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToKebabCase(this string value) => ToSeparatedCase(value, '-');

    /// <summary>
    ///     Converts to pascalcase.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">value</exception>
    public static string ToPascalCase(this string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return string.IsNullOrEmpty(value) ? value : char.ToUpperInvariant(value[0]) + value.Substring(1);
    }

    /// <summary>
    ///     Converts to snakecase.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToSnakeCase(this string value) => ToSeparatedCase(value, '_');

    /// <summary>
    ///     Truncates the specified maximum length.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <param name="suffix">The suffix.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">value</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">maxLength</exception>
    public static string Truncate(this string value, int maxLength, string suffix = "")
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (maxLength < 0) throw new ArgumentOutOfRangeException(nameof(maxLength));
        if (value.Length <= maxLength) return value;
        if (suffix.Length >= maxLength) return suffix.Substring(0, maxLength);
        return value.Substring(0, maxLength - suffix.Length) + suffix;
    }

    /// <summary>
    ///     Converts to separatedcase.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="separator">The separator.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">value</exception>
    private static string ToSeparatedCase(string value, char separator)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (value.Length == 0) return value;

        StringBuilder builder = new StringBuilder(value.Length + 8);
        for (int i = 0; i < value.Length; i++)
        {
            char ch = value[i];
            if (char.IsWhiteSpace(ch) || ch == '-' || ch == '_')
            {
                if (builder.Length > 0 && builder[builder.Length - 1] != separator) builder.Append(separator);
                continue;
            }

            if (char.IsUpper(ch) && i > 0 && builder.Length > 0 && builder[builder.Length - 1] != separator)
            {
                builder.Append(separator);
            }

            builder.Append(char.ToLowerInvariant(ch));
        }

        return builder.ToString().Trim(separator);
    }
}