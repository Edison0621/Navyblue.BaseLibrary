// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : FileOutbox.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="FileOutbox.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Text.Json;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     File-based outbox that persists domain events as JSON for later drain/dispatch.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Cqrs.IOutbox" />
/// <seealso cref="Navyblue.Foundation.Cqrs.IOutboxDrain" />
public class FileOutbox : IOutbox, IOutboxDrain
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly string _basePath;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileOutbox" /> class.
    /// </summary>
    /// <param name="basePath">The base path.</param>
    public FileOutbox(string? basePath = null)
    {
        this._basePath = basePath ?? Path.Combine(AppContext.BaseDirectory, "outbox");
        Directory.CreateDirectory(this._basePath);
    }

    #region IOutbox Members

    /// <summary>
    ///     Saves the asynchronous.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <returns></returns>
    public Task SaveAsync(Event @event)
    {
        if (@event == null) return Task.CompletedTask;
        string name = Guid.NewGuid().ToString("N") + ".json";
        string path = Path.Combine(this._basePath, name);
        OutboxEnvelope envelope = new OutboxEnvelope(@event.GetType().AssemblyQualifiedName!, JsonSerializer.Serialize(@event, @event.GetType(), _jsonOptions));
        File.WriteAllText(path, JsonSerializer.Serialize(envelope, _jsonOptions));
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Saves the asynchronous.
    /// </summary>
    /// <param name="events">The events.</param>
    public async Task SaveAsync(IEnumerable<Event> events)
    {
        if (events == null) return;
        foreach (Event e in events)
            await this.SaveAsync(e);
    }

    #endregion

    #region IOutboxDrain Members

    /// <summary>
    ///     Drains this instance.
    /// </summary>
    /// <returns></returns>
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
                OutboxEnvelope? envelope = JsonSerializer.Deserialize<OutboxEnvelope>(json, _jsonOptions);
                if (envelope?.TypeName != null)
                {
                    Type? type = Type.GetType(envelope.TypeName);
                    if (type != null)
                        evt = (Event?)JsonSerializer.Deserialize(envelope.Payload, type, _jsonOptions);
                }
            }
            catch
            {
                // ignored
            }

            if (evt != null)
            {
                yield return evt;
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    /* ignored */
                }
            }
        }
    }

    #endregion

    #region Nested type: OutboxEnvelope

    /// <summary>
    /// </summary>
    private sealed record OutboxEnvelope(string TypeName, string Payload);

    #endregion
}