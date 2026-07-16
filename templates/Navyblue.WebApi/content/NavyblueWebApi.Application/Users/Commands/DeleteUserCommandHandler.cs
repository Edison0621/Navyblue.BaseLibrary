using Navyblue.Foundation.Application;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Soft-deletes a user permanently from the active set (row retained).
/// </summary>
public sealed class DeleteUserCommand : Command<IdCommandResult>
{
    public DeleteUserCommand(long userId) => this.UserId = userId;

    public long UserId { get; }

    public override string DisplayName => "DeleteUser";

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
///     Soft-deletes the user, removes auth credentials, and clears Redis cache.
/// </summary>
public sealed class DeleteUserCommandHandler(
    IUserRepository userRepository,
    IAuthRepository authRepository,
    IDistributedCacheProvider cache,
    ICurrentUser currentUser)
    : CommandHandler<DeleteUserCommand, IdCommandResult>
{
    protected override async Task<IdCommandResult> ProcessRequest(DeleteUserCommand command)
    {
        User? user = await userRepository.FindAsync(command.UserId).ConfigureAwait(false)
            ?? throw new NotFoundException($"User '{command.UserId}' was not found.", "user_not_found");

        Auth? auth = await authRepository.FindByUserIdAsync(command.UserId).ConfigureAwait(false);
        if (auth is not null)
            authRepository.Remove(auth);

        user.SoftDelete(currentUser.UserId ?? currentUser.UserName);
        userRepository.Update(user);
        await cache.RemoveAsync(UserCacheKeys.ById(user.Id)).ConfigureAwait(false);
        return new IdCommandResult(user.Id.ToString());
    }
}

