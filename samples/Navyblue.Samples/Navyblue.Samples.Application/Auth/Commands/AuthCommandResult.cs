using Navyblue.Foundation.Cqrs;

namespace Navyblue.Samples.Application.Authentication.Commands;

/// <summary>
///     Result of a successful authentication: a signed JWT and the resolved user identity.
/// </summary>
public sealed class AuthCommandResult : CommandResult
{
    public AuthCommandResult(long userId, string userName, string token)
        : base(true)
    {
        this.UserId = userId;
        this.UserName = userName;
        this.Token = token;
    }

    public long UserId { get; }

    public string UserName { get; }

    /// <summary>Compact JWT to present as a Bearer token.</summary>
    public string Token { get; }
}
