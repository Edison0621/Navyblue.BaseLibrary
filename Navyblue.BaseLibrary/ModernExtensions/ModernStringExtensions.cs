// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernStringExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="ModernStringExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

public static class ModernStringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);
    public static bool IsNotNullOrWhiteSpace(this string? value) => !string.IsNullOrWhiteSpace(value);
    public static ReadOnlySpan<char> AsReadOnlySpan(this string? value) => value.AsSpan();

    public static string ToCamelCase(this string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return string.IsNullOrEmpty(value) ? value : char.ToLowerInvariant(value[0]) + value.Substring(1);
    }

    public static string ToPascalCase(this string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return string.IsNullOrEmpty(value) ? value : char.ToUpperInvariant(value[0]) + value.Substring(1);
    }

    public static string Truncate(this string value, int maxLength, string suffix = "")
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (maxLength < 0) throw new ArgumentOutOfRangeException(nameof(maxLength));
        if (value.Length <= maxLength) return value;
        if (suffix.Length >= maxLength) return suffix.Substring(0, maxLength);
        return value.Substring(0, maxLength - suffix.Length) + suffix;
    }

    public static string NormalizeLineEndings(this string value, string newline = "\n")
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\r", "\n", StringComparison.Ordinal).Replace("\n", newline, StringComparison.Ordinal);
    }

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

    public static string ToSnakeCase(this string value) => ToSeparatedCase(value, '_');
    public static string ToKebabCase(this string value) => ToSeparatedCase(value, '-');

    public static string EnsureStartsWith(this string value, string prefix, StringComparison comparison = StringComparison.Ordinal)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (prefix == null) throw new ArgumentNullException(nameof(prefix));
        return value.StartsWith(prefix, comparison) ? value : prefix + value;
    }

    public static string EnsureEndsWith(this string value, string suffix, StringComparison comparison = StringComparison.Ordinal)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (suffix == null) throw new ArgumentNullException(nameof(suffix));
        return value.EndsWith(suffix, comparison) ? value : value + suffix;
    }

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