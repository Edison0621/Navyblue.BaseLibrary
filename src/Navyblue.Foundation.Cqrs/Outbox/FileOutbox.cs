using System.Text.Json;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     File-based outbox that persists domain events as JSON for later drain/dispatch.
/// </summary>
public class FileOutbox : IOutbox, IOutboxDrain
{
    private readonly string _basePath;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public FileOutbox(string? basePath = null)
    {
        this._basePath = basePath ?? Path.Combine(AppContext.BaseDirectory, "outbox");
        Directory.CreateDirectory(this._basePath);
    }

    public Task SaveAsync(Event @event)
    {
        if (@event == null) return Task.CompletedTask;
        string name = Guid.NewGuid().ToString("N") + ".json";
        string path = Path.Combine(this._basePath, name);
        var envelope = new OutboxEnvelope(@event.GetType().AssemblyQualifiedName!, JsonSerializer.Serialize(@event, @event.GetType(), JsonOptions));
        File.WriteAllText(path, JsonSerializer.Serialize(envelope, JsonOptions));
        return Task.CompletedTask;
    }

    public async Task SaveAsync(IEnumerable<Event> events)
    {
        if (events == null) return;
        foreach (Event e in events)
            await this.SaveAsync(e);
    }

    public IEnumerable<Event> Drain()
    {
        if (!Directory.Exists(this._basePath)) yield break;
        string[] files = Directory.GetFiles(this._basePath, "*.json", SearchOption.TopDirectoryOnly);
        foreach (string file in files)
        {
            Event? evt = null;
            try
            {
                string json = File.ReadAllText(file);
                OutboxEnvelope? envelope = JsonSerializer.Deserialize<OutboxEnvelope>(json, JsonOptions);
                if (envelope?.TypeName != null)
                {
                    Type? type = Type.GetType(envelope.TypeName);
                    if (type != null)
                        evt = (Event?)JsonSerializer.Deserialize(envelope.Payload, type, JsonOptions);
                }
            }
            catch
            {
                // ignored
            }

            if (evt != null)
            {
                yield return evt;
                try { File.Delete(file); }
                catch { /* ignored */ }
            }
        }
    }

    private sealed record OutboxEnvelope(string TypeName, string Payload);
}
