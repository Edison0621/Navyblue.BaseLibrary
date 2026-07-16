using Navyblue.Samples.Application.Authentication;
using Navyblue.Samples.Domain.Users;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;

namespace Navyblue.Samples.Application.Users.Commands;

/// <summary>
///     Handles <see cref="UpdateUserCommand" /> — keeps Auth.Login in sync with email.
/// </summary>
public sealed class UpdateUserCommandHandler : CommandHandler<UpdateUserCommand, IdCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthRepository _authRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository, IAuthRepository authRepository)
    {
        this._userRepository = userRepository;
        this._authRepository = authRepository;
    }

    protected override async Task<IdCommandResult> ProcessRequest(UpdateUserCommand command)
    {
        User? user = await this._userRepository.FindAsync(command.UserId)
            ?? throw new NotFoundException($"User '{command.UserId}' was not found.", "user_not_found");

        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            string email = command.Email.Trim();
            User? existing = await this._userRepository.FindByEmailAsync(email);
            if (existing is not null && existing.Id != user.Id)
                throw new BusinessException($"Email '{email}' is already in use.", "email_taken");

            user.ChangeEmail(email);

            Domain.Authentication.Auth? auth = await this._authRepository.FindByUserIdAsync(user.Id);
            if (auth is not null)
            {
                auth.ChangeLogin(email);
                this._authRepository.Update(auth);
            }
        }

        if (!string.IsNullOrWhiteSpace(command.Name))
            user.Rename(command.Name);

        this._userRepository.Update(user);
        return new IdCommandResult(user.Id.ToString());
    }
}
