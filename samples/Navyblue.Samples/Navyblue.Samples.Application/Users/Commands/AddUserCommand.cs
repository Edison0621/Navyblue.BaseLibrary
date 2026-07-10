using Navyblue.Foundation.Cqrs;

namespace Navyblue.Samples.Application.Users.Commands;

/// <summary>
///     Creates a new user. Returns the generated user id.
/// </summary>
public sealed class AddUserCommand : Command<IdCommandResult>
{
    public AddUserCommand(string name, string email, string? password = null)
    {
        this.Name = name;
        this.Email = email;
        this.Password = password;
    }

    public string Name { get; }

    public string Email { get; }

    /// <summary>Optional initial password. When supplied, an Auth credential is created alongside the user.</summary>
    public string? Password { get; }

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
