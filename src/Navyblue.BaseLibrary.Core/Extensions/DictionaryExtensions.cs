// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : DictionaryExtensions.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-29  13:02
// ****************************************************************************************************************************************
// <copyright file="DictionaryExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
///     The dictionary extensions.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    ///     Get or add.
    /// </summary>
    /// <typeparam name="TKey" />
    /// <typeparam name="TValue" />
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="factory">The factory.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A <typeparamref name="TValue" /></returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(factory);

        if (dictionary.TryGetValue(key, out TValue? value))
        {
            return value;
        }

        value = factory(key);
        dictionary[key] = value;
        return value;
    }

    /// <summary>
    ///     Get value or default.
    /// </summary>
    /// <typeparam name="TKey" />
    /// <typeparam name="TValue" />
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A <typeparamref name="TValue" /></returns>
    public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        return dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;
    }

    /// <summary>
    ///     Try remove.
    /// </summary>
    /// <typeparam name="TKey" />
    /// <typeparam name="TValue" />
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A bool</returns>
    public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue? value) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        if (dictionary.TryGetValue(key, out value))
        {
            dictionary.Remove(key);
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    ///     Merges and return a dictionary with a key of type tkey and a value of type tvalues.
    /// </summary>
    /// <typeparam name="TKey" />
    /// <typeparam name="TValue" />
    /// <param name="first">The first.</param>
    /// <param name="second">The second.</param>
    /// <param name="overwrite">If true, overwrite.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><![CDATA[Dictionary<TKey, TValue>]]></returns>
    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> first, IEnumerable<KeyValuePair<TKey, TValue>> second, bool overwrite = true) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
        foreach (KeyValuePair<TKey, TValue> pair in first)
        {
            result[pair.Key] = pair.Value;
        }

        foreach (KeyValuePair<TKey, TValue> pair in second)
        {
            if (overwrite || !result.ContainsKey(pair.Key))
            {
                result[pair.Key] = pair.Value;
            }
        }

        return result;
    }
}