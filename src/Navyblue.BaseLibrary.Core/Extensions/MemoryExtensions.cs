// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : MemoryExtensions.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-29  13:02
// ****************************************************************************************************************************************
// <copyright file="MemoryExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
///     The read only memory extensions.
/// </summary>
public static class ReadOnlyMemoryExtensions
{
    /// <summary>
    ///     Converts to utf8 string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string</returns>
    public static string ToUtf8String(this ReadOnlyMemory<byte> value)
    {
        return Encoding.UTF8.GetString(value.Span);
    }

    /// <summary>
    ///     Converts to array safe.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>An array of byte</returns>
    public static byte[] ToArraySafe(this ReadOnlyMemory<byte> value)
    {
        return value.IsEmpty ? [] : value.ToArray();
    }

    /// <summary>
    ///     Converts to read only stream.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A Stream</returns>
    public static Stream ToReadOnlyStream(this ReadOnlyMemory<byte> value)
    {
        return new MemoryStream(value.ToArray(), writable: false);
    }

    /// <summary>
    ///     As read only span.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="value">The value.</param>
    /// <returns><![CDATA[ReadOnlySpan<T>]]></returns>
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ReadOnlyMemory<T> value)
    {
        return value.Span;
    }
}