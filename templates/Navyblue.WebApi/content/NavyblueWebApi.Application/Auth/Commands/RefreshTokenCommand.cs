// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : RefreshTokenCommand.cs
// Created          : 2026-07-13  11:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="RefreshTokenCommand.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.Options;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;
using NavyblueWebApi.Application.Users;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;

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

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    ITokenIssuer tokenIssuer,
    IIdGenerator<long> idGenerator,
    IOptions<RefreshTokenOptions> refreshTokenOptions)
    : CommandHandler<RefreshTokenCommand, AuthCommandResult>

{
    private readonly RefreshTokenOptions _refreshTokenOptions = refreshTokenOptions.Value;

    protected override async Task<AuthCommandResult> ProcessRequest(RefreshTokenCommand command)

    {
        string hash = RefreshTokenProtector.Hash(command.RefreshToken.Trim());

        RefreshToken? existing = await refreshTokenRepository.FindByTokenHashAsync(hash).ConfigureAwait(false)
                                 ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!existing.IsActive())

            throw new UnauthorizedException("Refresh token is expired or revoked.");

        User? user = await userRepository.FindAsync(existing.UserId).ConfigureAwait(false)
                     ?? throw new UnauthorizedException("Invalid refresh token.");

        if (user.Status is not UserStatus.Active)

            throw new ForbiddenException("User account is inactive.");

        AuthCommandResult issued = await TokenIssueHelper.IssueAsync(
                user,
                tokenIssuer,
                refreshTokenRepository,
                idGenerator,
                this._refreshTokenOptions)
            .ConfigureAwait(false);

        existing.Revoke(RefreshTokenProtector.Hash(issued.RefreshToken));

        refreshTokenRepository.Update(existing);

        return issued;
    }
}