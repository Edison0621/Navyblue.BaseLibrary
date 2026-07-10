using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Users;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Handles <see cref="DeleteUserCommand" /> — removes user and related auth credentials.
/// </summary>
public sealed class DeleteUserCommandHandler : CommandHandler<DeleteUserCommand, IdCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthRepository _authRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository, IAuthRepository authRepository)
    {
        this._userRepository = userRepository;
        this._authRepository = authRepository;
    }

    protected override async Task<IdCommandResult> ProcessRequest(DeleteUserCommand command)
    {
        User? user = await this._userRepository.FindAsync(command.UserId)
            ?? throw new NotFoundException($"User '{command.UserId}' was not found.", "user_not_found");

        Domain.Authentication.Auth? auth = await this._authRepository.FindByUserIdAsync(command.UserId);
        if (auth is not null)
            this._authRepository.Remove(auth);

        this._userRepository.Remove(user);
        return new IdCommandResult(user.Id.ToString());
    }
}
