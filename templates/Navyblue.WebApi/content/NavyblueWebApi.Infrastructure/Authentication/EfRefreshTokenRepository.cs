// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : EfRefreshTokenRepository.cs
// Created          : 2026-07-13  11:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="EfRefreshTokenRepository.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.EntityFrameworkCore;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Infrastructure.Persistence;

namespace NavyblueWebApi.Infrastructure.Authentication;

public sealed class EfRefreshTokenRepository(AppDbContext db) : IRefreshTokenRepository
{
    #region IRefreshTokenRepository Members

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)

    {
        ArgumentNullException.ThrowIfNull(token);

        await db.RefreshTokens.AddAsync(token, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<RefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)

    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        return await db.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<RefreshToken>> ListActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default)

    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        return await db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > now)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public void Update(RefreshToken token)

    {
        ArgumentNullException.ThrowIfNull(token);

        db.RefreshTokens.Update(token);
    }

    #endregion
}