// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernMemoryExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="ModernMemoryExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

public static class ModernReadOnlyMemoryExtensions
{
    public static string ToUtf8String(this ReadOnlyMemory<byte> value) => Encoding.UTF8.GetString(value.Span);
    public static byte[] ToArraySafe(this ReadOnlyMemory<byte> value) => value.IsEmpty ? Array.Empty<byte>() : value.ToArray();
    public static Stream ToReadOnlyStream(this ReadOnlyMemory<byte> value) => new MemoryStream(value.ToArray(), writable: false);
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ReadOnlyMemory<T> value) => value.Span;
}