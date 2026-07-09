// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernCollectionDiffExtensions.cs
// Created          : 2026-06-30
// ****************************************************************************************************************************************

#nullable enable
namespace Navyblue.BaseLibrary.Extensions;

public sealed record CollectionDiff<T>(IReadOnlyList<T> Added, IReadOnlyList<T> Removed, IReadOnlyList<T> Unchanged);

public sealed record CollectionDiffByKey<TLeft, TRight, TKey>(
    IReadOnlyList<TRight> Added,
    IReadOnlyList<TLeft> Removed,
    IReadOnlyList<(TLeft Left, TRight Right)> Matched)
    where TKey : notnull;

public static class ModernCollectionDiffExtensions
{
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
