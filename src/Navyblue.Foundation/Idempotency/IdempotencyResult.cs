// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IdempotencyResult.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IdempotencyResult.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Idempotency;

/// <summary>
///     The idempotencies decisions.
/// </summary>
public enum IdempotencyDecision
{
    /// <summary>
    ///     The started
    /// </summary>
    Started,

    /// <summary>
    ///     The replayed
    /// </summary>
    Replayed,

    /// <summary>
    ///     The conflict
    /// </summary>
    Conflict
}

/// <summary>
///     The idempotency result.
/// </summary>
public sealed record IdempotencyResult(IdempotencyDecision Decision, string? ResponsePayload = null)
{
    /// <summary>
    ///     Gets a value indicating whether should execute.
    /// </summary>
    /// <value>
    ///     <c>true</c> if [should execute]; otherwise, <c>false</c>.
    /// </value>
    public bool ShouldExecute => this.Decision == IdempotencyDecision.Started;
}

/// <summary>
///     The idempotency key provider interface.
/// </summary>
public interface IIdempotencyKeyProvider
{
    /// <summary>
    ///     Get the key asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     <![CDATA[ValueTask<IdempotencyKey?>]]>
    /// </returns>
    ValueTask<IdempotencyKey?> GetKeyAsync(CancellationToken cancellationToken = default);
}