using Navyblue.Samples.Domain.Users;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;

namespace Navyblue.Samples.Application.Users.Commands;

/// <summary>
///     Handles <see cref="ActivateUserCommand" />.
/// </summary>
public sealed class ActivateUserCommandHandler : CommandHandler<ActivateUserCommand, IdCommandResult>
{
    private readonly IUserRepository _userRepository;

    public ActivateUserCommandHandler(IUserRepository userRepository) => this._userRepository = userRepository;

    protected override async Task<IdCommandResult> ProcessRequest(ActivateUserCommand command)
    {
        User? user = await this._userRepository.FindAsync(command.UserId)
            ?? throw new NotFoundException($"User '{command.UserId}' was not found.", "user_not_found");

        user.Activate();
        this._userRepository.Update(user);
        return new IdCommandResult(user.Id.ToString());
    }
}
