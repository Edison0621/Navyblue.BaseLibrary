using System.Security.Cryptography;
using System.Text;

namespace Navyblue.BaseLibrary.Security;

public static class HexEncoding { public static string ToHexString(ReadOnlySpan<byte> bytes) => Convert.ToHexString(bytes).ToLowerInvariant(); public static byte[] FromHexString(string hex) => Convert.FromHexString(hex); }
public static class Base64Url { public static string Encode(ReadOnlySpan<byte> bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_'); public static byte[] Decode(string value) { var s = value.Replace('-', '+').Replace('_', '/'); s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '='); return Convert.FromBase64String(s); } }
public static class Hashing { public static byte[] Sha256(ReadOnlySpan<byte> bytes) => SHA256.HashData(bytes); public static string Sha256Hex(string value) => HexEncoding.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))); public static byte[] Sha512(ReadOnlySpan<byte> bytes) => SHA512.HashData(bytes); }
public static class HmacHashing { public static byte[] HmacSha256(ReadOnlySpan<byte> key, ReadOnlySpan<byte> data) => HMACSHA256.HashData(key, data); public static string HmacSha256Hex(string key, string data) => HexEncoding.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(data))); }
public static class PasswordHasher
{
    public static string Hash(string password, int iterations = 100_000, int saltSize = 16, int hashSize = 32) { ArgumentException.ThrowIfNullOrWhiteSpace(password); var salt = RandomNumberGenerator.GetBytes(saltSize); var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, hashSize); return $"pbkdf2-sha256${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}"; }
    public static bool Verify(string password, string stored) { var parts = stored.Split('$'); if (parts.Length != 4 || parts[0] != "pbkdf2-sha256" || !int.TryParse(parts[1], out var iterations)) return false; var salt = Convert.FromBase64String(parts[2]); var expected = Convert.FromBase64String(parts[3]); var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length); return CryptographicOperations.FixedTimeEquals(actual, expected); }
}
