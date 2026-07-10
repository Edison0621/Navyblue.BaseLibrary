// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : GenericEntitiesQueryResponse.cs
// Created          : 2026-07-10  17:07
//
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="GenericEntitiesQueryResponse.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics.CodeAnalysis;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///
/// </summary>
/// <typeparam name="QueryResponse">The type of the uery response.</typeparam>
/// <seealso cref="Navyblue.Foundation.Cqrs.IGenericEntitiesResponse&lt;QueryResponse&gt;" />
[ExcludeFromCodeCoverage]
public class GenericEntitiesQueryResponse<QueryResponse> : IGenericEntitiesResponse<QueryResponse>
{
    #region IGenericEntitiesResponse<QueryResponse> Members

    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the parent source.
    /// </summary>
    /// <value>
    /// The parent source.
    /// </value>
    public string ParentSource { get; set; }

    /// <summary>
    /// Gets or sets the results.
    /// </summary>
    /// <value>
    /// The results.
    /// </value>
    public IList<QueryResponse> Results { get; set; }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    /// <value>
    /// The source.
    /// </value>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the total count.
    /// </summary>
    /// <value>
    /// The total count.
    /// </value>
    public long TotalCount { get; set; }

    #endregion IGenericEntitiesResponse<QueryResponse> Members
}