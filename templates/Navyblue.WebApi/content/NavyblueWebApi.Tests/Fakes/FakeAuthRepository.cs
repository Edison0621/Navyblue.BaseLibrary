// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : FakeAuthRepository.cs
// Created          : 2026-07-13  11:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="FakeAuthRepository.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Authentication;

namespace NavyblueWebApi.Tests.Fakes;

internal sealed class FakeAuthRepository : IAuthRepository
{
    private readonly Dictionary<long, Auth> _byUserId = new();

    #region IAuthRepository Members

    public ValueTask<Auth?> FindByLoginAsync(string login, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(this._byUserId.Values.FirstOrDefault(a =>
            string.Equals(a.Login, login, StringComparison.OrdinalIgnoreCase)));

    public ValueTask<Auth?> FindByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        this._byUserId.TryGetValue(userId, out Auth? auth);
        return ValueTask.FromResult(auth);
    }

    public Task AddAsync(Auth auth, CancellationToken cancellationToken = default)
    {
        this._byUserId[auth.UserId] = auth;
        return Task.CompletedTask;
    }

    public void Update(Auth auth) => this._byUserId[auth.UserId] = auth;

    public void Remove(Auth auth) => this._byUserId.Remove(auth.UserId);

    #endregion
}