// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernCollectionExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:00
// ****************************************************************************************************************************************
// <copyright file="ModernCollectionExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public static class ModernEnumerableExtensions
{
    /// <summary>
    ///     Determines whether [is null or empty].
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <returns>
    ///     <c>true</c> if [is null or empty] [the specified source]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source) => source == null || !source.Any();

    /// <summary>
    ///     Chunks the by.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="size">The size.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">source</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">size</exception>
    public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int size)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
        return source.Chunk(size);
    }

    /// <summary>
    ///     Converts to hashset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">source</exception>
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return new HashSet<T>(source, comparer);
    }

    /// <summary>
    ///     Wheres the not null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">source</exception>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return source.Where(x => x != null)!;
    }
}

/// <summary>
/// </summary>
public static class ModernDictionaryExtensions
{
    /// <summary>
    ///     Gets the or add.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="factory">The factory.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     dictionary
    ///     or
    ///     factory
    /// </exception>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory) where TKey : notnull
    {
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
        if (factory == null) throw new ArgumentNullException(nameof(factory));
        if (dictionary.TryGetValue(key, out TValue? value)) return value;
        value = factory(key);
        dictionary[key] = value;
        return value;
    }

    /// <summary>
    ///     Gets the value or default.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">dictionary</exception>
    public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default) where TKey : notnull
    {
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
        return dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;
    }

    /// <summary>
    ///     Tries the remove.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">dictionary</exception>
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

    /// <summary>
    ///     Merges the specified second.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="first">The first.</param>
    /// <param name="second">The second.</param>
    /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     first
    ///     or
    ///     second
    /// </exception>
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