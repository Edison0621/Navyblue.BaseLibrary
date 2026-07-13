using NavyblueWebApi.Domain.Users;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Handles <see cref="InactivateUserCommand" /> and invalidates Redis cache.
/// </summary>
public sealed class InactivateUserCommandHandler : CommandHandler<InactivateUserCommand, IdCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IDistributedCacheProvider _cache;
    private readonly ICurrentUser _currentUser;

    public InactivateUserCommandHandler(
        IUserRepository userRepository,
        IDistributedCacheProvider cache,
        ICurrentUser currentUser)
    {
        this._userRepository = userRepository;
        this._cache = cache;
        this._currentUser = currentUser;
    }

    protected override async Task<IdCommandResult> ProcessRequest(InactivateUserCommand command)
    {
        User? user = await this._userRepository.FindAsync(command.UserId)
            ?? throw new NotFoundException($"User '{command.UserId}' was not found.", "user_not_found");

        user.Inactivate(this._currentUser.UserId ?? this._currentUser.UserName);
        this._userRepository.Update(user);
        await this._cache.RemoveAsync(UserCacheKeys.ById(user.Id)).ConfigureAwait(false);
        return new IdCommandResult(user.Id.ToString());
    }
}
