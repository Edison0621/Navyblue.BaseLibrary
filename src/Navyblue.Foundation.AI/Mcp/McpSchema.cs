// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : McpSchema.cs
// Created          : 2026-07-30  16:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="McpSchema.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Text.Json;
using System.Text.Json.Nodes;

namespace Navyblue.Foundation.AI;

/// <summary>
///     A tool as published to an MCP client: name, description, and JSON schema for its input.
/// </summary>
public sealed record McpToolDescriptor(string Name, string Description, string InputSchemaJson, string RiskLevel);

/// <summary>
///     Generates MCP-compatible JSON schemas from declared <see cref="ToolParameterSpec" /> lists, so the
///     schema published to clients and the schema enforced by the gateway come from one source of truth.
/// </summary>
public static class McpSchemaGenerator
{
    private static readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = false };

    /// <summary>
    ///     Builds the JSON schema for a tool. Falls back to an explicitly supplied
    ///     <see cref="ToolDefinition.InputSchemaJson" /> when no parameters are declared.
    /// </summary>
    public static string ToJsonSchema(ToolDefinition tool)
    {
        ArgumentNullException.ThrowIfNull(tool);

        if (tool.Parameters is null || tool.Parameters.Count == 0)
        {
            return tool.InputSchemaJson ?? """{"type":"object","properties":{},"additionalProperties":false}""";
        }

        JsonObject properties = [];
        JsonArray required = [];

        foreach (ToolParameterSpec spec in tool.Parameters)
        {
            JsonObject property = new() { ["type"] = MapType(spec.Type) };

            if (spec.Type == ToolParameterType.DateTime)
            {
                property["format"] = "date-time";
            }

            if (!string.IsNullOrWhiteSpace(spec.Description))
            {
                property["description"] = spec.Description;
            }

            if (spec.MaxLength is { } maxLength)
            {
                property["maxLength"] = maxLength;
            }

            if (spec.Minimum is { } minimum)
            {
                property["minimum"] = minimum;
            }

            if (spec.Maximum is { } maximum)
            {
                property["maximum"] = maximum;
            }

            if (spec.AllowedValues is { Count: > 0 } allowed)
            {
                JsonArray values = [];
                foreach (string value in allowed)
                {
                    values.Add(value);
                }

                property["enum"] = values;
            }

            properties[spec.Name] = property;

            if (spec.Required)
            {
                required.Add(spec.Name);
            }
        }

        JsonObject schema = new()
        {
            ["type"] = "object",
            ["properties"] = properties,
            ["additionalProperties"] = false
        };

        if (required.Count > 0)
        {
            schema["required"] = required;
        }

        return schema.ToJsonString(_serializerOptions);
    }

    /// <summary>
    ///     Projects a tool into its MCP descriptor.
    /// </summary>
    public static McpToolDescriptor Describe(ToolDefinition tool)
    {
        ArgumentNullException.ThrowIfNull(tool);
        return new McpToolDescriptor(tool.Name, tool.Description, ToJsonSchema(tool), tool.RiskLevel.ToString());
    }

    /// <summary>
    ///     Projects every tool visible to a tenant into MCP descriptors.
    /// </summary>
    public static IReadOnlyList<McpToolDescriptor> Describe(IEnumerable<ToolDefinition> tools)
    {
        ArgumentNullException.ThrowIfNull(tools);
        return tools.Select(Describe).ToArray();
    }

    private static string MapType(ToolParameterType type) => type switch
    {
        ToolParameterType.Integer => "integer",
        ToolParameterType.Number => "number",
        ToolParameterType.Boolean => "boolean",
        ToolParameterType.Array => "array",
        ToolParameterType.Object => "object",
        _ => "string"
    };
}