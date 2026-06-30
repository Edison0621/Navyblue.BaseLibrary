// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernCollectionExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="ModernCollectionExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
namespace Navyblue.BaseLibrary.Extensions;

public static class ModernEnumerableExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source) => source == null || !source.Any();

    public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int size)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
        return source.Chunk(size);
    }

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return new HashSet<T>(source, comparer);
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Where(x => x != null)!;
    }
}

public static class ModernDictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory) where TKey : notnull
    {
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
        if (factory == null) throw new ArgumentNullException(nameof(factory));
        if (dictionary.TryGetValue(key, out TValue? value)) return value;
        value = factory(key);
        dictionary[key] = value;
        return value;
    }

    public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default) where TKey : notnull
    {
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
        return dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;
    }

    public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue? value) where TKey : notnull
    {
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
        if (dictionary.Remove(key, out value))
        {
            return true;
        }

        value = default;
        return false;
    }

    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> first, IEnumerable<KeyValuePair<TKey, TValue>> second, bool overwrite = true) where TKey : notnull
    {
        if (first == null) throw new ArgumentNullException(nameof(first));
        if (second == null) throw new ArgumentNullException(nameof(second));
        Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
        foreach (KeyValuePair<TKey, TValue> pair in first) result[pair.Key] = pair.Value;
        foreach (KeyValuePair<TKey, TValue> pair in second)
        {
            if (overwrite || !result.ContainsKey(pair.Key)) result[pair.Key] = pair.Value;
        }

        return result;
    }
}