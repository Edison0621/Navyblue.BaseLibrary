using NavyblueWebApi.Application.Authentication;
using NavyblueWebApi.Domain.Users;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Caching;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Handles <see cref="UpdateUserCommand" /> — keeps Auth.Login in sync with email and invalidates Redis cache.
/// </summary>
public sealed class UpdateUserCommandHandler : CommandHandler<UpdateUserCommand, IdCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IDistributedCacheProvider _cache;
    private readonly ICurrentUser _currentUser;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IAuthRepository authRepository,
        IDistributedCacheProvider cache,
        ICurrentUser currentUser)
    {
        this._userRepository = userRepository;
        this._authRepository = authRepository;
        this._cache = cache;
        this._currentUser = currentUser;
    }

    protected override async Task<IdCommandResult> ProcessRequest(UpdateUserCommand command)
    {
        User? user = await this._userRepository.FindAsync(command.UserId)
            ?? throw new NotFoundException($"User '{command.UserId}' was not found.", "user_not_found");

        string? actor = this._currentUser.UserId ?? this._currentUser.UserName;

        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            string email = command.Email.Trim();
            User? existing = await this._userRepository.FindByEmailAsync(email);
            if (existing is not null && existing.Id != user.Id)
                throw new BusinessException($"Email '{email}' is already in use.", "email_taken");

            user.ChangeEmail(email, actor);

            Domain.Authentication.Auth? auth = await this._authRepository.FindByUserIdAsync(user.Id);
            if (auth is not null)
            {
                auth.ChangeLogin(email);
                this._authRepository.Update(auth);
            }
        }

        if (!string.IsNullOrWhiteSpace(command.Name))
            user.Rename(command.Name, actor);

        this._userRepository.Update(user);
        await this._cache.RemoveAsync(UserCacheKeys.ById(user.Id)).ConfigureAwait(false);
        return new IdCommandResult(user.Id.ToString());
    }
}
