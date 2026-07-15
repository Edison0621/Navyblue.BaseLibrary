// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : FakeUserRepository.cs
// Created          : 2026-07-13  11:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="FakeUserRepository.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Navyblue.Foundation.Data;
using NavyblueWebApi.Application.Users;
using NavyblueWebApi.Domain.Users;

namespace NavyblueWebApi.Tests.Fakes;

internal sealed class FakeUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<long, User> _store = new();

    private IEnumerable<User> Active => this._store.Values.Where(u => !u.IsDeleted);

    #region IUserRepository Members

    public ValueTask<User?> FindAsync(long id, CancellationToken cancellationToken = default)
    {
        this._store.TryGetValue(id, out User? user);
        return ValueTask.FromResult(user is { IsDeleted: false } ? user : null);
    }

    public ValueTask<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        User? user = this.Active.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        return ValueTask.FromResult(user);
    }

    public Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<User>>(this.Active.OrderBy(u => u.Id).ToList());

    public Task<PageData<User>> PageAsync(PageQuery page, string? keyword = null, CancellationToken cancellationToken = default)
    {
        IEnumerable<User> query = this.Active;
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            string key = keyword.Trim();
            query = query.Where(u =>
                u.Name.Contains(key, StringComparison.OrdinalIgnoreCase)
                || u.Email.Contains(key, StringComparison.OrdinalIgnoreCase));
        }

        List<User> all = query.OrderBy(u => u.Id).ToList();
        int pageIndex = Math.Max(page.PageIndex, 1);
        int pageSize = Math.Clamp(page.PageSize, 1, 200);
        List<User> items = all.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PageData<User>(items, all.Count, pageIndex, pageSize));
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        if (!this._store.TryAdd(user.Id, user))
            throw new InvalidOperationException($"User {user.Id} already exists.");
        return Task.CompletedTask;
    }

    public void Update(User user) => this._store[user.Id] = user;

    public void Remove(User user) => this._store.TryRemove(user.Id, out _);

    #endregion
}