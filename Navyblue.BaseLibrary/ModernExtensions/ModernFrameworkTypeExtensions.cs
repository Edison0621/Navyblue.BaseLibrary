// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernFrameworkTypeExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="ModernFrameworkTypeExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Globalization;
#if NET8_0_OR_GREATER
using System.Buffers;
using System.Collections.Frozen;
#endif

namespace Navyblue.BaseLibrary.Extensions;

public static class ModernDateOnlyExtensions
{
    public static DateOnly StartOfMonth(this DateOnly value) => new DateOnly(value.Year, value.Month, 1);
    public static DateOnly EndOfMonth(this DateOnly value) => value.StartOfMonth().AddMonths(1).AddDays(-1);
    public static DateTime ToDateTimeAtStartOfDay(this DateOnly value, DateTimeKind kind = DateTimeKind.Unspecified) => DateTime.SpecifyKind(value.ToDateTime(TimeOnly.MinValue), kind);
    public static DateTime ToDateTimeAtEndOfDay(this DateOnly value, DateTimeKind kind = DateTimeKind.Unspecified) => DateTime.SpecifyKind(value.ToDateTime(TimeOnly.MaxValue), kind);
    public static bool IsBetween(this DateOnly value, DateOnly start, DateOnly end, bool inclusive = true) => inclusive ? value >= start && value <= end : value > start && value < end;
    public static int DaysUntil(this DateOnly value, DateOnly other) => other.DayNumber - value.DayNumber;
    public static bool TryParseDateOnly(this ReadOnlySpan<char> value, out DateOnly result, IFormatProvider? provider = null, DateTimeStyles style = DateTimeStyles.None) => DateOnly.TryParse(value, provider, style, out result);
}

public static class ModernTimeOnlyExtensions
{
    public static bool IsBetween(this TimeOnly value, TimeOnly start, TimeOnly end, bool inclusive = true)
    {
        return start <= end
            ? inclusive ? value >= start && value <= end : value > start && value < end
            : inclusive
                ? value >= start || value <= end
                : value > start || value < end;
    }

    public static TimeSpan Until(this TimeOnly value, TimeOnly other)
    {
        TimeSpan result = other - value;
        return result < TimeSpan.Zero ? result + TimeSpan.FromDays(1) : result;
    }

    public static bool TryParseTimeOnly(this ReadOnlySpan<char> value, out TimeOnly result, IFormatProvider? provider = null, DateTimeStyles style = DateTimeStyles.None) => TimeOnly.TryParse(value, provider, style, out result);
}

public static class ModernHalfExtensions
{
    public static float ToSingle(this Half value) => (float)value;
    public static double ToDouble(this Half value) => (double)value;
    public static bool IsFinite(this Half value) => !Half.IsNaN(value) && !Half.IsInfinity(value);
}

public static class ModernPriorityQueueExtensions
{
    public static void EnqueueRange<TElement, TPriority>(this PriorityQueue<TElement, TPriority> queue, IEnumerable<(TElement Element, TPriority Priority)> items)
    {
        if (queue == null) throw new ArgumentNullException(nameof(queue));
        if (items == null) throw new ArgumentNullException(nameof(items));
        foreach ((TElement Element, TPriority Priority) item in items) queue.Enqueue(item.Element, item.Priority);
    }

    public static bool TryDequeue<TElement, TPriority>(this PriorityQueue<TElement, TPriority> queue, out TElement? element)
    {
        if (queue == null) throw new ArgumentNullException(nameof(queue));
        if (queue.TryDequeue(out TElement? value, out _))
        {
            element = value!;
            return true;
        }

        element = default;
        return false;
    }
}

#if NET7_0_OR_GREATER
public static class ModernGenericMathExtensions
{
    public static bool TryParseNumber<T>(this ReadOnlySpan<char> value, out T result, IFormatProvider? provider = null) where T : ISpanParsable<T>
    {
        if (T.TryParse(value, provider, out T? parsed))
        {
            result = parsed;
            return true;
        }

        result = default!;
        return false;
    }

    public static T ParseOrDefault<T>(this string? value, T defaultValue = default!, IFormatProvider? provider = null) where T : IParsable<T>
    {
        return T.TryParse(value, provider, out T? result) ? result : defaultValue;
    }

    public static bool IsZero(this Int128 value) => value == Int128.Zero;
    public static bool IsZero(this UInt128 value) => value == UInt128.Zero;
    public static string ToInvariantString(this Int128 value) => value.ToString(CultureInfo.InvariantCulture);
    public static string ToInvariantString(this UInt128 value) => value.ToString(CultureInfo.InvariantCulture);
}
#endif

#if NET8_0_OR_GREATER
public static class ModernNet8TypeExtensions
{
    public static SearchValues<char> ToSearchValues(this string values)
    {
        if (values == null) throw new ArgumentNullException(nameof(values));
        return SearchValues.Create(values);
    }

    public static SearchValues<char> ToSearchValues(this ReadOnlySpan<char> values) => SearchValues.Create(values);

    public static bool ContainsAny(this ReadOnlySpan<char> value, SearchValues<char> searchValues)
    {
        if (searchValues == null) throw new ArgumentNullException(nameof(searchValues));
        return value.IndexOfAny(searchValues) >= 0;
    }

    public static FrozenDictionary<TKey, TValue> ToFrozenDictionarySafe<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey>? comparer = null) where TKey : notnull
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.ToFrozenDictionary(comparer);
    }

    public static FrozenSet<T> ToFrozenSetSafe<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.ToFrozenSet(comparer);
    }

    public static DateTimeOffset GetUtcNowOffset(this TimeProvider timeProvider)
    {
        if (timeProvider == null) throw new ArgumentNullException(nameof(timeProvider));
        return timeProvider.GetUtcNow();
    }

    public static Task DelayAsync(this TimeProvider timeProvider, TimeSpan delay, CancellationToken cancellationToken = default)
    {
        if (timeProvider == null) throw new ArgumentNullException(nameof(timeProvider));
        return Task.Delay(delay, timeProvider, cancellationToken);
    }
}
#endif