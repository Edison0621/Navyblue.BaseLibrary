using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Data;
using NavyblueWebApi.Model.Users;

namespace NavyblueWebApi.Application.Users.Queries;

/// <summary>
///     Paged user list query.
/// </summary>
public sealed class ListUsersQuery : Query<PageData<UserModel>>
{
    public ListUsersQuery(string? keyword = null, int pageIndex = 1, int pageSize = 20)
    {
        this.Keyword = keyword;
        this.PageIndex = pageIndex;
        this.PageSize = pageSize;
    }

    public string? Keyword { get; }

    public int PageIndex { get; }

    public int PageSize { get; }

    public override string DisplayName => "ListUsers";

    public override string Id { get; } = Guid.NewGuid().ToString("N");

    public override bool Validate(out string validationErrorMessage)
    {
        if (this.PageIndex < 1)
        {
            validationErrorMessage = "pageIndex must be >= 1.";
            return false;
        }

        if (this.PageSize is < 1 or > 200)
        {
            validationErrorMessage = "pageSize must be between 1 and 200.";
            return false;
        }

        validationErrorMessage = string.Empty;
        return true;
    }
}
