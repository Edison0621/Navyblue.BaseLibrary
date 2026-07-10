// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IGenericSearchQuery.cs
// Created          : 2026-07-10  17:07
//
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IGenericSearchQuery.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///
/// </summary>
public interface IGenericSearchQuery
{
    /// <summary>
    /// Gets or sets the filter.
    /// </summary>
    /// <value>
    /// The filter.
    /// </value>
    string Filter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [include total count].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [include total count]; otherwise, <c>false</c>.
    /// </value>
    bool IncludeTotalCount { get; set; }

    /// <summary>
    /// Gets or sets the order by.
    /// </summary>
    /// <value>
    /// The order by.
    /// </value>
    string OrderBy { get; set; }

    /// <summary>
    /// Gets or sets the search text.
    /// </summary>
    /// <value>
    /// The search text.
    /// </value>
    string SearchText { get; set; }

    /// <summary>
    /// Gets or sets the select.
    /// </summary>
    /// <value>
    /// The select.
    /// </value>
    string Select { get; set; }

    /// <summary>
    /// Gets or sets the skip.
    /// </summary>
    /// <value>
    /// The skip.
    /// </value>
    int Skip { get; set; }

    /// <summary>
    /// Gets or sets the top.
    /// </summary>
    /// <value>
    /// The top.
    /// </value>
    int Top { get; set; }
}