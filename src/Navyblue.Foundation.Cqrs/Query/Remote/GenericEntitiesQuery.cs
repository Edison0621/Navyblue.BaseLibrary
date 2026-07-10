// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : GenericEntitiesQuery.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="GenericEntitiesQuery.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics.CodeAnalysis;

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     The generic entities query.
/// </summary>
/// <typeparam name="QueryResponse" />
[ExcludeFromCodeCoverage]
public abstract class GenericEntitiesQuery<QueryResponse> : Query<GenericEntitiesQueryResponse<QueryResponse>>, IGenericSearchQuery
{
    #region IGenericSearchQuery Members

    /// <summary>
    ///     Gets or sets the search text.
    /// </summary>
    public string SearchText { get; set; }

    /// <summary>
    ///     Gets or sets the filter.
    /// </summary>
    public string Filter { get; set; }

    /// <summary>
    ///     Gets or sets the select.
    /// </summary>
    public string Select { get; set; }

    /// <summary>
    ///     Gets or sets the order by.
    /// </summary>
    public string OrderBy { get; set; }

    /// <summary>
    ///     Gets or sets the top.
    /// </summary>
    public int Top { get; set; }

    /// <summary>
    ///     Gets or sets the skip.
    /// </summary>
    public int Skip { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether include total count.
    /// </summary>
    public bool IncludeTotalCount { get; set; }

    #endregion
}