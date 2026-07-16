// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : UserQueryHandlerTests.cs
// Created          : 2026-07-13  11:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="UserQueryHandlerTests.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Application;
using Navyblue.Foundation.Data;
using Navyblue.Foundation.Testing;
using NavyblueWebApi.Application.Users;
using NavyblueWebApi.Application.Users.Commands;
using NavyblueWebApi.Application.Users.Queries;
using NavyblueWebApi.Domain.Users;
using NavyblueWebApi.Model.Users;
using NavyblueWebApi.Tests.Fakes;
using Xunit;

namespace NavyblueWebApi.Tests;

public sealed class ListUsersQueryHandlerTests
{
    [Fact]
    public async Task PageAsync_Filters_And_Pages()
    {
        FakeUserRepository users = new();
        await users.AddAsync(new User(1, "Alice", "a@navyblue.local"));
        await users.AddAsync(new User(2, "Bob", "b@navyblue.local"));
        await users.AddAsync(new User(3, "Carol", "carol@navyblue.local"));

        ListUsersQueryHandler handler = new(users);
        PageData<UserModel> page = await handler.Handle(new ListUsersQuery("Alice", pageIndex: 1, pageSize: 10));

        Assert.Equal(1, page.Total);
        Assert.Single(page.Items);
        Assert.Equal("Alice", page.Items[0].Name);
    }
}

public sealed class GetUserQueryHandlerTests
{
    [Fact]
    public async Task Get_Caches_UserModel()
    {
        FakeUserRepository users = new();
        await users.AddAsync(new User(10, "Admin", "admin@navyblue.local"));
        InMemoryCacheProvider cache = new();

        GetUserQueryHandler handler = new(users, cache);
        UserModel? first = await handler.Handle(new GetUserQuery(10));
        users.Remove((await users.FindAsync(10))!);

        UserModel? cached = await handler.Handle(new GetUserQuery(10));

        Assert.NotNull(first);
        Assert.NotNull(cached);
        Assert.Equal("Admin", cached!.Name);
    }
}

public sealed class ActivateUserCommandHandlerTests
{
    [Fact]
    public async Task Activate_Updates_Status_And_Clears_Cache()
    {
        FakeUserRepository users = new();
        User user = new(5, "Demo", "demo@navyblue.local");
        user.Inactivate();
        await users.AddAsync(user);

        InMemoryCacheProvider cache = new();
        await cache.SetAsync(UserCacheKeys.ById(5), new UserModel
        {
            Id = 5,
            Name = "stale",
            Email = "demo@navyblue.local",
            Status = 1,
            CreatedAt = DateTimeOffset.UtcNow
        });

        ActivateUserCommandHandler handler = new(users, cache, CurrentUser.Anonymous);
        await handler.Handle(new ActivateUserCommand(5));

        User? updated = await users.FindAsync(5);
        Assert.Equal(UserStatus.Active, updated!.Status);
        Assert.Null(await cache.GetAsync<UserModel>(UserCacheKeys.ById(5)));
    }
}

public sealed class SoftDeleteUserCommandHandlerTests
{
    [Fact]
    public async Task SoftDelete_Hides_User_From_Queries()
    {
        FakeUserRepository users = new();
        FakeAuthRepository auths = new();
        await users.AddAsync(new User(9, "Gone", "gone@navyblue.local"));
        InMemoryCacheProvider cache = new();

        DeleteUserCommandHandler handler = new(users, auths, cache, CurrentUser.Anonymous);
        await handler.Handle(new DeleteUserCommand(9));

        Assert.Null(await users.FindAsync(9));
        Assert.Empty(await users.ListAsync());
    }
}