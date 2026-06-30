// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : HttpClientService.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:51
// ****************************************************************************************************************************************
// <copyright file="HttpClientService.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Navyblue.BaseLibrary.Http;

/// <summary>
///     The http client options.
/// </summary>
public sealed class HttpClientOptions
{
    /// <summary>
    ///     Gets or sets the timeout.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Gets or sets the retry count.
    /// </summary>
    public int RetryCount { get; set; } = 2;

    /// <summary>
    ///     Gets the default headers.
    /// </summary>
    public IDictionary<string, string> DefaultHeaders { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}

/// <summary>
///     The http client service interface.
/// </summary>
public interface IHttpClientService
{
    /// <summary>
    ///     Get from json asynchronously.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="clientName">The client name.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[Task<T?>]]></returns>
    Task<T?> GetFromJsonAsync<T>(string clientName, string requestUri, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Post as json asynchronously.
    /// </summary>
    /// <typeparam name="TRequest" />
    /// <typeparam name="TResponse" />
    /// <param name="clientName">The client name.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[Task<TResponse?>]]></returns>
    Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(string clientName, string requestUri, TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Sends and return a task of type httpresponsemessage asynchronously.
    /// </summary>
    /// <param name="clientName">The client name.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[Task<HttpResponseMessage>]]></returns>
    Task<HttpResponseMessage> SendAsync(string clientName, HttpRequestMessage request, CancellationToken cancellationToken = default);
}

/// <summary>
///     The http client service.
/// </summary>
/// <param name="httpClientFactory">The http client factory.</param>
/// <param name="options">The options.</param>
public sealed class HttpClientService(IHttpClientFactory httpClientFactory, IOptions<HttpClientOptions> options) : IHttpClientService
{
    #region IHttpClientService Members

    /// <summary>
    ///     Get from json asynchronously.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="clientName">The client name.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[Task<T?>]]></returns>
    public async Task<T?> GetFromJsonAsync<T>(string clientName, string requestUri, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await this.SendAsync(clientName, new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Post as json asynchronously.
    /// </summary>
    /// <typeparam name="TRequest" />
    /// <typeparam name="TResponse" />
    /// <param name="clientName">The client name.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[Task<TResponse?>]]></returns>
    public async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(string clientName, string requestUri, TRequest request, CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = JsonContent.Create(request) };
        using HttpResponseMessage response = await this.SendAsync(clientName, message, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Sends and return a task of type httpresponsemessage asynchronously.
    /// </summary>
    /// <param name="clientName">The client name.</param>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[Task<HttpResponseMessage>]]></returns>
    public async Task<HttpResponseMessage> SendAsync(string clientName, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        HttpClient client = httpClientFactory.CreateClient(clientName);
        client.Timeout = options.Value.Timeout;
        foreach (KeyValuePair<string, string> header in options.Value.DefaultHeaders) request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        int attempts = Math.Max(1, options.Value.RetryCount + 1);
        for (int attempt = 1;; attempt++)
        {
            try
            {
                return await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch when (attempt < attempts)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt), cancellationToken).ConfigureAwait(false);
            }
        }
    }

    #endregion
}

/// <summary>
///     The http service collection extensions.
/// </summary>
public static class HttpServiceCollectionExtensions
{
    /// <summary>
    ///     Add navyblue http.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configure">The configure.</param>
    /// <returns>An IServiceCollection</returns>
    public static IServiceCollection AddNavyblueHttp(this IServiceCollection services, Action<HttpClientOptions>? configure = null)
    {
        services.AddHttpClient();
        if (configure is not null) services.Configure(configure);
        else services.AddOptions<HttpClientOptions>();
        services.AddSingleton<IHttpClientService, HttpClientService>();
        return services;
    }
}