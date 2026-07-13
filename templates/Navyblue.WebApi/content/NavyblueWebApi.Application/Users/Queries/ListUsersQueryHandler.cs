using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Data;
using NavyblueWebApi.Domain.Users;
using NavyblueWebApi.Model.Users;

namespace NavyblueWebApi.Application.Users.Queries;

/// <summary>
///     Handles <see cref="ListUsersQuery" />.
/// </summary>
public sealed class ListUsersQueryHandler : QueryHandler<ListUsersQuery, PageData<UserModel>>
{
    private readonly IUserRepository _userRepository;

    public ListUsersQueryHandler(IUserRepository userRepository) => this._userRepository = userRepository;

    protected override async Task<PageData<UserModel>> ProcessRequest(ListUsersQuery query)
    {
        PageData<User> page = await this._userRepository
            .PageAsync(new PageQuery(query.PageIndex, query.PageSize), query.Keyword)
            .ConfigureAwait(false);

        IReadOnlyList<UserModel> items = page.Items.Select(GetUserQueryHandler.ToModel).ToList();
        return new PageData<UserModel>(items, page.Total, page.PageIndex, page.PageSize);
    }
}
