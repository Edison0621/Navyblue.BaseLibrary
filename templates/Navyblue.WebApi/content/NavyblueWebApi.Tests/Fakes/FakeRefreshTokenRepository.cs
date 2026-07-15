// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : FakeRefreshTokenRepository.cs
// Created          : 2026-07-13  11:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="FakeRefreshTokenRepository.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Authentication;

namespace NavyblueWebApi.Tests.Fakes;

internal sealed class FakeRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ConcurrentDictionary<string, RefreshToken> _byHash = new(StringComparer.Ordinal);

    #region IRefreshTokenRepository Members

    public Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        if (!this._byHash.TryAdd(token.TokenHash, token))
            throw new InvalidOperationException("Duplicate refresh token hash.");
        return Task.CompletedTask;
    }

    public ValueTask<RefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        this._byHash.TryGetValue(tokenHash, out RefreshToken? token);
        return ValueTask.FromResult(token);
    }

    public Task<IReadOnlyList<RefreshToken>> ListActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RefreshToken> list = this._byHash.Values
            .Where(t => t.UserId == userId && t.IsActive())
            .ToList();
        return Task.FromResult(list);
    }

    public void Update(RefreshToken token) => this._byHash[token.TokenHash] = token;

    #endregion
}