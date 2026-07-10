// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernCollectionDiffExtensions.cs
// Created          : 2026-06-30  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:04
// ****************************************************************************************************************************************
// <copyright file="ModernCollectionDiffExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed record CollectionDiff<T>(IReadOnlyList<T> Added, IReadOnlyList<T> Removed, IReadOnlyList<T> Unchanged);

/// <summary>
/// </summary>
/// <typeparam name="TLeft">The type of the left.</typeparam>
/// <typeparam name="TRight">The type of the right.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
public sealed record CollectionDiffByKey<TLeft, TRight, TKey>(
    IReadOnlyList<TRight> Added,
    IReadOnlyList<TLeft> Removed,
    IReadOnlyList<(TLeft Left, TRight Right)> Matched)
    where TKey : notnull;

/// <summary>
/// </summary>
public static class ModernCollectionDiffExtensions
{
    /// <summary>
    ///     Differences the specified next.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="current">The current.</param>
    /// <param name="next">The next.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     current
    ///     or
    ///     next
    /// </exception>
    public static CollectionDiff<T> Diff<T>(this IEnumerable<T> current, IEnumerable<T> next, IEqualityComparer<T>? comparer = null)
    {
        if (current == null) throw new ArgumentNullException(nameof(current));
        if (next == null) throw new ArgumentNullException(nameof(next));

        comparer ??= EqualityComparer<T>.Default;
        HashSet<T> currentSet = new(current, comparer);
        HashSet<T> nextSet = new(next, comparer);

        List<T> added = nextSet.Where(item => !currentSet.Contains(item)).ToList();
        List<T> removed = currentSet.Where(item => !nextSet.Contains(item)).ToList();
        List<T> unchanged = currentSet.Where(nextSet.Contains).ToList();
        return new CollectionDiff<T>(added, removed, unchanged);
    }

    /// <summary>
    ///     Differences the by.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left.</typeparam>
    /// <typeparam name="TRight">The type of the right.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="current">The current.</param>
    /// <param name="next">The next.</param>
    /// <param name="currentKeySelector">The current key selector.</param>
    /// <param name="nextKeySelector">The next key selector.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     current
    ///     or
    ///     next
    ///     or
    ///     currentKeySelector
    ///     or
    ///     nextKeySelector
    /// </exception>
    public static CollectionDiffByKey<TLeft, TRight, TKey> DiffBy<TLeft, TRight, TKey>(
        this IEnumerable<TLeft> current,
        IEnumerable<TRight> next,
        Func<TLeft, TKey> currentKeySelector,
        Func<TRight, TKey> nextKeySelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        if (current == null) throw new ArgumentNullException(nameof(current));
        if (next == null) throw new ArgumentNullException(nameof(next));
        if (currentKeySelector == null) throw new ArgumentNullException(nameof(currentKeySelector));
        if (nextKeySelector == null) throw new ArgumentNullException(nameof(nextKeySelector));

        comparer ??= EqualityComparer<TKey>.Default;
        Dictionary<TKey, TLeft> currentMap = current.ToDictionary(currentKeySelector, comparer);
        Dictionary<TKey, TRight> nextMap = next.ToDictionary(nextKeySelector, comparer);

        List<TRight> added = nextMap.Where(pair => !currentMap.ContainsKey(pair.Key)).Select(pair => pair.Value).ToList();
        List<TLeft> removed = currentMap.Where(pair => !nextMap.ContainsKey(pair.Key)).Select(pair => pair.Value).ToList();
        List<(TLeft Left, TRight Right)> matched = currentMap
            .Where(pair => nextMap.ContainsKey(pair.Key))
            .Select(pair => (pair.Value, nextMap[pair.Key]))
            .ToList();

        return new CollectionDiffByKey<TLeft, TRight, TKey>(added, removed, matched);
    }

    /// <summary>
    ///     Converts to dictionarylastwins.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="valueSelector">The value selector.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     source
    ///     or
    ///     keySelector
    ///     or
    ///     valueSelector
    /// </exception>
    public static Dictionary<TKey, TValue> ToDictionaryLastWins<TSource, TKey, TValue>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
        if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));

        Dictionary<TKey, TValue> dictionary = new(comparer);
        foreach (TSource item in source)
        {
            dictionary[keySelector(item)] = valueSelector(item);
        }

        return dictionary;
    }

    /// <summary>
    ///     Groups to dictionary.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     source
    ///     or
    ///     keySelector
    /// </exception>
    public static Dictionary<TKey, List<TSource>> GroupToDictionary<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

        Dictionary<TKey, List<TSource>> dictionary = new(comparer);
        foreach (TSource item in source)
        {
            TKey key = keySelector(item);
            if (!dictionary.TryGetValue(key, out List<TSource>? list))
            {
                list = new List<TSource>();
                dictionary[key] = list;
            }

            list.Add(item);
        }

        return dictionary;
    }
}