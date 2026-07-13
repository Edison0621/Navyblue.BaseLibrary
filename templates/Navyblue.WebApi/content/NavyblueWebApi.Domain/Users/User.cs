using Navyblue.Foundation.Domain;

namespace NavyblueWebApi.Domain.Users;

/// <summary>
///     User aggregate root with audit + soft-delete fields from <see cref="FullAuditedAggregateRoot{TKey}" />.
/// </summary>
public sealed class User : FullAuditedAggregateRoot<long>
{
    /// <summary>EF Core materialization constructor.</summary>
    private User() : base(default)
    {
        this.Name = null!;
        this.Email = null!;
    }

    public User(long id, string name, string email, string? createdBy = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("User email is required.", nameof(email));

        this.Name = name;
        this.Email = email;
        this.Status = UserStatus.Active;
        this.CreatedAt = DateTimeOffset.UtcNow;
        this.CreatedBy = createdBy;
    }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public UserStatus Status { get; private set; }

    public void Rename(string name, string? modifiedBy = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainRuleViolationException("User name cannot be empty.", "user_name_empty");
        this.EnsureNotDeleted();
        this.Name = name;
        this.Touch(modifiedBy);
    }

    public void ChangeEmail(string email, string? modifiedBy = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainRuleViolationException("User email cannot be empty.", "user_email_empty");
        this.EnsureNotDeleted();
        this.Email = email;
        this.Touch(modifiedBy);
    }

    public void Inactivate(string? modifiedBy = null)
    {
        this.EnsureNotDeleted();
        if (this.Status == UserStatus.Inactive) return;
        this.Status = UserStatus.Inactive;
        this.Touch(modifiedBy);
    }

    public void Activate(string? modifiedBy = null)
    {
        this.EnsureNotDeleted();
        if (this.Status == UserStatus.Active) return;
        this.Status = UserStatus.Active;
        this.Touch(modifiedBy);
    }

    /// <summary>Soft-delete the user (does not hard-remove the row).</summary>
    public void SoftDelete(string? deletedBy = null)
    {
        this.EnsureNotDeleted();
        this.MarkAsDeleted(deletedBy);
        this.Status = UserStatus.Inactive;
        // Free unique email for reuse after soft-delete.
        this.Email = $"{this.Email}#deleted#{this.Id}";
        this.Touch(deletedBy);
    }

    private void Touch(string? modifiedBy)
    {
        this.ModifiedAt = DateTimeOffset.UtcNow;
        if (!string.IsNullOrWhiteSpace(modifiedBy))
            this.ModifiedBy = modifiedBy;
    }
}
