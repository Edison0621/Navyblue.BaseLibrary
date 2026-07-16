// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : PasswordHashFormat.cs
// Created          : 2026-07-15  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="PasswordHashFormat.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace NavyblueWebApi.Application.Authentication;

/// <summary>
///     Span-based parsing helpers for Foundation <c>PasswordHasher</c> stored strings.
/// </summary>
public static class PasswordHashFormat
{
    /// <summary>
    ///     Extracts the Base64 salt from <c>pbkdf2-sha256$iter$salt$hash</c> without allocating a split array.
    /// </summary>
    public static string ExtractSalt(string stored)
    {
        if (string.IsNullOrWhiteSpace(stored))
            return string.Empty;

        ReadOnlySpan<char> span = stored.AsSpan();
        int first = span.IndexOf('$');
        if (first < 0) return string.Empty;
        span = span[(first + 1)..];

        int second = span.IndexOf('$');
        if (second < 0) return string.Empty;
        span = span[(second + 1)..];

        int third = span.IndexOf('$');
        if (third < 0) return string.Empty;
        return span[..third].ToString();
    }
}