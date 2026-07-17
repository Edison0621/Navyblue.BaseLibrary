using Navyblue.Foundation.Application;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;
using NavyblueWebApi.Domain.Users;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Reactivates an inactive user.
/// </summary>
public sealed class ActivateUserCommand(long userId) : Command<IdCommandResult>
{
    public long UserId { get; } = userId;

    public override string DisplayName => "ActivateUser";

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
///     Handles <see cref="ActivateUserCommand" /> and invalidates Redis cache.
/// </summary>
public sealed class ActivateUserCommandHandler(
    IUserRepository userRepository,
    IDistributedCacheProvider cache,
    ICurrentUser currentUser)
    : CommandHandler<ActivateUserCommand, IdCommandResult>
{
    protected override async Task<IdCommandResult> ProcessRequest(ActivateUserCommand command)
    {
        User? user = await userRepository.FindAsync(command.UserId).ConfigureAwait(false)
            ?? throw new NotFoundException($"User '{command.UserId}' was not found.", "user_not_found");

        user.Activate(currentUser.UserId ?? currentUser.UserName);
        userRepository.Update(user);
        await cache.RemoveAsync(UserCacheKeys.ById(user.Id)).ConfigureAwait(false);
        return new IdCommandResult(user.Id.ToString());
    }
}

