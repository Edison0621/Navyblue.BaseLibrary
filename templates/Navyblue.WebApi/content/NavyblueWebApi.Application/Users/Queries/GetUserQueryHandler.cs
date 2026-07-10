using NavyblueWebApi.Domain.Users;
using NavyblueWebApi.Model.Users;
using Navyblue.Foundation.Cqrs;

namespace NavyblueWebApi.Application.Users.Queries;

/// <summary>
///     Handles <see cref="GetUserQuery" /> by projecting the <see cref="User" /> aggregate into a <see cref="UserModel" />.
/// </summary>
public sealed class GetUserQueryHandler : QueryHandler<GetUserQuery, UserModel?>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }

    protected override async Task<UserModel?> ProcessRequest(GetUserQuery query)
    {
        User? user = await this._userRepository.FindAsync(query.UserId);
        return user is null ? null : ToModel(user);
    }

    public static UserModel ToModel(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Status = (int)user.Status,
        CreatedAt = user.CreatedAt,
        ModifiedAt = user.ModifiedAt
    };
}
