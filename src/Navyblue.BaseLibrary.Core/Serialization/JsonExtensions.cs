// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : JsonExtensions.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="JsonExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Text.Json;

namespace Navyblue.BaseLibrary.Serialization;

/// <summary>
///     The json extensions.
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    ///     The default options
    /// </summary>
    private static readonly JsonSerializerOptions _defaultOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    ///     Converts to the json.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="options">The options.</param>
    /// <returns>
    ///     A string
    /// </returns>
    public static string ToJson<T>(this T value, JsonSerializerOptions? options = null) => JsonSerializer.Serialize(value, options ?? _defaultOptions);

    /// <summary>
    ///     Froms the json.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json">The json.</param>
    /// <param name="options">The options.</param>
    /// <returns>
    ///     A <typeparamref name="T" />
    /// </returns>
    public static T? FromJson<T>(this string json, JsonSerializerOptions? options = null) => JsonSerializer.Deserialize<T>(json, options ?? _defaultOptions);
}