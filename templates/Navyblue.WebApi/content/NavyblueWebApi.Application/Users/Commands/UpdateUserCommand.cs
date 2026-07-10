using Navyblue.Foundation.Cqrs;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Updates an existing user's name and/or email.
/// </summary>
public sealed class UpdateUserCommand : Command<IdCommandResult>
{
    public UpdateUserCommand(long userId, string? name, string? email)
    {
        this.UserId = userId;
        this.Name = name;
        this.Email = email;
    }

    /// <summary>Identifier of the user to update.</summary>
    public long UserId { get; }

    public string? Name { get; }

    public string? Email { get; }

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
