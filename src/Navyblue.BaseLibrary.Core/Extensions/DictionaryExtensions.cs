namespace Navyblue.BaseLibrary.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(factory);

        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        value = factory(key);
        dictionary[key] = value;
        return value;
    }

    public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }

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

    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> first, IEnumerable<KeyValuePair<TKey, TValue>> second, bool overwrite = true) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        var result = new Dictionary<TKey, TValue>();
        foreach (var pair in first)
        {
            result[pair.Key] = pair.Value;
        }

        foreach (var pair in second)
        {
            if (overwrite || !result.ContainsKey(pair.Key))
            {
                result[pair.Key] = pair.Value;
            }
        }

        return result;
    }
}
