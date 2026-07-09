// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : InMemoryIdempotencyStore.cs
// Created          : 2026-07-09  16:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  16:06
// ****************************************************************************************************************************************
// <copyright file="InMemoryIdempotencyStore.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Idempotency;

namespace Navyblue.Foundation.Testing;

/// <summary>
///     In-memory <see cref="IIdempotencyStore" /> for tests.
/// </summary>
public sealed class InMemoryIdempotencyStore(IClock? clock = null) : IIdempotencyStore
{
    private readonly ConcurrentDictionary<string, IdempotencyRecord> _records = new(StringComparer.Ordinal);
    private readonly IClock _clock = clock ?? new SystemClock();

    /// <summary>
    ///     Gets a snapshot of stored records.
    /// </summary>
    public IReadOnlyDictionary<string, IdempotencyRecord> Records => this._records;

    /// <inheritdoc />
    public ValueTask<bool> TryBeginAsync(IdempotencyKey key, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        this.PurgeExpired(key.Value);
        DateTimeOffset now = this._clock.UtcNow;
        IdempotencyRecord created = new(key, IdempotencyState.Processing, now, now.Add(ttl));
        bool started = this._records.TryAdd(key.Value, created);
        return ValueTask.FromResult(started);
    }

    /// <inheritdoc />
    public ValueTask CompleteAsync(IdempotencyKey key, string? responsePayload = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        this._records.AddOrUpdate(
            key.Value,
            _ => new IdempotencyRecord(key, IdempotencyState.Succeeded, this._clock.UtcNow, null, responsePayload),
            (_, existing) => existing with { State = IdempotencyState.Succeeded, ResponsePayload = responsePayload });
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask FailAsync(IdempotencyKey key, string? reason = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        this._records.AddOrUpdate(
            key.Value,
            _ => new IdempotencyRecord(key, IdempotencyState.Failed, this._clock.UtcNow, null, reason),
            (_, existing) => existing with { State = IdempotencyState.Failed, ResponsePayload = reason });
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask<IdempotencyRecord?> GetAsync(IdempotencyKey key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        this.PurgeExpired(key.Value);
        this._records.TryGetValue(key.Value, out IdempotencyRecord? record);
        return ValueTask.FromResult(record);
    }

    /// <summary>
    ///     Clears all records.
    /// </summary>
    public void Clear() => this._records.Clear();

    private void PurgeExpired(string key)
    {
        if (!this._records.TryGetValue(key, out IdempotencyRecord? record))
        {
            return;
        }

        if (record.ExpiresAt is { } expiresAt && expiresAt <= this._clock.UtcNow)
        {
            this._records.TryRemove(key, out _);
        }
    }
}

/// <summary>
///     Fake <see cref="IIdempotencyKeyProvider" /> that returns a fixed key.
/// </summary>
public sealed class FakeIdempotencyKeyProvider(string? key = "test-idempotency-key") : IIdempotencyKeyProvider
{
    /// <summary>
    ///     Gets or sets the key value returned by <see cref="GetKeyAsync" />.
    /// </summary>
    public string? Key { get; set; } = key;

    /// <inheritdoc />
    public ValueTask<IdempotencyKey?> GetKeyAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(string.IsNullOrWhiteSpace(this.Key) ? null : IdempotencyKey.From(this.Key));
}
