using Microsoft.Extensions.Options;
using NavyblueWebApi.Application.Users;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;
using Navyblue.Foundation.Security;

namespace NavyblueWebApi.Application.Authentication.Commands;

/// <summary>
///     Login: verify password, issue access JWT + refresh token (persisted hash).
/// </summary>
public sealed class AuthCommandHandler : CommandHandler<AuthCommand, AuthCommandResult>
{
    private readonly IAuthRepository _authRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenIssuer _tokenIssuer;
    private readonly IIdGenerator<long> _idGenerator;
    private readonly RefreshTokenOptions _refreshTokenOptions;

    public AuthCommandHandler(
        IAuthRepository authRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenIssuer tokenIssuer,
        IIdGenerator<long> idGenerator,
        IOptions<RefreshTokenOptions> refreshTokenOptions)
    {
        this._authRepository = authRepository;
        this._userRepository = userRepository;
        this._refreshTokenRepository = refreshTokenRepository;
        this._tokenIssuer = tokenIssuer;
        this._idGenerator = idGenerator;
        this._refreshTokenOptions = refreshTokenOptions.Value;
    }

    protected override async Task<AuthCommandResult> ProcessRequest(AuthCommand command)
    {
        Auth? auth = await this._authRepository.FindByLoginAsync(command.Login)
            ?? throw new UnauthorizedException("Invalid login or password.");

        if (!PasswordHasher.Verify(command.Password, auth.PasswordHash))
            throw new UnauthorizedException("Invalid login or password.");

        User? user = await this._userRepository.FindAsync(auth.UserId)
            ?? throw new UnauthorizedException("Invalid login or password.");

        if (user.Status != UserStatus.Active)
            throw new ForbiddenException("User account is inactive.");

        return await TokenIssueHelper.IssueAsync(
                user,
                this._tokenIssuer,
                this._refreshTokenRepository,
                this._idGenerator,
                this._refreshTokenOptions)
            .ConfigureAwait(false);
    }
}
