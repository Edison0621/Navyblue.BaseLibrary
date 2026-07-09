// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernSecurityExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="ModernSecurityExtensions.cs" company="">
//     Copyright (c) 2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Security.Cryptography;

namespace Navyblue.BaseLibrary.Extensions;

public static class ModernGuidV7
{
    private const int Version7 = 7;

    public static Guid NewGuidV7() => NewGuidV7(DateTimeOffset.UtcNow);

    public static Guid NewGuidV7(DateTimeOffset timestamp)
    {
        long unixTimeMilliseconds = timestamp.ToUnixTimeMilliseconds();
        if (unixTimeMilliseconds < 0 || unixTimeMilliseconds > 0xFFFFFFFFFFFFL)
        {
            throw new ArgumentOutOfRangeException(nameof(timestamp), "UUID v7 timestamp must fit in 48 bits of Unix epoch milliseconds.");
        }

        Span<byte> uuid = stackalloc byte[16];
        RandomNumberGenerator.Fill(uuid);

        uuid[0] = (byte)(unixTimeMilliseconds >> 40);
        uuid[1] = (byte)(unixTimeMilliseconds >> 32);
        uuid[2] = (byte)(unixTimeMilliseconds >> 24);
        uuid[3] = (byte)(unixTimeMilliseconds >> 16);
        uuid[4] = (byte)(unixTimeMilliseconds >> 8);
        uuid[5] = (byte)unixTimeMilliseconds;
        uuid[6] = (byte)((Version7 << 4) | (uuid[6] & 0x0F));
        uuid[8] = (byte)((uuid[8] & 0x3F) | 0x80);

        return CreateGuidFromRfcBytes(uuid);
    }

    public static bool IsVersion7(this Guid value)
    {
        Span<byte> bytes = stackalloc byte[16];
        WriteRfcBytes(value, bytes);
        return (bytes[6] >> 4) == Version7;
    }

    public static DateTimeOffset? GetVersion7Timestamp(this Guid value)
    {
        Span<byte> bytes = stackalloc byte[16];
        WriteRfcBytes(value, bytes);
        if ((bytes[6] >> 4) != Version7) return null;

        long milliseconds = ((long)bytes[0] << 40)
            | ((long)bytes[1] << 32)
            | ((long)bytes[2] << 24)
            | ((long)bytes[3] << 16)
            | ((long)bytes[4] << 8)
            | bytes[5];

        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
    }

    private static Guid CreateGuidFromRfcBytes(ReadOnlySpan<byte> bytes)
    {
        Span<byte> guidBytes = stackalloc byte[16];
        guidBytes[0] = bytes[3];
        guidBytes[1] = bytes[2];
        guidBytes[2] = bytes[1];
        guidBytes[3] = bytes[0];
        guidBytes[4] = bytes[5];
        guidBytes[5] = bytes[4];
        guidBytes[6] = bytes[7];
        guidBytes[7] = bytes[6];
        bytes[8..16].CopyTo(guidBytes[8..16]);
        return new Guid(guidBytes);
    }

    private static void WriteRfcBytes(Guid value, Span<byte> destination)
    {
        Span<byte> guidBytes = stackalloc byte[16];
        value.TryWriteBytes(guidBytes);
        destination[0] = guidBytes[3];
        destination[1] = guidBytes[2];
        destination[2] = guidBytes[1];
        destination[3] = guidBytes[0];
        destination[4] = guidBytes[5];
        destination[5] = guidBytes[4];
        destination[6] = guidBytes[7];
        destination[7] = guidBytes[6];
        guidBytes[8..16].CopyTo(destination[8..16]);
    }
}

public static class ModernBase64UrlExtensions
{
    public static string ToBase64UrlString(this ReadOnlySpan<byte> value)
    {
        return Convert.ToBase64String(value).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    public static byte[] FromBase64UrlString(this string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        string padded = value.Replace('-', '+').Replace('_', '/');
        padded = (padded.Length % 4) switch
        {
            0 => padded,
            2 => padded + "==",
            3 => padded + "=",
            _ => throw new FormatException("Invalid Base64Url string length.")
        };
        return Convert.FromBase64String(padded);
    }
}

public sealed record AesGcmPayload(byte[] Nonce, byte[] Ciphertext, byte[] Tag);

public static class ModernAuthenticatedEncryptionExtensions
{
    public static AesGcmPayload EncryptAesGcm(this ReadOnlySpan<byte> plaintext, ReadOnlySpan<byte> key, ReadOnlySpan<byte> associatedData = default)
    {
        ValidateAesKey(key);
        byte[] nonce = RandomNumberGenerator.GetBytes(12);
        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[16];

#if NET8_0_OR_GREATER
        using AesGcm aes = new AesGcm(key, tag.Length);
#else
        using AesGcm aes = new AesGcm(key.ToArray());
#endif
        aes.Encrypt(nonce, plaintext, ciphertext, tag, associatedData);
        return new AesGcmPayload(nonce, ciphertext, tag);
    }

    public static byte[] DecryptAesGcm(this AesGcmPayload payload, ReadOnlySpan<byte> key, ReadOnlySpan<byte> associatedData = default)
    {
        if (payload == null) throw new ArgumentNullException(nameof(payload));
        ValidateAesKey(key);
        byte[] plaintext = new byte[payload.Ciphertext.Length];

#if NET8_0_OR_GREATER
        using AesGcm aes = new AesGcm(key, payload.Tag.Length);
#else
        using AesGcm aes = new AesGcm(key.ToArray());
#endif
        aes.Decrypt(payload.Nonce, payload.Ciphertext, payload.Tag, plaintext, associatedData);
        return plaintext;
    }

    private static void ValidateAesKey(ReadOnlySpan<byte> key)
    {
        if (key.Length is not (16 or 24 or 32))
        {
            throw new ArgumentException("AES key length must be 128, 192, or 256 bits.", nameof(key));
        }
    }
}

#if NET8_0_OR_GREATER
public static class ModernSha3Extensions
{
    public static bool Sha3IsSupported => SHA3_256.IsSupported;

    public static byte[] Sha3_256(this ReadOnlySpan<byte> value)
    {
        EnsureSha3Supported();
        return SHA3_256.HashData(value);
    }

    public static byte[] Sha3_384(this ReadOnlySpan<byte> value)
    {
        EnsureSha3Supported();
        return SHA3_384.HashData(value);
    }

    public static byte[] Sha3_512(this ReadOnlySpan<byte> value)
    {
        EnsureSha3Supported();
        return SHA3_512.HashData(value);
    }

    public static byte[] HmacSha3_256(this ReadOnlySpan<byte> value, ReadOnlySpan<byte> key)
    {
        EnsureSha3Supported();
        return HMACSHA3_256.HashData(key, value);
    }

    public static byte[] HmacSha3_384(this ReadOnlySpan<byte> value, ReadOnlySpan<byte> key)
    {
        EnsureSha3Supported();
        return HMACSHA3_384.HashData(key, value);
    }

    public static byte[] HmacSha3_512(this ReadOnlySpan<byte> value, ReadOnlySpan<byte> key)
    {
        EnsureSha3Supported();
        return HMACSHA3_512.HashData(key, value);
    }

    private static void EnsureSha3Supported()
    {
        if (!SHA3_256.IsSupported)
        {
            throw new PlatformNotSupportedException("SHA-3 is not supported on the current platform.");
        }
    }
}
#endif