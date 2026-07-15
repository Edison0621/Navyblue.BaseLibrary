// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : RefreshTokenProtector.cs
// Created          : 2026-07-13  11:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="RefreshTokenProtector.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Buffers;
using System.Security.Cryptography;
using System.Text;

namespace NavyblueWebApi.Application.Authentication;

/// <summary>
///     Opaque refresh-token helpers (generate plaintext + SHA-256 hash for storage).
/// </summary>
public static class RefreshTokenProtector
{
    private const int TokenByteLength = 64;

    public static string CreateToken()
    {
        Span<byte> bytes = stackalloc byte[TokenByteLength];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public static string Hash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        // Base64 of 64 bytes is 88 chars; leave headroom for atypical tokens.
        int maxUtf8 = Encoding.UTF8.GetMaxByteCount(token.Length);
        byte[]? rented = null;
        Span<byte> utf8 = maxUtf8 <= 256
            ? stackalloc byte[maxUtf8]
            : (rented = ArrayPool<byte>.Shared.Rent(maxUtf8)).AsSpan(0, maxUtf8);

        try
        {
            int written = Encoding.UTF8.GetBytes(token, utf8);
            Span<byte> hash = stackalloc byte[SHA256.HashSizeInBytes];
            SHA256.HashData(utf8[..written], hash);
            return Convert.ToHexStringLower(hash);
        }
        finally
        {
            if (rented is not null)
                ArrayPool<byte>.Shared.Return(rented);
        }
    }
}