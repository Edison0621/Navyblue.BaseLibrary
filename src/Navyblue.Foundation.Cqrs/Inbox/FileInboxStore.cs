using System.Collections.Concurrent;
using System.Text.Json;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     File-based inbox store for request idempotency.
/// </summary>
public class FileInboxStore : IInboxStore
{
    private readonly string _basePath;
    private readonly ConcurrentDictionary<string, byte> _processed = new();
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public FileInboxStore(string? basePath = null)
    {
        this._basePath = basePath ?? Path.Combine(AppContext.BaseDirectory, "inbox");
        Directory.CreateDirectory(this._basePath);
    }

    public Task<bool> IsProcessedAsync(string requestId)
    {
        if (string.IsNullOrEmpty(requestId)) return Task.FromResult(false);
        if (this._processed.ContainsKey(requestId)) return Task.FromResult(true);
        return Task.FromResult(File.Exists(this.GetMarkerPath(requestId)));
    }

    public Task MarkProcessedAsync(string requestId)
    {
        if (string.IsNullOrEmpty(requestId)) return Task.CompletedTask;
        this._processed[requestId] = 0;
        File.WriteAllText(this.GetMarkerPath(requestId), DateTime.UtcNow.ToString("O"));
        return Task.CompletedTask;
    }

    public Task<bool> TryGetResponse<TResponse>(string requestId, out TResponse response)
    {
        response = default!;
        string path = this.GetResponsePath(requestId);
        if (!File.Exists(path)) return Task.FromResult(false);
        try
        {
            string json = File.ReadAllText(path);
            TResponse? deserialized = JsonSerializer.Deserialize<TResponse>(json, JsonOptions);
            if (deserialized is null && !typeof(TResponse).IsValueType)
                return Task.FromResult(false);
            response = deserialized!;
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task SetResponse<TResponse>(string requestId, TResponse response)
    {
        if (string.IsNullOrEmpty(requestId)) return Task.CompletedTask;
        string json = JsonSerializer.Serialize(response, JsonOptions);
        File.WriteAllText(this.GetResponsePath(requestId), json);
        return Task.CompletedTask;
    }

    private string GetMarkerPath(string requestId) => Path.Combine(this._basePath, requestId + ".done");
    private string GetResponsePath(string requestId) => Path.Combine(this._basePath, requestId + ".json");
}
