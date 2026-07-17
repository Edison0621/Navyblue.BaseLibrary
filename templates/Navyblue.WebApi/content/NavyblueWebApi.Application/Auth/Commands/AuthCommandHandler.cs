using Microsoft.Extensions.Options;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;
using Navyblue.Foundation.Security;
using NavyblueWebApi.Application.Users;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;

namespace NavyblueWebApi.Application.Authentication.Commands;

/// <summary>
///     Authenticates a user by login (email) and password.
/// </summary>
public sealed class AuthCommand(string login, string password) : Command<AuthCommandResult>
{
    public string Login { get; } = login;

    public string Password { get; } = password;

    public override string DisplayName => "Auth";

    public override string Id { get; } = Guid.NewGuid().ToString("N");

    public override bool Validate(out string validationErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(this.Login))
        {
            validationErrorMessage = "Login is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(this.Password))
        {
            validationErrorMessage = "Password is required.";
            return false;
        }

        validationErrorMessage = string.Empty;
        return true;
    }
}

/// <summary>
///     Login: verify password, issue access JWT + refresh token (persisted hash).
/// </summary>
public sealed class AuthCommandHandler(
    IAuthRepository authRepository,
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITokenIssuer tokenIssuer,
    IIdGenerator<long> idGenerator,
    IOptions<RefreshTokenOptions> refreshTokenOptions)
    : CommandHandler<AuthCommand, AuthCommandResult>
{
    private readonly RefreshTokenOptions _refreshTokenOptions = refreshTokenOptions.Value;

    protected override async Task<AuthCommandResult> ProcessRequest(AuthCommand command)
    {
        Auth? auth = await authRepository.FindByLoginAsync(command.Login).ConfigureAwait(false)
            ?? throw new UnauthorizedException("Invalid login or password.");

        if (!PasswordHasher.Verify(command.Password, auth.PasswordHash))
            throw new UnauthorizedException("Invalid login or password.");

        User? user = await userRepository.FindAsync(auth.UserId).ConfigureAwait(false)
            ?? throw new UnauthorizedException("Invalid login or password.");

        if (user.Status is not UserStatus.Active)
            throw new ForbiddenException("User account is inactive.");

        return await TokenIssueHelper.IssueAsync(
                user,
                tokenIssuer,
                refreshTokenRepository,
                idGenerator,
                this._refreshTokenOptions)
            .ConfigureAwait(false);
    }
}

