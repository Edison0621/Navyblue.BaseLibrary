using Navyblue.Foundation.Cqrs;

namespace NavyblueWebApi.Application.Authentication.Commands;

/// <summary>
///     Authenticates a user by login (email) and password.
/// </summary>
public sealed class AuthCommand : Command<AuthCommandResult>
{
    public AuthCommand(string login, string password)
    {
        this.Login = login;
        this.Password = password;
    }

    public string Login { get; }

    public string Password { get; }

    public override string DisplayName => "Auth";

    public override string Id { get; } = Guid.NewGuid().ToString("N");

    public override bool Validate(out string validationErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(this.Login))
        {
            validationErrorMessage = "Login is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(this.Password))
        {
            validationErrorMessage = "Password is required.";
            return false;
        }

        validationErrorMessage = string.Empty;
        return true;
    }
}
