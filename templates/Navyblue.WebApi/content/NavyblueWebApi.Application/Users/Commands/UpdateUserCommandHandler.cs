using Navyblue.Foundation.Application;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Updates an existing user's name and/or email.
/// </summary>
public sealed class UpdateUserCommand(long userId, string? name, string? email) : Command<IdCommandResult>
{
    /// <summary>Identifier of the user to update.</summary>
    public long UserId { get; } = userId;

    public string? Name { get; } = name;

    public string? Email { get; } = email;

    public override string DisplayName => "UpdateUser";

    public override string Id { get; } = Guid.NewGuid().ToString("N");

    public override bool Validate(out string validationErrorMessage)
    {
        if (this.UserId <= 0)
        {
            validationErrorMessage = "A positive user Id is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(this.Name) && string.IsNullOrWhiteSpace(this.Email))
        {
            validationErrorMessage = "At least one of Name or Email must be provided.";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(this.Email) && !this.Email.Contains('@', StringComparison.Ordinal))
        {
            validationErrorMessage = "A valid Email is required.";
            return false;
        }

        validationErrorMessage = string.Empty;
        return true;
    }
}

/// <summary>
///     Handles <see cref="UpdateUserCommand" /> — keeps Auth.Login in sync with email and invalidates Redis cache.
/// </summary>
public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IAuthRepository authRepository,
    IDistributedCacheProvider cache,
    ICurrentUser currentUser)
    : CommandHandler<UpdateUserCommand, IdCommandResult>
{
    protected override async Task<IdCommandResult> ProcessRequest(UpdateUserCommand command)
    {
        User? user = await userRepository.FindAsync(command.UserId).ConfigureAwait(false)
            ?? throw new NotFoundException($"User '{command.UserId}' was not found.", "user_not_found");

        string? actor = currentUser.UserId ?? currentUser.UserName;

        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            string email = command.Email.Trim();
            User? existing = await userRepository.FindByEmailAsync(email).ConfigureAwait(false);
            if (existing is not null && existing.Id != user.Id)
                throw new BusinessException($"Email '{email}' is already in use.", "email_taken");

            user.ChangeEmail(email, actor);

            Auth? auth = await authRepository.FindByUserIdAsync(user.Id).ConfigureAwait(false);
            if (auth is not null)
            {
                auth.ChangeLogin(email);
                authRepository.Update(auth);
            }
        }

        if (!string.IsNullOrWhiteSpace(command.Name))
            user.Rename(command.Name, actor);

        userRepository.Update(user);
        await cache.RemoveAsync(UserCacheKeys.ById(user.Id)).ConfigureAwait(false);
        return new IdCommandResult(user.Id.ToString());
    }
}

