using NavyblueWebApi.Domain.Users;
using NavyblueWebApi.Model.Users;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Cqrs;

namespace NavyblueWebApi.Application.Users.Queries;

/// <summary>
///     Handles <see cref="GetUserQuery" /> with Redis cache-aside for the <see cref="UserModel" />.
/// </summary>
public sealed class GetUserQueryHandler : QueryHandler<GetUserQuery, UserModel?>
{
    private readonly IUserRepository _userRepository;
    private readonly IDistributedCacheProvider _cache;

    public GetUserQueryHandler(IUserRepository userRepository, IDistributedCacheProvider cache)
    {
        this._userRepository = userRepository;
        this._cache = cache;
    }

    protected override async Task<UserModel?> ProcessRequest(GetUserQuery query)
    {
        string cacheKey = UserCacheKeys.ById(query.UserId);
        UserModel? cached = await this._cache.GetAsync<UserModel>(cacheKey).ConfigureAwait(false);
        if (cached is not null)
            return cached;

        User? user = await this._userRepository.FindAsync(query.UserId).ConfigureAwait(false);
        if (user is null)
            return null;

        UserModel model = ToModel(user);
        await this._cache.SetAsync(
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
