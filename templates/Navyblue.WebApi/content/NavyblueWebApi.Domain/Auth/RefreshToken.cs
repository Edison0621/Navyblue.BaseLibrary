using Navyblue.Foundation.Domain;

namespace NavyblueWebApi.Domain.Authentication;

/// <summary>
///     Opaque refresh token persisted as a SHA-256 hash. Plaintext is returned to the client once.
/// </summary>
public sealed class RefreshToken : Entity<long>
{
    private RefreshToken() : base(default)
    {
        this.TokenHash = null!;
    }

    public RefreshToken(long id, long userId, string tokenHash, DateTimeOffset expiresAt) : base(id)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("Token hash is required.", nameof(tokenHash));

        this.UserId = userId;
        this.TokenHash = tokenHash;
        this.ExpiresAt = expiresAt;
        this.CreatedAt = DateTimeOffset.UtcNow;
    }

    public long UserId { get; private set; }

    public string TokenHash { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    /// <summary>Hash of the replacement token after rotation (audit trail).</summary>
    public string? ReplacedByTokenHash { get; private set; }

    public bool IsExpired(DateTimeOffset? utcNow = null) => (utcNow ?? DateTimeOffset.UtcNow) >= this.ExpiresAt;

    public bool IsRevoked => this.RevokedAt is not null;

    public bool IsActive(DateTimeOffset? utcNow = null) => !this.IsRevoked && !this.IsExpired(utcNow);

    public void Revoke(string? replacedByTokenHash = null, DateTimeOffset? revokedAt = null)
    {
        if (this.IsRevoked) return;
        this.RevokedAt = revokedAt ?? DateTimeOffset.UtcNow;
        this.ReplacedByTokenHash = replacedByTokenHash;
    }
}
