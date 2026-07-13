using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;
using Navyblue.Foundation.Security;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Handles <see cref="AddUserCommand" />: creates the <see cref="User" /> aggregate and optional <see cref="Auth" />.
/// </summary>
public sealed class AddUserCommandHandler : CommandHandler<AddUserCommand, IdCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IIdGenerator<long> _idGenerator;
    private readonly ICurrentUser _currentUser;

    public AddUserCommandHandler(
        IUserRepository userRepository,
        IAuthRepository authRepository,
        IIdGenerator<long> idGenerator,
        ICurrentUser currentUser)
    {
        this._userRepository = userRepository;
        this._authRepository = authRepository;
        this._idGenerator = idGenerator;
        this._currentUser = currentUser;
    }

    protected override async Task<IdCommandResult> ProcessRequest(AddUserCommand command)
    {
        User? existing = await this._userRepository.FindByEmailAsync(command.Email);
        if (existing is not null)
            throw new BusinessException($"A user with email '{command.Email}' already exists.", "user_email_duplicate");

        long userId = this._idGenerator.NextId();
        string? actor = this._currentUser.UserId ?? this._currentUser.UserName;
        User user = new(userId, command.Name, command.Email, actor);
        await this._userRepository.AddAsync(user);

        if (!string.IsNullOrWhiteSpace(command.Password))
        {
            string hash = PasswordHasher.Hash(command.Password);
            string salt = ExtractSalt(hash);
            Auth auth = new(this._idGenerator.NextId(), userId, command.Email, hash, salt);
            await this._authRepository.AddAsync(auth);
        }

        return new IdCommandResult(userId.ToString());
    }

    private static string ExtractSalt(string stored)
    {
        string[] parts = stored.Split('$');
        return parts.Length == 4 ? parts[2] : string.Empty;
    }
}
