using System.Security.Cryptography;
using System.Text;

namespace NavyblueWebApi.Application.Authentication;

/// <summary>
///     Opaque refresh-token helpers (generate plaintext + SHA-256 hash for storage).
/// </summary>
public static class RefreshTokenProtector
{
    public static string CreateToken()
    {
        byte[] bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public static string Hash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
