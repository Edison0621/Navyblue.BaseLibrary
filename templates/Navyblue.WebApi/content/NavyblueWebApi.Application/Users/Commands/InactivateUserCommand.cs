using Navyblue.Foundation.Cqrs;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Marks a user inactive without deleting the record.
/// </summary>
public sealed class InactivateUserCommand : Command<IdCommandResult>
{
    public InactivateUserCommand(long userId)
    {
        this.UserId = userId;
    }

    public long UserId { get; }

    public override string DisplayName => "InactivateUser";

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
