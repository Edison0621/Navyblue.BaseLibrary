// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : PageMetadata.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="PageMetadata.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
/// </summary>
public interface IPageMetadataAccessor
{
    /// <summary>
    ///     Gets or sets the current.
    /// </summary>
    /// <value>
    ///     The current.
    /// </value>
    PageMetadata Current { get; set; }
}

/// <summary>
/// </summary>
public sealed class PageMetadata
{
    /// <summary>
    ///     Gets the canonical URL.
    /// </summary>
    /// <value>
    ///     The canonical URL.
    /// </value>
    public string? CanonicalUrl { get; init; }

    /// <summary>
    ///     Gets the description.
    /// </summary>
    /// <value>
    ///     The description.
    /// </value>
    public string? Description { get; init; }

    /// <summary>
    ///     Gets the meta.
    /// </summary>
    /// <value>
    ///     The meta.
    /// </value>
    public IDictionary<string, string> Meta { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Gets the title.
    /// </summary>
    /// <value>
    ///     The title.
    /// </value>
    public string Title { get; init; } = string.Empty;
}

/// <summary>
/// </summary>
/// <seealso cref="Navyblue.Foundation.AspNetCore.IPageMetadataAccessor" />
public sealed class PageMetadataAccessor : IPageMetadataAccessor
{
    /// <summary>
    ///     The current value
    /// </summary>
    private static readonly AsyncLocal<PageMetadata?> _currentValue = new();

    #region IPageMetadataAccessor Members

    /// <summary>
    ///     Gets or sets the current.
    /// </summary>
    /// <value>
    ///     The current.
    /// </value>
    public PageMetadata Current
    {
        get => _currentValue.Value ??= new PageMetadata();
        set => _currentValue.Value = value;
    }

    #endregion
}

/// <summary>
/// </summary>
public static class PageMetadataExtensions
{
    /// <summary>
    ///     Withes the title.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    /// <param name="title">The title.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static PageMetadata WithTitle(this PageMetadata metadata, string title)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        return new PageMetadata { Title = title, Description = metadata.Description, CanonicalUrl = metadata.CanonicalUrl, Meta = metadata.Meta };
    }
}