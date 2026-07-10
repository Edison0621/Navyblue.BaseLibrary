using Navyblue.Samples.Model.Users;
using Navyblue.Foundation.Cqrs;

namespace Navyblue.Samples.Application.Users.Queries;

/// <summary>
///     Returns the full list of users. Pagination is intentionally omitted to keep the sample simple.
/// </summary>
public sealed class ListUsersQuery : Query<List<UserModel>>
{
    public ListUsersQuery(string? keyword = null)
    {
        this.Keyword = keyword;
    }

    public string? Keyword { get; }

    public override string DisplayName => "ListUsers";

    public override string Id { get; } = Guid.NewGuid().ToString("N");

    public override bool Validate(out string validationErrorMessage)
    {
        validationErrorMessage = string.Empty;
        return true;
    }
}
