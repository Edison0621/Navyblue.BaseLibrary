using Navyblue.Foundation.Cqrs;

namespace Navyblue.Samples.Application.Users.Commands;

/// <summary>
///     Reactivates an inactive user.
/// </summary>
public sealed class ActivateUserCommand : Command<IdCommandResult>
{
    public ActivateUserCommand(long userId) => this.UserId = userId;

    public long UserId { get; }
    public override string DisplayName => "ActivateUser";
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
