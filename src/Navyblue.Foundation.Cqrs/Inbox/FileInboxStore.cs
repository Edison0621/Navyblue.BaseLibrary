// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : FileInboxStore.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="FileInboxStore.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using System.Text.Json;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     File-based inbox store for request idempotency.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Cqrs.IInboxStore" />
public class FileInboxStore : IInboxStore
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly string _basePath;
    private readonly ConcurrentDictionary<string, byte> _processed = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileInboxStore" /> class.
    /// </summary>
    /// <param name="basePath">The base path.</param>
    public FileInboxStore(string? basePath = null)
    {
        this._basePath = basePath ?? Path.Combine(AppContext.BaseDirectory, "inbox");
        Directory.CreateDirectory(this._basePath);
    }

    #region IInboxStore Members

    /// <summary>
    ///     Determines whether [is processed asynchronous] [the specified request identifier].
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <returns></returns>
    public Task<bool> IsProcessedAsync(string requestId)
    {
        if (string.IsNullOrEmpty(requestId)) return Task.FromResult(false);
        if (this._processed.ContainsKey(requestId)) return Task.FromResult(true);
        return Task.FromResult(File.Exists(this.GetMarkerPath(requestId)));
    }

    /// <summary>
    ///     Marks the processed asynchronous.
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <returns></returns>
    public Task MarkProcessedAsync(string requestId)
    {
        if (string.IsNullOrEmpty(requestId)) return Task.CompletedTask;
        this._processed[requestId] = 0;
        File.WriteAllText(this.GetMarkerPath(requestId), DateTime.UtcNow.ToString("O"));
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Tries the get response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="requestId">The request identifier.</param>
    /// <param name="response">The response.</param>
    /// <returns></returns>
    public Task<bool> TryGetResponse<TResponse>(string requestId, out TResponse response)
    {
        response = default!;
        string path = this.GetResponsePath(requestId);
        if (!File.Exists(path)) return Task.FromResult(false);
        try
        {
            string json = File.ReadAllText(path);
            TResponse? deserialized = JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
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

    /// <summary>
    ///     Sets the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="requestId">The request identifier.</param>
    /// <param name="response">The response.</param>
    /// <returns></returns>
    public Task SetResponse<TResponse>(string requestId, TResponse response)
    {
        if (string.IsNullOrEmpty(requestId)) return Task.CompletedTask;
        string json = JsonSerializer.Serialize(response, _jsonOptions);
        File.WriteAllText(this.GetResponsePath(requestId), json);
        return Task.CompletedTask;
    }

    #endregion

    /// <summary>
    ///     Gets the marker path.
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <returns></returns>
    private string GetMarkerPath(string requestId) => Path.Combine(this._basePath, requestId + ".done");

    /// <summary>
    ///     Gets the response path.
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <returns></returns>
    private string GetResponsePath(string requestId) => Path.Combine(this._basePath, requestId + ".json");
}