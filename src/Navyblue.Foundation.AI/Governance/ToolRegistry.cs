// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ToolRegistry.cs
// Created          : 2026-07-30  15:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="ToolRegistry.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Process-wide, thread-safe tool catalog. Populate at startup; the scoped <see cref="ToolGateway" /> reads from it.
/// </summary>
public sealed class ToolRegistry : IToolRegistry
{
    private readonly ConcurrentDictionary<string, ToolDefinition> _tools = new(StringComparer.OrdinalIgnoreCase);

    #region IToolRegistry Members

    /// <inheritdoc />
    public void Register(ToolDefinition tool)
    {
        ArgumentNullException.ThrowIfNull(tool);
        Guard.NotNullOrWhiteSpace(tool.Name, nameof(tool.Name));
        this._tools[tool.Name] = tool;
    }

    /// <inheritdoc />
    public bool TryRemove(string name) => this._tools.TryRemove(name, out _);

    /// <inheritdoc />
    public IReadOnlyList<ToolDefinition> ListTools(string? tenantId = null)
        => this._tools.Values
            .Where(t => t.IsVisibleTo(tenantId))
            .OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

    /// <inheritdoc />
    public bool TryGetTool(string name, out ToolDefinition? tool)
    {
        if (!string.IsNullOrWhiteSpace(name) && this._tools.TryGetValue(name, out ToolDefinition? found))
        {
            tool = found;
            return true;
        }

        tool = null;
        return false;
    }

    #endregion
}