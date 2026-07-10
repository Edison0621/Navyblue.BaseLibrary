// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IGenericEntitiesResponse.cs
// Created          : 2026-07-10  17:07
//
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IGenericEntitiesResponse.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IGenericEntitiesResponse<TResponse>
{
    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    int Count { get; set; }

    /// <summary>
    /// Gets or sets the parent source.
    /// </summary>
    /// <value>
    /// The parent source.
    /// </value>
    string ParentSource { get; set; }

    /// <summary>
    /// Gets or sets the results.
    /// </summary>
    /// <value>
    /// The results.
    /// </value>
    IList<TResponse> Results { get; set; }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    /// <value>
    /// The source.
    /// </value>
    string Source { get; set; }

    /// <summary>
    /// Gets or sets the total count.
    /// </summary>
    /// <value>
    /// The total count.
    /// </value>
    long TotalCount { get; set; }
}