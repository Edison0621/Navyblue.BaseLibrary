namespace NavyblueWebApi.Application.Authentication;

/// <summary>
///     Refresh-token lifetime options (bound from <c>RefreshToken</c> configuration section).
/// </summary>
public sealed class RefreshTokenOptions
{
    public const string SectionName = "RefreshToken";

    /// <summary>How long a refresh token remains valid. Default: 7 days.</summary>
    public TimeSpan Expire { get; set; } = TimeSpan.FromDays(7);
}
