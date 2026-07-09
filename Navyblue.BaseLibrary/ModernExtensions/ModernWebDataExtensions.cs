// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernWebDataExtensions.cs
// Created          : 2026-06-30  15:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:00
// ****************************************************************************************************************************************
// <copyright file="ModernWebDataExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Text;
using System.Text.Json;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public static class ModernUriExtensions
{
    /// <summary>
    ///     Adds the query parameter.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">uri</exception>
    /// <exception cref="System.ArgumentException">Query parameter name cannot be empty. - name</exception>
    public static Uri AddQueryParameter(this Uri uri, string name, string? value)
    {
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Query parameter name cannot be empty.", nameof(name));
        return uri.AddQueryParameters(new[] { new KeyValuePair<string, string?>(name, value) });
    }

    /// <summary>
    ///     Adds the query parameters.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     uri
    ///     or
    ///     parameters
    /// </exception>
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

    /// <summary>
    ///     Gets the query parameters.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">uri</exception>
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

    /// <summary>
    ///     Parses the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns></returns>
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

    /// <summary>
    ///     Builds the query.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns></returns>
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

/// <summary>
/// </summary>
public static class ModernJsonElementExtensions
{
    /// <summary>
    ///     Tries the get property.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">propertyName</exception>
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

    /// <summary>
    ///     Gets the string or default.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns></returns>
    public static string? GetStringOrDefault(this JsonElement element, string propertyName, string? defaultValue = null, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        return element.TryGetProperty(propertyName, out JsonElement value, comparison) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : defaultValue;
    }

    /// <summary>
    ///     Gets the int32 or default.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns></returns>
    public static int GetInt32OrDefault(this JsonElement element, string propertyName, int defaultValue = default, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        return element.TryGetProperty(propertyName, out JsonElement value, comparison) && value.TryGetInt32(out int result)
            ? result
            : defaultValue;
    }

    /// <summary>
    ///     Gets the boolean or default.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns></returns>
    public static bool GetBooleanOrDefault(this JsonElement element, string propertyName, bool defaultValue = default, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        return element.TryGetProperty(propertyName, out JsonElement value, comparison) && value.ValueKind is JsonValueKind.True or JsonValueKind.False
            ? value.GetBoolean()
            : defaultValue;
    }
}

/// <summary>
/// </summary>
public static class ModernJsonSerializerOptionsExtensions
{
    /// <summary>
    ///     Configures the web defaults.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="writeIndented">if set to <c>true</c> [write indented].</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">options</exception>
    public static JsonSerializerOptions ConfigureWebDefaults(this JsonSerializerOptions options, bool writeIndented = false)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.PropertyNameCaseInsensitive = true;
        options.WriteIndented = writeIndented;
        return options;
    }

    /// <summary>
    ///     Clones the specified options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">options</exception>
    public static JsonSerializerOptions Clone(this JsonSerializerOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        return new JsonSerializerOptions(options);
    }
}

/// <summary>
/// </summary>
public static class ModernExceptionExtensions
{
    /// <summary>
    ///     Gets the root exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">exception</exception>
    public static Exception GetRootException(this Exception exception)
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        while (exception.InnerException is not null) exception = exception.InnerException;
        return exception;
    }

    /// <summary>
    ///     Enumerates the exception chain.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">nameof(exception)</exception>
    public static IEnumerable<Exception> EnumerateExceptionChain(this Exception exception)
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            yield return current;
        }
    }

    /// <summary>
    ///     Gets the message chain.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="separator">The separator.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">nameof(exception)</exception>
    public static string GetMessageChain(this Exception exception, string separator = " -> ")
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        return string.Join(separator, exception.EnumerateExceptionChain().Select(x => x.Message).Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    /// <summary>
    ///     Determines whether [is caused by] [the specified exception].
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <param name="exception">The exception.</param>
    /// <returns>
    ///     <c>true</c> if [is caused by] [the specified exception]; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">nameof(exception)</exception>
    public static bool IsCausedBy<TException>(this Exception exception) where TException : Exception
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        return exception.EnumerateExceptionChain().Any(static x => x is TException);
    }
}