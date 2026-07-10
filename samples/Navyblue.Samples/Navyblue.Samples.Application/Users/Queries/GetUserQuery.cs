using Navyblue.Samples.Model.Users;
using Navyblue.Foundation.Cqrs;

namespace Navyblue.Samples.Application.Users.Queries;

/// <summary>
///     Returns a single user by id, or null when not found.
/// </summary>
public sealed class GetUserQuery : Query<UserModel?>
{
    public GetUserQuery(long userId)
    {
        this.UserId = userId;
    }

    public long UserId { get; }

    public override string DisplayName => "GetUser";

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
