// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : InMemoryDistributedLockProvider.cs
// Created          : 2026-07-09  16:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="InMemoryDistributedLockProvider.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Locking;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.Testing;

/// <summary>
///     In-memory <see cref="IDistributedLockProvider" /> for tests.
/// </summary>
public sealed class InMemoryDistributedLockProvider(IClock? clock = null) : IDistributedLockProvider
{
    private readonly IClock _clock = clock ?? new SystemClock();
    private readonly ConcurrentDictionary<string, LockState> _locks = new(StringComparer.Ordinal);

    #region IDistributedLockProvider Members

    /// <inheritdoc />
    public async ValueTask<IDistributedLock?> TryAcquireAsync(string name, TimeSpan expiry, TimeSpan waitTime, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(name, nameof(name));
        DateTimeOffset deadline = this._clock.UtcNow.Add(waitTime);
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.PurgeExpired(name);
            string token = Guid.NewGuid().ToString("N");
            LockState candidate = new(token, this._clock.UtcNow.Add(expiry));
            if (this._locks.TryAdd(name, candidate))
            {
                return new DistributedLockHandle(name, token, () => this.ReleaseAsync(name, token));
            }

            if (this._clock.UtcNow >= deadline || waitTime <= TimeSpan.Zero)
            {
                return null;
            }

            await Task.Delay(10, cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion

    /// <summary>
    ///     Clears all locks.
    /// </summary>
    public void Clear() => this._locks.Clear();

    private ValueTask ReleaseAsync(string name, string token)
    {
        if (this._locks.TryGetValue(name, out LockState? state) && state.Token == token)
        {
            this._locks.TryRemove(name, out _);
        }

        return ValueTask.CompletedTask;
    }

    private void PurgeExpired(string name)
    {
        if (!this._locks.TryGetValue(name, out LockState? state))
        {
            return;
        }

        if (state.ExpiresAt <= this._clock.UtcNow)
        {
            this._locks.TryRemove(name, out _);
        }
    }

    #region Nested type: LockState

    private sealed record LockState(string Token, DateTimeOffset ExpiresAt);

    #endregion
}