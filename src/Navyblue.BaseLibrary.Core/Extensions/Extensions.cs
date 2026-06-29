using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? value) => string.IsNullOrWhiteSpace(value);
    public static bool IsNotNullOrWhiteSpace(this string? value) => !string.IsNullOrWhiteSpace(value);
    public static ReadOnlySpan<char> AsReadOnlySpan(this string? value) => value.AsSpan();

    public static string ToCamelCase(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return string.IsNullOrEmpty(value) ? value : char.ToLowerInvariant(value[0]) + value[1..];
    }

    public static string ToPascalCase(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return string.IsNullOrEmpty(value) ? value : char.ToUpperInvariant(value[0]) + value[1..];
    }

    public static string Truncate(this string value, int maxLength, string suffix = "")
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(maxLength);
        if (value.Length <= maxLength) return value;
        if (suffix.Length >= maxLength) return suffix[..maxLength];
        return value[..(maxLength - suffix.Length)] + suffix;
    }

    public static string NormalizeLineEndings(this string value, string newline = "\n")
    {
        ArgumentNullException.ThrowIfNull(value);
        return value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\r", "\n", StringComparison.Ordinal).Replace("\n", newline, StringComparison.Ordinal);
    }

    public static string RemoveWhiteSpace(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return string.Create(value.Length, value, static (span, source) =>
        {
            var position = 0;
            foreach (var ch in source)
            {
                if (!char.IsWhiteSpace(ch)) span[position++] = ch;
            }

            span[position..].Clear();
        }).TrimEnd('\0');
    }

    public static string ToSnakeCase(this string value) => ToSeparatedCase(value, '_');
    public static string ToKebabCase(this string value) => ToSeparatedCase(value, '-');

    public static string EnsureStartsWith(this string value, string prefix, StringComparison comparison = StringComparison.Ordinal)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(prefix);
        return value.StartsWith(prefix, comparison) ? value : prefix + value;
    }

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

        var builder = new StringBuilder(value.Length + 8);
        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];
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

public static class EnumerableExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source) => source is null || !source.Any();
    public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int size) { ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size); return source.Chunk(size); }
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer) => new(source, comparer);
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class => source.Where(static x => x is not null)!;
}

public static class DateTimeExtensions
{
    public static long ToUnixTimeMilliseconds(this DateTimeOffset value) => value.ToUniversalTime().ToUnixTimeMilliseconds();
    public static DateTimeOffset StartOfDay(this DateTimeOffset value) => new(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset);
    public static DateTimeOffset EndOfDay(this DateTimeOffset value) => value.StartOfDay().AddDays(1).AddTicks(-1);
    public static bool IsBetween(this DateTimeOffset value, DateTimeOffset start, DateTimeOffset end, bool inclusive = true) => inclusive ? value >= start && value <= end : value > start && value < end;
}

public static class GuidExtensions
{
    public static bool IsEmpty(this Guid value) => value == Guid.Empty;
    public static bool IsNotEmpty(this Guid value) => value != Guid.Empty;
    public static string ToNString(this Guid value) => value.ToString("N");
}

public static class IntExtensions
{
    public static bool Between(this int value, int min, int max) => value >= min && value <= max;
    public static int ClampTo(this int value, int min, int max) => Math.Clamp(value, min, max);
}

public static class EnumExtensions
{
    public static string GetName<TEnum>(this TEnum value) where TEnum : struct, Enum => Enum.GetName(value) ?? value.ToString();
    public static bool HasFlagFast<TEnum>(this TEnum value, TEnum flag) where TEnum : struct, Enum => (Convert.ToUInt64(value) & Convert.ToUInt64(flag)) == Convert.ToUInt64(flag);
}
