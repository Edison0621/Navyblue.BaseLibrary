// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Security.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="Security.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.Security;

/// <summary>
///     The hex encoding.
/// </summary>
public static class HexEncoding
{
    /// <summary>
    ///     Converts to hex string.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns>A string</returns>
    public static string ToHexString(ReadOnlySpan<byte> bytes)
#if NET9_0_OR_GREATER
        => Convert.ToHexStringLower(bytes);
#else
        => Convert.ToHexString(bytes).ToLowerInvariant();
#endif

    /// <summary>
    ///     Froms hex string.
    /// </summary>
    /// <param name="hex">The hex.</param>
    /// <returns>An array of byte</returns>
    public static byte[] FromHexString(string hex) => Convert.FromHexString(hex);
}

/// <summary>
///     The base64 url.
/// </summary>
public static class Base64Url
{
    /// <summary>
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns>A string</returns>
    public static string Encode(ReadOnlySpan<byte> bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    /// <summary>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>An array of byte</returns>
    public static byte[] Decode(string value)
    {
        string s = value.Replace('-', '+').Replace('_', '/');
        s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
        return Convert.FromBase64String(s);
    }
}

/// <summary>
///     The hashing.
/// </summary>
public static class Hashing
{
    /// <summary>
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns>An array of byte</returns>
    public static byte[] Sha256(ReadOnlySpan<byte> bytes) => SHA256.HashData(bytes);

    /// <summary>
    ///     Sha256s the hex.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string</returns>
    public static string Sha256Hex(string value)
    {
#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
#else
        Guard.NotNullOrWhiteSpace(value, nameof(value));
#endif
        int maxUtf8 = Encoding.UTF8.GetMaxByteCount(value.Length);
        Span<byte> utf8 = maxUtf8 <= 256 ? stackalloc byte[maxUtf8] : new byte[maxUtf8];
        int written = Encoding.UTF8.GetBytes(value, utf8);
        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData(utf8[..written], hash);
        return HexEncoding.ToHexString(hash);
    }

    /// <summary>
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns>An array of byte</returns>
    public static byte[] Sha512(ReadOnlySpan<byte> bytes) => SHA512.HashData(bytes);
}

/// <summary>
///     The hmac hashing.
/// </summary>
public static class HmacHashing
{
    /// <summary>
    ///     Hmacs the sha256.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="data">The data.</param>
    /// <returns>An array of byte</returns>
    public static byte[] HmacSha256(ReadOnlySpan<byte> key, ReadOnlySpan<byte> data) => HMACSHA256.HashData(key, data);

    /// <summary>
    ///     Hmacs sha256 hex.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="data">The data.</param>
    /// <returns>A string</returns>
    public static string HmacSha256Hex(string key, string data) => HexEncoding.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(data)));
}

/// <summary>
///     The password hasher.
/// </summary>
public static class PasswordHasher
{
    private const string AlgorithmPrefix = "pbkdf2-sha256$";

    /// <summary>
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="iterations">The iterations.</param>
    /// <param name="saltSize">The salt size.</param>
    /// <param name="hashSize">The hash size.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <returns>A string</returns>
    public static string Hash(string password, int iterations = 100_000, int saltSize = 16, int hashSize = 32)
    {
#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
#else
        Guard.NotNullOrWhiteSpace(password, nameof(password));
#endif
        byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
        byte[] hash = DerivePbkdf2(password, salt, iterations, hashSize);
        return $"{AlgorithmPrefix}{iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    /// <summary>
    ///     Extracts the Base64 salt segment from a stored PBKDF2 hash string.
    /// </summary>
    public static bool TryGetSalt(string stored, out string salt)
    {
        salt = string.Empty;
        if (!TryParseStored(stored, out _, out ReadOnlySpan<char> saltChars, out _))
            return false;

        salt = saltChars.ToString();
        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="stored">The stored.</param>
    /// <returns>A bool</returns>
    public static bool Verify(string password, string stored)
    {
        if (string.IsNullOrWhiteSpace(password)
            || !TryParseStored(stored, out int iterations, out ReadOnlySpan<char> saltChars, out ReadOnlySpan<char> hashChars))
            return false;

        int saltMax = checked((saltChars.Length * 3) / 4);
        int hashMax = checked((hashChars.Length * 3) / 4);
        Span<byte> saltBuf = saltMax <= 64 ? stackalloc byte[saltMax] : new byte[saltMax];
        Span<byte> expectedBuf = hashMax <= 64 ? stackalloc byte[hashMax] : new byte[hashMax];

        if (!Convert.TryFromBase64Chars(saltChars, saltBuf, out int saltLen)
            || !Convert.TryFromBase64Chars(hashChars, expectedBuf, out int expectedLen)
            || saltLen == 0
            || expectedLen == 0)
            return false;

        ReadOnlySpan<byte> salt = saltBuf[..saltLen];
        ReadOnlySpan<byte> expected = expectedBuf[..expectedLen];

#if NET7_0_OR_GREATER
        Span<byte> actual = expectedLen <= 64 ? stackalloc byte[expectedLen] : new byte[expectedLen];
        Rfc2898DeriveBytes.Pbkdf2(password, salt, actual, iterations, HashAlgorithmName.SHA256);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
#else
        byte[] actual = DerivePbkdf2(password, salt.ToArray(), iterations, expectedLen);
        return CryptographicOperations.FixedTimeEquals(actual, expected.ToArray());
#endif
    }

    private static bool TryParseStored(
        string? stored,
        out int iterations,
        out ReadOnlySpan<char> saltChars,
        out ReadOnlySpan<char> hashChars)
    {
        iterations = 0;
        saltChars = default;
        hashChars = default;
        if (string.IsNullOrWhiteSpace(stored))
            return false;

        ReadOnlySpan<char> span = stored.AsSpan();
        if (!span.StartsWith(AlgorithmPrefix, StringComparison.Ordinal))
            return false;

        span = span[AlgorithmPrefix.Length..];
        int first = span.IndexOf('$');
        if (first <= 0 || !int.TryParse(span[..first], out iterations) || iterations <= 0)
            return false;

        span = span[(first + 1)..];
        int second = span.IndexOf('$');
        if (second <= 0)
            return false;

        saltChars = span[..second];
        hashChars = span[(second + 1)..];
        return !saltChars.IsEmpty && !hashChars.IsEmpty;
    }

    private static byte[] DerivePbkdf2(string password, byte[] salt, int iterations, int hashSize)
    {
#if NET7_0_OR_GREATER
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, hashSize);
#else
        using Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return deriveBytes.GetBytes(hashSize);
#endif
    }
}

/// <summary>
///     DotNetCore.Security-compatible hash service (PBKDF2).
/// </summary>
public interface IHashService
{
    /// <summary>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="salt">The salt.</param>
    /// <returns>A string</returns>
    string Create(string value, string salt);

    /// <summary>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="salt">The salt.</param>
    /// <param name="hash">The hash.</param>
    /// <returns>A bool</returns>
    bool Validate(string value, string salt, string hash);
}

/// <summary>
///     Default <see cref="IHashService" /> using PBKDF2-SHA512.
/// </summary>
public sealed class HashService : IHashService
{
    private const int KEY_SIZE = 64;
    private const int ITERATIONS = 100_000;

    #region IHashService Members

    /// <summary>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="salt">The salt.</param>
    /// <returns>A string</returns>
    public string Create(string value, string salt)
    {
        Guard.NotNullOrWhiteSpace(value, nameof(value));
        Guard.NotNullOrWhiteSpace(salt, nameof(salt));
#if NET7_0_OR_GREATER
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(salt), ITERATIONS, HashAlgorithmName.SHA512, KEY_SIZE);
#else
        using Rfc2898DeriveBytes deriveBytes = new(value, Encoding.UTF8.GetBytes(salt), ITERATIONS, HashAlgorithmName.SHA512);
        byte[] hash = deriveBytes.GetBytes(KEY_SIZE);
#endif
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="salt">The salt.</param>
    /// <param name="hash">The hash.</param>
    /// <returns>A bool</returns>
    public bool Validate(string value, string salt, string hash)
    {
        try
        {
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(this.Create(value, salt)),
                Convert.FromBase64String(hash));
        }
        catch
        {
            return false;
        }
    }

    #endregion

    /// <summary>
    ///     Creates the salt.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>A string</returns>
    public static string CreateSalt(int size = 32) => Convert.ToBase64String(RandomNumberGenerator.GetBytes(size));
}

/// <summary>
///     DI helpers for security services.
/// </summary>
public static class SecurityServiceCollectionExtensions
{
    /// <summary>
    ///     Add navyblue hash service.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>An IServiceCollection</returns>
    public static IServiceCollection AddNavyblueHashService(this IServiceCollection services)
    {
        services.TryAddSingleton<IHashService, HashService>();
        return services;
    }
}