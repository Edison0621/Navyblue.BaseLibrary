using Navyblue.Foundation.Application;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;
using Navyblue.Foundation.Security;
using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Creates a new user. Returns the generated user id.
/// </summary>
public sealed class AddUserCommand(string name, string email, string? password = null) : Command<IdCommandResult>
{
    public string Name { get; } = name;

    public string Email { get; } = email;

    /// <summary>Optional initial password. When supplied, an Auth credential is created alongside the user.</summary>
    public string? Password { get; } = password;

    public override string DisplayName => "AddUser";

    public override string Id { get; } = Guid.NewGuid().ToString("N");

    public override bool Validate(out string validationErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(this.Name))
        {
            validationErrorMessage = "Name is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(this.Email) || !this.Email.Contains('@', StringComparison.Ordinal))
        {
            validationErrorMessage = "A valid Email is required.";
            return false;
        }

        validationErrorMessage = string.Empty;
        return true;
    }
}

/// <summary>
///     Handles <see cref="AddUserCommand" />: creates the <see cref="User" /> aggregate and optional <see cref="Auth" />.
/// </summary>
public sealed class AddUserCommandHandler(
    IUserRepository userRepository,
    IAuthRepository authRepository,
    IIdGenerator<long> idGenerator,
    ICurrentUser currentUser)
    : CommandHandler<AddUserCommand, IdCommandResult>
{
    protected override async Task<IdCommandResult> ProcessRequest(AddUserCommand command)
    {
        User? existing = await userRepository.FindByEmailAsync(command.Email).ConfigureAwait(false);
        if (existing is not null)
            throw new BusinessException($"A user with email '{command.Email}' already exists.", "user_email_duplicate");

        long userId = idGenerator.NextId();
        string? actor = currentUser.UserId ?? currentUser.UserName;
        User user = new(userId, command.Name, command.Email, actor);
        await userRepository.AddAsync(user).ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(command.Password))
        {
            string hash = PasswordHasher.Hash(command.Password);
            string salt = PasswordHashFormat.ExtractSalt(hash);
            Auth auth = new(idGenerator.NextId(), userId, command.Email, hash, salt);
            await authRepository.AddAsync(auth).ConfigureAwait(false);
        }

        return new IdCommandResult(userId.ToString());
    }
}

