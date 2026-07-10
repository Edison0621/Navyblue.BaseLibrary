// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernFrameworkTypeExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:04
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

/// <summary>
/// </summary>
public static class ModernDateOnlyExtensions
{
    /// <summary>
    ///     Dayses the until.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns></returns>
    public static int DaysUntil(this DateOnly value, DateOnly other) => other.DayNumber - value.DayNumber;

    /// <summary>
    ///     Ends the of month.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static DateOnly EndOfMonth(this DateOnly value) => value.StartOfMonth().AddMonths(1).AddDays(-1);

    /// <summary>
    ///     Determines whether the specified start is between.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <param name="inclusive">if set to <c>true</c> [inclusive].</param>
    /// <returns>
    ///     <c>true</c> if the specified start is between; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsBetween(this DateOnly value, DateOnly start, DateOnly end, bool inclusive = true) => inclusive ? value >= start && value <= end : value > start && value < end;

    /// <summary>
    ///     Starts the of month.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static DateOnly StartOfMonth(this DateOnly value) => new DateOnly(value.Year, value.Month, 1);

    /// <summary>
    ///     Converts to datetimeatendofday.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="kind">The kind.</param>
    /// <returns></returns>
    public static DateTime ToDateTimeAtEndOfDay(this DateOnly value, DateTimeKind kind = DateTimeKind.Unspecified) => DateTime.SpecifyKind(value.ToDateTime(TimeOnly.MaxValue), kind);

    /// <summary>
    ///     Converts to datetimeatstartofday.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="kind">The kind.</param>
    /// <returns></returns>
    public static DateTime ToDateTimeAtStartOfDay(this DateOnly value, DateTimeKind kind = DateTimeKind.Unspecified) => DateTime.SpecifyKind(value.ToDateTime(TimeOnly.MinValue), kind);

    /// <summary>
    ///     Tries the parse date only.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <param name="provider">The provider.</param>
    /// <param name="style">The style.</param>
    /// <returns></returns>
    public static bool TryParseDateOnly(this ReadOnlySpan<char> value, out DateOnly result, IFormatProvider? provider = null, DateTimeStyles style = DateTimeStyles.None) => DateOnly.TryParse(value, provider, style, out result);
}

/// <summary>
/// </summary>
public static class ModernHalfExtensions
{
    /// <summary>
    ///     Determines whether this instance is finite.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     <c>true</c> if the specified value is finite; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsFinite(this Half value) => !Half.IsNaN(value) && !Half.IsInfinity(value);

    /// <summary>
    ///     Converts to double.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static double ToDouble(this Half value) => (double)value;

    /// <summary>
    ///     Converts to single.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static float ToSingle(this Half value) => (float)value;
}

/// <summary>
/// </summary>
public static class ModernTimeOnlyExtensions
{
    /// <summary>
    ///     Determines whether the specified start is between.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <param name="inclusive">if set to <c>true</c> [inclusive].</param>
    /// <returns>
    ///     <c>true</c> if the specified start is between; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsBetween(this TimeOnly value, TimeOnly start, TimeOnly end, bool inclusive = true)
    {
        return start <= end
            ? inclusive ? value >= start && value <= end : value > start && value < end
            : inclusive
                ? value >= start || value <= end
                : value > start || value < end;
    }

    /// <summary>
    ///     Tries the parse time only.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <param name="provider">The provider.</param>
    /// <param name="style">The style.</param>
    /// <returns></returns>
    public static bool TryParseTimeOnly(this ReadOnlySpan<char> value, out TimeOnly result, IFormatProvider? provider = null, DateTimeStyles style = DateTimeStyles.None) => TimeOnly.TryParse(value, provider, style, out result);

    /// <summary>
    ///     Untils the specified other.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns></returns>
    public static TimeSpan Until(this TimeOnly value, TimeOnly other)
    {
        TimeSpan result = other - value;
        return result < TimeSpan.Zero ? result + TimeSpan.FromDays(1) : result;
    }
}

/// <summary>
/// </summary>
public static class ModernPriorityQueueExtensions
{
    /// <summary>
    ///     Enqueues the range.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <typeparam name="TPriority">The type of the priority.</typeparam>
    /// <param name="queue">The queue.</param>
    /// <param name="items">The items.</param>
    /// <exception cref="System.ArgumentNullException">
    ///     queue
    ///     or
    ///     items
    /// </exception>
    public static void EnqueueRange<TElement, TPriority>(this PriorityQueue<TElement, TPriority> queue, IEnumerable<(TElement Element, TPriority Priority)> items)
    {
        if (queue == null) throw new ArgumentNullException(nameof(queue));
        if (items == null) throw new ArgumentNullException(nameof(items));
        foreach ((TElement Element, TPriority Priority) item in items) queue.Enqueue(item.Element, item.Priority);
    }

    /// <summary>
    ///     Tries the dequeue.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    /// <typeparam name="TPriority">The type of the priority.</typeparam>
    /// <param name="queue">The queue.</param>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">queue</exception>
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
/// <summary>
/// </summary>
public static class ModernGenericMathExtensions
{
    /// <summary>
    ///     Tries the parse number.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <param name="provider">The provider.</param>
    /// <returns></returns>
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

    /// <summary>
    ///     Parses the or default.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="provider">The provider.</param>
    /// <returns></returns>
    public static T ParseOrDefault<T>(this string? value, T defaultValue = default!, IFormatProvider? provider = null) where T : IParsable<T>
    {
        return T.TryParse(value, provider, out T? result) ? result : defaultValue;
    }

    /// <summary>
    ///     Determines whether the specified value is zero.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     <c>true</c> if the specified value is zero; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsZero(this Int128 value) => value == Int128.Zero;

    /// <summary>
    ///     Determines whether the specified value is zero.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     <c>true</c> if the specified value is zero; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsZero(this UInt128 value) => value == UInt128.Zero;

    /// <summary>
    ///     Converts to invariantstring.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToInvariantString(this Int128 value) => value.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    ///     Converts to invariantstring.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToInvariantString(this UInt128 value) => value.ToString(CultureInfo.InvariantCulture);
}
#endif

#if NET8_0_OR_GREATER
/// <summary>
/// </summary>
public static class ModernNet8TypeExtensions
{
    /// <summary>
    ///     Converts to searchvalues.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">nameof(values)</exception>
    public static SearchValues<char> ToSearchValues(this string values)
    {
        if (values == null) throw new ArgumentNullException(nameof(values));
        return SearchValues.Create(values);
    }

    /// <summary>
    ///     Converts to searchvalues.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns></returns>
    public static SearchValues<char> ToSearchValues(this ReadOnlySpan<char> values) => SearchValues.Create(values);

    /// <summary>
    ///     Determines whether the specified value contains any.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="searchValues">The search values.</param>
    /// <returns>
    ///     <c>true</c> if the specified value contains any; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">nameof(searchValues)</exception>
    public static bool ContainsAny(this ReadOnlySpan<char> value, SearchValues<char> searchValues)
    {
        if (searchValues == null) throw new ArgumentNullException(nameof(searchValues));
        return value.IndexOfAny(searchValues) >= 0;
    }

    /// <summary>
    ///     Converts to frozendictionarysafe.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">nameof(source)</exception>
    public static FrozenDictionary<TKey, TValue> ToFrozenDictionarySafe<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey>? comparer = null) where TKey : notnull
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.ToFrozenDictionary(comparer);
    }

    /// <summary>
    ///     Converts to frozensetsafe.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">nameof(source)</exception>
    public static FrozenSet<T> ToFrozenSetSafe<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.ToFrozenSet(comparer);
    }

    /// <summary>
    ///     Gets the UTC now offset.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">nameof(timeProvider)</exception>
    public static DateTimeOffset GetUtcNowOffset(this TimeProvider timeProvider)
    {
        if (timeProvider == null) throw new ArgumentNullException(nameof(timeProvider));
        return timeProvider.GetUtcNow();
    }

    /// <summary>
    ///     Delays the asynchronous.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    /// <param name="delay">The delay.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">nameof(timeProvider)</exception>
    public static Task DelayAsync(this TimeProvider timeProvider, TimeSpan delay, CancellationToken cancellationToken = default)
    {
        if (timeProvider == null) throw new ArgumentNullException(nameof(timeProvider));
        return Task.Delay(delay, timeProvider, cancellationToken);
    }
}
#endif