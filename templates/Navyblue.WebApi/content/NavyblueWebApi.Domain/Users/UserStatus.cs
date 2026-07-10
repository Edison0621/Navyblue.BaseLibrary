namespace NavyblueWebApi.Domain.Users;

/// <summary>
///     User account status lifecycle.
/// </summary>
public enum UserStatus
{
    /// <summary>Account is active and can authenticate.</summary>
    Active = 1,

    /// <summary>Account is disabled and cannot authenticate.</summary>
    Inactive = 2
}
