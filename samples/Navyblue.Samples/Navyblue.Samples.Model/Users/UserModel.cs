namespace Navyblue.Samples.Model.Users;

/// <summary>
///     Read model returned by user queries and write endpoints.
/// </summary>
public sealed class UserModel
{
    public long Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public int Status { get; init; }

    public string StatusName => Status switch
    {
        1 => "Active",
        2 => "Inactive",
        _ => "Unknown"
    };

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? ModifiedAt { get; init; }
}
