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
    public static string ToHexString(ReadOnlySpan<byte> bytes) => Convert.ToHexString(bytes).ToLowerInvariant();

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
    public static string Sha256Hex(string value) => HexEncoding.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

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
        Guard.NotNullOrWhiteSpace(password, nameof(password));
        byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
        byte[] hash = DerivePbkdf2(password, salt, iterations, hashSize);
        return $"pbkdf2-sha256${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="stored">The stored.</param>
    /// <returns>A bool</returns>
    public static bool Verify(string password, string stored)
    {
        string[] parts = stored.Split('$');
        if (parts.Length != 4 || parts[0] != "pbkdf2-sha256" || !int.TryParse(parts[1], out int iterations)) return false;
        byte[] salt = Convert.FromBase64String(parts[2]);
        byte[] expected = Convert.FromBase64String(parts[3]);
        byte[] actual = DerivePbkdf2(password, salt, iterations, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
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