using Navyblue.Foundation.Cqrs;

namespace Navyblue.Samples.Application.Users.Commands;

/// <summary>
///     Deletes a user permanently.
/// </summary>
public sealed class DeleteUserCommand : Command<IdCommandResult>
{
    public DeleteUserCommand(long userId)
    {
        this.UserId = userId;
    }

    public long UserId { get; }

    public override string DisplayName => "DeleteUser";

    public override string Id { get; } = Guid.NewGuid().ToString("N");

    public override bool Validate(out string validationErrorMessage)
    {
        if (this.UserId <= 0)
        {
            validationErrorMessage = "A positive user Id is required.";
            return false;
        }

        validationErrorMessage = string.Empty;
        return true;
    }
}
