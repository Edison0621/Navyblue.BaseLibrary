// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : IAuthRepository.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="IAuthRepository.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using NavyblueWebApi.Domain.Authentication;

namespace NavyblueWebApi.Application.Authentication;

/// <summary>
///     Persistence abstraction for the <see cref="Auth" /> credential entity.
/// </summary>
public interface IAuthRepository
{
    ValueTask<Auth?> FindByLoginAsync(string login, CancellationToken cancellationToken = default);

    ValueTask<Auth?> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    Task AddAsync(Auth auth, CancellationToken cancellationToken = default);

    void Update(Auth auth);

    void Remove(Auth auth);
}