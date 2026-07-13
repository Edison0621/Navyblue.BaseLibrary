using NavyblueWebApi.Domain.Authentication;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;

namespace NavyblueWebApi.Application.Authentication.Commands;

/// <summary>
///     Revokes a refresh token (logout). Idempotent when the token is unknown/already revoked.
/// </summary>
public sealed class RevokeRefreshTokenCommand : Command<IdCommandResult>
{
    public RevokeRefreshTokenCommand(string refreshToken) => this.RefreshToken = refreshToken ?? string.Empty;

    public string RefreshToken { get; }

    public override string DisplayName => "RevokeRefreshToken";

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

public sealed class RevokeRefreshTokenCommandHandler : CommandHandler<RevokeRefreshTokenCommand, IdCommandResult>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RevokeRefreshTokenCommandHandler(IRefreshTokenRepository refreshTokenRepository)
        => this._refreshTokenRepository = refreshTokenRepository;

    protected override async Task<IdCommandResult> ProcessRequest(RevokeRefreshTokenCommand command)
    {
        string hash = RefreshTokenProtector.Hash(command.RefreshToken.Trim());
        RefreshToken? existing = await this._refreshTokenRepository.FindByTokenHashAsync(hash).ConfigureAwait(false);
        if (existing is null || existing.IsRevoked)
            return new IdCommandResult("revoked");

        existing.Revoke();
        this._refreshTokenRepository.Update(existing);
        return new IdCommandResult(existing.Id.ToString());
    }
}
