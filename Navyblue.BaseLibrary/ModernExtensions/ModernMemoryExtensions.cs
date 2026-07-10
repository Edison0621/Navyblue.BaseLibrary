// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernMemoryExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:04
// ****************************************************************************************************************************************
// <copyright file="ModernMemoryExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable

using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public static class ModernReadOnlyMemoryExtensions
{
    /// <summary>
    ///     Ases the read only span.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ReadOnlyMemory<T> value) => value.Span;

    /// <summary>
    ///     Converts to arraysafe.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static byte[] ToArraySafe(this ReadOnlyMemory<byte> value) => value.IsEmpty ? Array.Empty<byte>() : value.ToArray();

    /// <summary>
    ///     Converts to readonlystream.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static Stream ToReadOnlyStream(this ReadOnlyMemory<byte> value) => new MemoryStream(value.ToArray(), writable: false);

    /// <summary>
    ///     Converts to utf8string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToUtf8String(this ReadOnlyMemory<byte> value) => Encoding.UTF8.GetString(value.Span);
}