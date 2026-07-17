using Navyblue.Foundation.Application;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;
using NavyblueWebApi.Domain.Users;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Marks a user inactive without deleting the record.
/// </summary>
public sealed class InactivateUserCommand(long userId) : Command<IdCommandResult>
{
    public long UserId { get; } = userId;

    public override string DisplayName => "InactivateUser";

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
///     Handles <see cref="InactivateUserCommand" /> and invalidates Redis cache.
/// </summary>
public sealed class InactivateUserCommandHandler(
    IUserRepository userRepository,
    IDistributedCacheProvider cache,
    ICurrentUser currentUser)
    : CommandHandler<InactivateUserCommand, IdCommandResult>
{
    protected override async Task<IdCommandResult> ProcessRequest(InactivateUserCommand command)
    {
        User? user = await userRepository.FindAsync(command.UserId).ConfigureAwait(false)
            ?? throw new NotFoundException($"User '{command.UserId}' was not found.", "user_not_found");

        user.Inactivate(currentUser.UserId ?? currentUser.UserName);
        userRepository.Update(user);
        await cache.RemoveAsync(UserCacheKeys.ById(user.Id)).ConfigureAwait(false);
        return new IdCommandResult(user.Id.ToString());
    }
}

