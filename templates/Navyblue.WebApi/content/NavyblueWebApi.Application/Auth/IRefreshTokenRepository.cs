// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : IRefreshTokenRepository.cs
// Created          : 2026-07-13  11:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="IRefreshTokenRepository.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using NavyblueWebApi.Domain.Authentication;

namespace NavyblueWebApi.Application.Authentication;

/// <summary>
///     Persistence for <see cref="RefreshToken" /> rows (hashed values only).
/// </summary>
public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);

    ValueTask<RefreshToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RefreshToken>> ListActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    void Update(RefreshToken token);
}