using Microsoft.Extensions.Options;
using NavyblueWebApi.Application.Users;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;

namespace NavyblueWebApi.Application.Authentication.Commands;

/// <summary>
///     Exchanges a valid refresh token for a new access + refresh token pair (rotation).
/// </summary>
public sealed class RefreshTokenCommand : Command<AuthCommandResult>
{
    public RefreshTokenCommand(string refreshToken) => this.RefreshToken = refreshToken ?? string.Empty;

    public string RefreshToken { get; }

    public override string DisplayName => "RefreshToken";

    public override string Id { get; } = Guid.NewGuid().ToString("N");

    public override bool Validate(out string validationErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(this.RefreshToken))
        {
            validationErrorMessage = "Refresh token is required.";
            return false;
        }

        validationErrorMessage = string.Empty;
        return true;
    }
}

public sealed class RefreshTokenCommandHandler : CommandHandler<RefreshTokenCommand, AuthCommandResult>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenIssuer _tokenIssuer;
    private readonly IIdGenerator<long> _idGenerator;
    private readonly RefreshTokenOptions _refreshTokenOptions;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ITokenIssuer tokenIssuer,
        IIdGenerator<long> idGenerator,
        IOptions<RefreshTokenOptions> refreshTokenOptions)
    {
        this._refreshTokenRepository = refreshTokenRepository;
        this._userRepository = userRepository;
        this._tokenIssuer = tokenIssuer;
        this._idGenerator = idGenerator;
        this._refreshTokenOptions = refreshTokenOptions.Value;
    }

    protected override async Task<AuthCommandResult> ProcessRequest(RefreshTokenCommand command)
    {
        string hash = RefreshTokenProtector.Hash(command.RefreshToken.Trim());
        RefreshToken? existing = await this._refreshTokenRepository.FindByTokenHashAsync(hash)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!existing.IsActive())
            throw new UnauthorizedException("Refresh token is expired or revoked.");

        User? user = await this._userRepository.FindAsync(existing.UserId)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (user.Status != UserStatus.Active)
            throw new ForbiddenException("User account is inactive.");

        AuthCommandResult issued = await TokenIssueHelper.IssueAsync(
                user,
                this._tokenIssuer,
                this._refreshTokenRepository,
                this._idGenerator,
                this._refreshTokenOptions)
            .ConfigureAwait(false);

        existing.Revoke(RefreshTokenProtector.Hash(issued.RefreshToken));
        this._refreshTokenRepository.Update(existing);

        return issued;
    }
}
