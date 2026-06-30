// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Idempotency.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:51
// ****************************************************************************************************************************************
// <copyright file="Idempotency.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Idempotency;

/// <summary>
///     The idempotency key.
/// </summary>
public sealed record IdempotencyKey(string Value)
{
    /// <summary>
    ///     Froms the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     An IdempotencyKey
    /// </returns>
    /// <exception cref="ArgumentException">Idempotency key cannot be empty. - value</exception>
    public static IdempotencyKey From(string value) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Idempotency key cannot be empty.", nameof(value)) : new(value.Trim());
}

/// <summary>
///     The idempotencies states.
/// </summary>
public enum IdempotencyState
{
    /// <summary>
    ///     The processing
    /// </summary>
    Processing,

    /// <summary>
    ///     The succeeded
    /// </summary>
    Succeeded,

    /// <summary>
    ///     The failed
    /// </summary>
    Failed
}

/// <summary>
///     The idempotency record.
/// </summary>
public sealed record IdempotencyRecord(IdempotencyKey Key, IdempotencyState State, DateTimeOffset CreatedAt, DateTimeOffset? ExpiresAt, string? ResponsePayload = null);

/// <summary>
///     The idempotency store interface.
/// </summary>
public interface IIdempotencyStore
{
    /// <summary>
    ///     Try begin asynchronously.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="ttl">The ttl.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     <![CDATA[ValueTask<bool>]]>
    /// </returns>
    ValueTask<bool> TryBeginAsync(IdempotencyKey key, TimeSpan ttl, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Completes the asynchronous.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="responsePayload">The response payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A ValueTask
    /// </returns>
    ValueTask CompleteAsync(IdempotencyKey key, string? responsePayload = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Fails the asynchronous.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="reason">The reason.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A ValueTask
    /// </returns>
    ValueTask FailAsync(IdempotencyKey key, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get and return a valuetask of type idempotencyrecord asynchronously.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     <![CDATA[ValueTask<IdempotencyRecord?>]]>
    /// </returns>
    ValueTask<IdempotencyRecord?> GetAsync(IdempotencyKey key, CancellationToken cancellationToken = default);
}