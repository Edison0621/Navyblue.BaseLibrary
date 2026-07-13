namespace NavyblueWebApi.Application.Users;

/// <summary>
///     Cache key helpers for user read models (Redis / <c>IDistributedCacheProvider</c>).
/// </summary>
public static class UserCacheKeys
{
    public static string ById(long userId) => $"user:id:{userId}";
}
