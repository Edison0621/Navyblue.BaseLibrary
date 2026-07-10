using NavyblueWebApi.Domain.Users;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Handles <see cref="InactivateUserCommand" />.
/// </summary>
public sealed class InactivateUserCommandHandler : CommandHandler<InactivateUserCommand, IdCommandResult>
{
    private readonly IUserRepository _userRepository;

    public InactivateUserCommandHandler(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }

    protected override async Task<IdCommandResult> ProcessRequest(InactivateUserCommand command)
    {
        User? user = await this._userRepository.FindAsync(command.UserId)
            ?? throw new NotFoundException($"User '{command.UserId}' was not found.", "user_not_found");

        user.Inactivate();
        this._userRepository.Update(user);
        return new IdCommandResult(user.Id.ToString());
    }
}
