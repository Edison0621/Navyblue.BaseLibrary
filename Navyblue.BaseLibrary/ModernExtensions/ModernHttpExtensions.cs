// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernHttpExtensions.cs
// Created          : 2026-06-30  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:00
// ****************************************************************************************************************************************
// <copyright file="ModernHttpExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public static class ModernHttpRequestMessageExtensions
{
    /// <summary>
    ///     Withes the header.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">request</exception>
    /// <exception cref="System.ArgumentException">Header name cannot be blank. - name</exception>
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

    /// <summary>
    ///     Withes the bearer token.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="token">The token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">request</exception>
    public static HttpRequestMessage WithBearerToken(this HttpRequestMessage request, string? token)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return request;
    }

    /// <summary>
    ///     Withes the json accept.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">request</exception>
    public static HttpRequestMessage WithJsonAccept(this HttpRequestMessage request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return request;
    }

    /// <summary>
    ///     Withes the correlation identifier.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="correlationId">The correlation identifier.</param>
    /// <param name="headerName">Name of the header.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">request</exception>
    /// <exception cref="System.ArgumentException">Correlation id cannot be blank. - correlationId</exception>
    public static HttpRequestMessage WithCorrelationId(this HttpRequestMessage request, string correlationId, string headerName = "X-Correlation-Id")
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(correlationId)) throw new ArgumentException("Correlation id cannot be blank.", nameof(correlationId));
        return request.WithHeader(headerName, correlationId);
    }
}

/// <summary>
/// </summary>
public static class ModernHttpResponseMessageExtensions
{
    /// <summary>
    ///     Determines whether this instance is success.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <returns>
    ///     <c>true</c> if the specified response is success; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">response</exception>
    public static bool IsSuccess(this HttpResponseMessage response)
    {
        if (response == null) throw new ArgumentNullException(nameof(response));
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    ///     Reads the json or default asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response">The response.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">response</exception>
    public static async Task<T?> ReadJsonOrDefaultAsync<T>(this HttpResponseMessage response, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (response == null) throw new ArgumentNullException(nameof(response));
        if (response.Content == null || response.StatusCode == HttpStatusCode.NoContent)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(options, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Reads the string or empty asynchronous.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">response</exception>
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
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#endif
    }

    /// <summary>
    ///     Ensures the success with body asynchronous.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">response</exception>
    /// <exception cref="System.Net.Http.HttpRequestException">null</exception>
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

/// <summary>
/// </summary>
public static class ModernHttpClientExtensions
{
    /// <summary>
    ///     Gets the json or default asynchronous.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client">The client.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">client</exception>
    public static async Task<T?> GetJsonOrDefaultAsync<T>(this HttpClient client, string requestUri, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        using HttpResponseMessage response = await client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        await response.EnsureSuccessWithBodyAsync(cancellationToken).ConfigureAwait(false);
        return await response.ReadJsonOrDefaultAsync<T>(options, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Posts the json for asynchronous.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="client">The client.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="value">The value.</param>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">client</exception>
    public static async Task<TResponse?> PostJsonForAsync<TRequest, TResponse>(this HttpClient client, string requestUri, TRequest value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        using HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, value, options, cancellationToken).ConfigureAwait(false);
        await response.EnsureSuccessWithBodyAsync(cancellationToken).ConfigureAwait(false);
        return await response.ReadJsonOrDefaultAsync<TResponse>(options, cancellationToken).ConfigureAwait(false);
    }
}