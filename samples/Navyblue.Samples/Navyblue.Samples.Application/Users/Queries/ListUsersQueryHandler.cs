using Navyblue.Samples.Domain.Users;
using Navyblue.Samples.Model.Users;
using Navyblue.Foundation.Cqrs;

namespace Navyblue.Samples.Application.Users.Queries;

/// <summary>
///     Handles <see cref="ListUsersQuery" />.
/// </summary>
public sealed class ListUsersQueryHandler : QueryHandler<ListUsersQuery, List<UserModel>>
{
    private readonly IUserRepository _userRepository;

    public ListUsersQueryHandler(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }

    protected override async Task<List<UserModel>> ProcessRequest(ListUsersQuery query)
    {
        IReadOnlyList<User> users = await this._userRepository.ListAsync();

        IEnumerable<User> filtered = users;
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            string keyword = query.Keyword.Trim();
            filtered = users.Where(u =>
                u.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || u.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        return filtered
            .OrderBy(u => u.Id)
            .Select(GetUserQueryHandler.ToModel)
            .ToList();
    }
}
