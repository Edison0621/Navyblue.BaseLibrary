using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Users;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Soft-deletes the user, removes auth credentials, and clears Redis cache.
/// </summary>
public sealed class DeleteUserCommandHandler : CommandHandler<DeleteUserCommand, IdCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IDistributedCacheProvider _cache;
    private readonly ICurrentUser _currentUser;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        IAuthRepository authRepository,
        IDistributedCacheProvider cache,
        ICurrentUser currentUser)
    {
        this._userRepository = userRepository;
        this._authRepository = authRepository;
        this._cache = cache;
        this._currentUser = currentUser;
    }

    protected override async Task<IdCommandResult> ProcessRequest(DeleteUserCommand command)
    {
        User? user = await this._userRepository.FindAsync(command.UserId)
            ?? throw new NotFoundException($"User '{command.UserId}' was not found.", "user_not_found");

        Domain.Authentication.Auth? auth = await this._authRepository.FindByUserIdAsync(command.UserId);
        if (auth is not null)
            this._authRepository.Remove(auth);

        user.SoftDelete(this._currentUser.UserId ?? this._currentUser.UserName);
        this._userRepository.Update(user);
        await this._cache.RemoveAsync(UserCacheKeys.ById(user.Id)).ConfigureAwait(false);
        return new IdCommandResult(user.Id.ToString());
    }
}
