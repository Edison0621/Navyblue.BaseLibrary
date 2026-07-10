using NavyblueWebApi.Application.Users;
using NavyblueWebApi.Domain.Authentication;
using NavyblueWebApi.Domain.Users;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Security;

namespace NavyblueWebApi.Application.Authentication.Commands;

/// <summary>
///     Handles <see cref="AuthCommand" />: verifies the stored password hash, ensures the
///     owning user is active, and issues a JWT through <see cref="ITokenIssuer" />.
/// </summary>
public sealed class AuthCommandHandler : CommandHandler<AuthCommand, AuthCommandResult>
{
    private readonly IAuthRepository _authRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenIssuer _tokenIssuer;

    public AuthCommandHandler(IAuthRepository authRepository, IUserRepository userRepository, ITokenIssuer tokenIssuer)
    {
        this._authRepository = authRepository;
        this._userRepository = userRepository;
        this._tokenIssuer = tokenIssuer;
    }

    protected override async Task<AuthCommandResult> ProcessRequest(AuthCommand command)
    {
        Auth? auth = await this._authRepository.FindByLoginAsync(command.Login)
            ?? throw new UnauthorizedException("Invalid login or password.");

        if (!PasswordHasher.Verify(command.Password, auth.PasswordHash))
            throw new UnauthorizedException("Invalid login or password.");

        User? user = await this._userRepository.FindAsync(auth.UserId)
            ?? throw new UnauthorizedException("Invalid login or password.");

        if (user.Status != UserStatus.Active)
            throw new ForbiddenException("User account is inactive.");

        string token = this._tokenIssuer.Issue(user.Id, user.Name, new Dictionary<string, string>
        {
            ["email"] = user.Email
        });

        return new AuthCommandResult(user.Id, user.Name, token);
    }
}
