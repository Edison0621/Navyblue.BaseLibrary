using Navyblue.Foundation.Application;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Data;
using Navyblue.Foundation.Testing;
using NavyblueWebApi.Application.Users.Queries;
using NavyblueWebApi.Domain.Users;
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
        PageData<Model.Users.UserModel> page = await handler.Handle(new ListUsersQuery("Alice", pageIndex: 1, pageSize: 10));

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
        Model.Users.UserModel? first = await handler.Handle(new GetUserQuery(10));
        users.Remove((await users.FindAsync(10))!);

        Model.Users.UserModel? cached = await handler.Handle(new GetUserQuery(10));

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
        await cache.SetAsync(Application.Users.UserCacheKeys.ById(5), new Model.Users.UserModel { Id = 5, Name = "stale" });

        Application.Users.Commands.ActivateUserCommandHandler handler = new(users, cache, CurrentUser.Anonymous);
        await handler.Handle(new Application.Users.Commands.ActivateUserCommand(5));

        User? updated = await users.FindAsync(5);
        Assert.Equal(UserStatus.Active, updated!.Status);
        Assert.Null(await cache.GetAsync<Model.Users.UserModel>(Application.Users.UserCacheKeys.ById(5)));
    }
}

public sealed class SoftDeleteUserCommandHandlerTests
{
    [Fact]
    public async Task SoftDelete_Hides_User_From_Queries()
    {
        FakeUserRepository users = new();
        Fakes.FakeAuthRepository auths = new();
        await users.AddAsync(new User(9, "Gone", "gone@navyblue.local"));
        InMemoryCacheProvider cache = new();

        Application.Users.Commands.DeleteUserCommandHandler handler = new(users, auths, cache, CurrentUser.Anonymous);
        await handler.Handle(new Application.Users.Commands.DeleteUserCommand(9));

        Assert.Null(await users.FindAsync(9));
        Assert.Empty(await users.ListAsync());
    }
}
