// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Locking.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:51
// ****************************************************************************************************************************************
// <copyright file="Locking.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Locking;

/// <summary>
///     The distributed lock interface.
/// </summary>
/// <seealso cref="System.IAsyncDisposable" />
public interface IDistributedLock : IAsyncDisposable
{
    /// <summary>
    ///     Gets the name.
    /// </summary>
    /// <value>
    ///     The name.
    /// </value>
    string Name { get; }

    /// <summary>
    ///     Gets the token.
    /// </summary>
    /// <value>
    ///     The token.
    /// </value>
    string Token { get; }

    /// <summary>
    ///     Gets a value indicating whether this instance is acquired.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is acquired; otherwise, <c>false</c>.
    /// </value>
    bool IsAcquired { get; }
}

/// <summary>
///     The distributed lock provider interface.
/// </summary>
public interface IDistributedLockProvider
{
    /// <summary>
    ///     Try the acquire asynchronously.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="expiry">The expiry.</param>
    /// <param name="waitTime">The wait time.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     <![CDATA[ValueTask<IDistributedLock?>]]>
    /// </returns>
    ValueTask<IDistributedLock?> TryAcquireAsync(string name, TimeSpan expiry, TimeSpan waitTime, CancellationToken cancellationToken = default);
}

/// <summary>
///     The distributed lock handle.
/// </summary>
/// <seealso cref="Navyblue.BaseLibrary.Locking.IDistributedLock" />
/// <param name="name">The name.</param>
/// <param name="token">The token.</param>
/// <param name="release">The release.</param>
public sealed class DistributedLockHandle(string name, string token, Func<ValueTask> release) : IDistributedLock
{
    #region IDistributedLock Members

    /// <summary>
    ///     Gets the name.
    /// </summary>
    /// <value>
    ///     The name.
    /// </value>
    public string Name { get; } = name;

    /// <summary>
    ///     Gets the token.
    /// </summary>
    /// <value>
    ///     The token.
    /// </value>
    public string Token { get; } = token;

    /// <summary>
    ///     Gets a value indicating whether acquired.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is acquired; otherwise, <c>false</c>.
    /// </value>
    public bool IsAcquired { get; private set; } = true;

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
    /// </summary>
    /// <returns>
    ///     A ValueTask
    /// </returns>
    public async ValueTask DisposeAsync()
    {
        if (!this.IsAcquired) return;
        this.IsAcquired = false;
        await release().ConfigureAwait(false);
    }

    #endregion
}