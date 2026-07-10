// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Query.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="Query.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// Base representation of a query
/// </summary>
/// <typeparam name="QueryResponse">Type of the query response</typeparam>
/// <seealso cref="Navyblue.Foundation.Cqrs.IRequest&lt;QueryResponse&gt;" />
public abstract class Query<QueryResponse> : IRequest<QueryResponse>
{
    /// <summary>
    /// Gets or sets the correlation identifier.
    /// </summary>
    /// <value>
    /// The correlation identifier.
    /// </value>
    public string CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the transaction identifier.
    /// </summary>
    /// <value>
    /// The transaction identifier.
    /// </value>
    public string TransactionId { get; set; }

    #region IRequest<QueryResponse> Members

    /// <summary>
    /// Gets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    public abstract string DisplayName { get; }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public abstract string Id { get; }

    /// <summary>
    /// Validates the specified validation error message.
    /// </summary>
    /// <param name="validationErrorMessage">The validation error message.</param>
    /// <returns></returns>
    public abstract bool Validate(out string validationErrorMessage);

    #endregion
}