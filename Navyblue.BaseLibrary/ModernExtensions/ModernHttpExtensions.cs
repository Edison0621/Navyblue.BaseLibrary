// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernHttpExtensions.cs
// Created          : 2026-06-30
// ****************************************************************************************************************************************

#nullable enable
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Navyblue.BaseLibrary.Extensions;

public static class ModernHttpRequestMessageExtensions
{
    public static HttpRequestMessage WithHeader(this HttpRequestMessage request, string name, string? value)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Header name cannot be blank.", nameof(name));
        if (!string.IsNullOrEmpty(value))
        {
            request.Headers.TryAddWithoutValidation(name, value);
        }

        return request;
    }

    public static HttpRequestMessage WithBearerToken(this HttpRequestMessage request, string? token)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return request;
    }

    public static HttpRequestMessage WithJsonAccept(this HttpRequestMessage request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return request;
    }

    public static HttpRequestMessage WithCorrelationId(this HttpRequestMessage request, string correlationId, string headerName = "X-Correlation-Id")
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(correlationId)) throw new ArgumentException("Correlation id cannot be blank.", nameof(correlationId));
        return request.WithHeader(headerName, correlationId);
    }
}

public static class ModernHttpResponseMessageExtensions
{
    public static bool IsSuccess(this HttpResponseMessage response)
    {
        if (response == null) throw new ArgumentNullException(nameof(response));
        return response.IsSuccessStatusCode;
    }

    public static async Task<T?> ReadJsonOrDefaultAsync<T>(this HttpResponseMessage response, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (response == null) throw new ArgumentNullException(nameof(response));
        if (response.Content == null || response.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(options, cancellationToken).ConfigureAwait(false);
    }

    public static async Task<string> ReadStringOrEmptyAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response == null) throw new ArgumentNullException(nameof(response));
        if (response.Content == null)
        {
            return string.Empty;
        }

#if NET8_0_OR_GREATER
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
        cancellationToken.ThrowIfCancellationRequested();
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
    }

    public static async Task<HttpResponseMessage> EnsureSuccessWithBodyAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response == null) throw new ArgumentNullException(nameof(response));
        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        string body = await response.ReadStringOrEmptyAsync(cancellationToken).ConfigureAwait(false);
        string message = string.IsNullOrWhiteSpace(body)
            ? $"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase})."
            : $"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}). Body: {body}";

        throw new HttpRequestException(message, null, response.StatusCode);
    }
}

public static class ModernHttpClientExtensions
{
    public static async Task<T?> GetJsonOrDefaultAsync<T>(this HttpClient client, string requestUri, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        using HttpResponseMessage response = await client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        await response.EnsureSuccessWithBodyAsync(cancellationToken).ConfigureAwait(false);
        return await response.ReadJsonOrDefaultAsync<T>(options, cancellationToken).ConfigureAwait(false);
    }

    public static async Task<TResponse?> PostJsonForAsync<TRequest, TResponse>(this HttpClient client, string requestUri, TRequest value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        using HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, value, options, cancellationToken).ConfigureAwait(false);
        await response.EnsureSuccessWithBodyAsync(cancellationToken).ConfigureAwait(false);
        return await response.ReadJsonOrDefaultAsync<TResponse>(options, cancellationToken).ConfigureAwait(false);
    }
}
