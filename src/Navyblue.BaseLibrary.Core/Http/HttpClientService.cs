using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Navyblue.BaseLibrary.Http;

public sealed class HttpClientOptions { public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30); public int RetryCount { get; set; } = 2; public IDictionary<string, string> DefaultHeaders { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); }
public interface IHttpClientService { Task<T?> GetFromJsonAsync<T>(string clientName, string requestUri, CancellationToken cancellationToken = default); Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(string clientName, string requestUri, TRequest request, CancellationToken cancellationToken = default); Task<HttpResponseMessage> SendAsync(string clientName, HttpRequestMessage request, CancellationToken cancellationToken = default); }
public sealed class HttpClientService(IHttpClientFactory httpClientFactory, IOptions<HttpClientOptions> options) : IHttpClientService
{
    public async Task<T?> GetFromJsonAsync<T>(string clientName, string requestUri, CancellationToken cancellationToken = default) { using var response = await SendAsync(clientName, new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken).ConfigureAwait(false); response.EnsureSuccessStatusCode(); return await response.Content.ReadFromJsonAsync<T>(cancellationToken).ConfigureAwait(false); }
    public async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(string clientName, string requestUri, TRequest request, CancellationToken cancellationToken = default) { using var message = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = JsonContent.Create(request) }; using var response = await SendAsync(clientName, message, cancellationToken).ConfigureAwait(false); response.EnsureSuccessStatusCode(); return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken).ConfigureAwait(false); }
    public async Task<HttpResponseMessage> SendAsync(string clientName, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(clientName); client.Timeout = options.Value.Timeout;
        foreach (var header in options.Value.DefaultHeaders) request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        var attempts = Math.Max(1, options.Value.RetryCount + 1);
        for (var attempt = 1; ; attempt++) { try { return await client.SendAsync(request, cancellationToken).ConfigureAwait(false); } catch when (attempt < attempts) { await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt), cancellationToken).ConfigureAwait(false); } }
    }
}
public static class HttpServiceCollectionExtensions { public static IServiceCollection AddNavyblueHttp(this IServiceCollection services, Action<HttpClientOptions>? configure = null) { services.AddHttpClient(); if (configure is not null) services.Configure(configure); else services.AddOptions<HttpClientOptions>(); services.AddSingleton<IHttpClientService, HttpClientService>(); return services; } }
