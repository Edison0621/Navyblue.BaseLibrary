// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : IUserRepository.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="IUserRepository.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Data;
using NavyblueWebApi.Domain.Users;

namespace NavyblueWebApi.Application.Users;

/// <summary>
///     Persistence abstraction for the <see cref="User" /> aggregate.
/// </summary>
public interface IUserRepository
{
    ValueTask<User?> FindAsync(long id, CancellationToken cancellationToken = default);

    ValueTask<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default);

    Task<PageData<User>> PageAsync(PageQuery page, string? keyword = null, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    void Update(User user);

    void Remove(User user);
}