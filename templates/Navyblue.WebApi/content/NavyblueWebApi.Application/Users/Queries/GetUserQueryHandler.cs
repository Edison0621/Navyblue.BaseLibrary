using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Cqrs;
using NavyblueWebApi.Domain.Users;
using NavyblueWebApi.Model.Users;

namespace NavyblueWebApi.Application.Users.Queries;

/// <summary>
///     Returns a single user by id, or null when not found.
/// </summary>
public sealed class GetUserQuery(long userId) : Query<UserModel?>
{
    public long UserId { get; } = userId;

    public override string DisplayName => "GetUser";

    public override string Id { get; } = Guid.NewGuid().ToString("N");

    public override bool Validate(out string validationErrorMessage)
    {
        if (this.UserId <= 0)
        {
            validationErrorMessage = "A positive user Id is required.";
            return false;
        }

        validationErrorMessage = string.Empty;
        return true;
    }
}

/// <summary>
///     Handles <see cref="GetUserQuery" /> with Redis cache-aside for the <see cref="UserModel" />.
/// </summary>
public sealed class GetUserQueryHandler(IUserRepository userRepository, IDistributedCacheProvider cache)
    : QueryHandler<GetUserQuery, UserModel?>
{
    protected override async Task<UserModel?> ProcessRequest(GetUserQuery query)
    {
        string cacheKey = UserCacheKeys.ById(query.UserId);
        UserModel? cached = await cache.GetAsync<UserModel>(cacheKey).ConfigureAwait(false);
        if (cached is not null)
            return cached;

        User? user = await userRepository.FindAsync(query.UserId).ConfigureAwait(false);
        if (user is null)
            return null;

        UserModel model = ToModel(user);
        await cache.SetAsync(
                cacheKey,
                model,
                new CacheEntryOptions(AbsoluteExpirationRelativeToNow: TimeSpan.FromMinutes(10)))
            .ConfigureAwait(false);
        return model;
    }

    public static UserModel ToModel(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Status = (int)user.Status,
        CreatedAt = user.CreatedAt,
        CreatedBy = user.CreatedBy,
        ModifiedAt = user.ModifiedAt,
        ModifiedBy = user.ModifiedBy
    };
}

