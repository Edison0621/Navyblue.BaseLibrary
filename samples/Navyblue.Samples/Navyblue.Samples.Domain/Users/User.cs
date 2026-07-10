using Navyblue.Foundation.Domain;

namespace Navyblue.Samples.Domain.Users;

/// <summary>
///     User aggregate root. Owns the user profile and its lifecycle.
///     Credentials live in the separate <see cref="Auth" /> entity keyed by user id.
/// </summary>
public sealed class User : AggregateRoot<long>
{
    public User(long id, string name, string email) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("User email is required.", nameof(email));

        this.Name = name;
        this.Email = email;
        this.Status = UserStatus.Active;
        this.CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Display name of the user.</summary>
    public string Name { get; private set; }

    /// <summary>Unique email address, used as the login identity.</summary>
    public string Email { get; private set; }

    /// <summary>Current lifecycle status.</summary>
    public UserStatus Status { get; private set; }

    /// <summary>When the user was created.</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Last modification time.</summary>
    public DateTimeOffset? ModifiedAt { get; private set; }

    /// <summary>Rename the user. Empty names are rejected.</summary>
    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainRuleViolationException("User name cannot be empty.", "user_name_empty");
        this.Name = name;
        this.ModifiedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Change the email address.</summary>
    public void ChangeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainRuleViolationException("User email cannot be empty.", "user_email_empty");
        this.Email = email;
        this.ModifiedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Mark the account inactive. Idempotent.</summary>
    public void Inactivate()
    {
        if (this.Status == UserStatus.Inactive) return;
        this.Status = UserStatus.Inactive;
        this.ModifiedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Reactivate an inactive account.</summary>
    public void Activate()
    {
        if (this.Status == UserStatus.Active) return;
        this.Status = UserStatus.Active;
        this.ModifiedAt = DateTimeOffset.UtcNow;
    }
}
