// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernWebDataExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="ModernWebDataExtensions.cs" company="">
//     Copyright (c) 2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Text;
using System.Text.Json;

namespace Navyblue.BaseLibrary.Extensions;

public static class ModernUriExtensions
{
    public static Uri AddQueryParameter(this Uri uri, string name, string? value)
    {
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Query parameter name cannot be empty.", nameof(name));
        return uri.AddQueryParameters(new[] { new KeyValuePair<string, string?>(name, value) });
    }

    public static Uri AddQueryParameters(this Uri uri, IEnumerable<KeyValuePair<string, string?>> parameters)
    {
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        if (parameters == null) throw new ArgumentNullException(nameof(parameters));

        UriBuilder builder = new UriBuilder(uri);
        List<KeyValuePair<string, string?>> query = ParseQuery(builder.Query).ToList();
        query.AddRange(parameters.Where(p => !string.IsNullOrWhiteSpace(p.Key)));
        builder.Query = BuildQuery(query);
        return builder.Uri;
    }

    public static IReadOnlyDictionary<string, string?> GetQueryParameters(this Uri uri, StringComparer? comparer = null)
    {
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        Dictionary<string, string?> result = new Dictionary<string, string?>(comparer ?? StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, string?> item in ParseQuery(uri.Query))
        {
            result[item.Key] = item.Value;
        }

        return result;
    }

    private static IEnumerable<KeyValuePair<string, string?>> ParseQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) yield break;
        string source = query[0] == '?' ? query.Substring(1) : query;
        foreach (string part in source.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            int index = part.IndexOf('=');
            string key = index < 0 ? Uri.UnescapeDataString(part) : Uri.UnescapeDataString(part.Substring(0, index));
            string? value = index < 0 ? null : Uri.UnescapeDataString(part.Substring(index + 1));
            if (!string.IsNullOrWhiteSpace(key)) yield return new KeyValuePair<string, string?>(key, value);
        }
    }

    private static string BuildQuery(IEnumerable<KeyValuePair<string, string?>> parameters)
    {
        StringBuilder builder = new StringBuilder();
        foreach (KeyValuePair<string, string?> parameter in parameters)
        {
            if (builder.Length > 0) builder.Append('&');
            builder.Append(Uri.EscapeDataString(parameter.Key));
            if (parameter.Value is not null)
            {
                builder.Append('=').Append(Uri.EscapeDataString(parameter.Value));
            }
        }

        return builder.ToString();
    }
}

public static class ModernJsonElementExtensions
{
    public static bool TryGetProperty(this JsonElement element, string propertyName, out JsonElement value, StringComparison comparison)
    {
        if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
        if (element.ValueKind != JsonValueKind.Object)
        {
            value = default;
            return false;
        }

        foreach (JsonProperty property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, comparison))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    public static string? GetStringOrDefault(this JsonElement element, string propertyName, string? defaultValue = null, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        return element.TryGetProperty(propertyName, out JsonElement value, comparison) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : defaultValue;
    }

    public static int GetInt32OrDefault(this JsonElement element, string propertyName, int defaultValue = default, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        return element.TryGetProperty(propertyName, out JsonElement value, comparison) && value.TryGetInt32(out int result)
            ? result
            : defaultValue;
    }

    public static bool GetBooleanOrDefault(this JsonElement element, string propertyName, bool defaultValue = default, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        return element.TryGetProperty(propertyName, out JsonElement value, comparison) && value.ValueKind is JsonValueKind.True or JsonValueKind.False
            ? value.GetBoolean()
            : defaultValue;
    }
}

public static class ModernJsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions ConfigureWebDefaults(this JsonSerializerOptions options, bool writeIndented = false)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.PropertyNameCaseInsensitive = true;
        options.WriteIndented = writeIndented;
        return options;
    }

    public static JsonSerializerOptions Clone(this JsonSerializerOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        return new JsonSerializerOptions(options);
    }
}

public static class ModernExceptionExtensions
{
    public static Exception GetRootException(this Exception exception)
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        while (exception.InnerException is not null) exception = exception.InnerException;
        return exception;
    }

    public static IEnumerable<Exception> EnumerateExceptionChain(this Exception exception)
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            yield return current;
        }
    }

    public static string GetMessageChain(this Exception exception, string separator = " -> ")
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        return string.Join(separator, exception.EnumerateExceptionChain().Select(x => x.Message).Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public static bool IsCausedBy<TException>(this Exception exception) where TException : Exception
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        return exception.EnumerateExceptionChain().Any(static x => x is TException);
    }
}