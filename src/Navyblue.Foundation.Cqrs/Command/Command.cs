// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Command.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="Command.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Base representation of a Command
/// </summary>
/// <typeparam name="TCommandResponse">Type of the command response</typeparam>
/// <seealso cref="Navyblue.Foundation.Cqrs.IRequest&lt;TCommandResponse&gt;" />
public abstract class Command<TCommandResponse> : IRequest<TCommandResponse>
    where TCommandResponse : CommandResult
{
    /// <summary>
    ///     Gets or sets the correlation identifier.
    /// </summary>
    /// <value>
    ///     The correlation identifier.
    /// </value>
    public string CorrelationId { get; set; }

    /// <summary>
    ///     Gets or sets the transaction identifier.
    /// </summary>
    /// <value>
    ///     The transaction identifier.
    /// </value>
    public string TransactionId { get; set; }

    /// <summary>
    ///     Gets or sets the domain.
    /// </summary>
    /// <value>
    ///     The domain.
    /// </value>
    public string Domain { get; set; }

    /// <summary>
    ///     Gets or sets the issued at.
    /// </summary>
    /// <value>
    ///     The issued at.
    /// </value>
    public DateTime IssuedAt { get; set; }

    #region IRequest<TCommandResponse> Members

    /// <summary>
    ///     Gets the display name.
    /// </summary>
    /// <value>
    ///     The display name.
    /// </value>
    public abstract string DisplayName { get; }

    /// <summary>
    ///     Gets the identifier.
    /// </summary>
    /// <value>
    ///     The identifier.
    /// </value>
    public abstract string Id { get; }

    /// <summary>
    ///     Validates the specified validation error message.
    /// </summary>
    /// <param name="validationErrorMessage">The validation error message.</param>
    /// <returns></returns>
    public abstract bool Validate(out string validationErrorMessage);

    #endregion
}