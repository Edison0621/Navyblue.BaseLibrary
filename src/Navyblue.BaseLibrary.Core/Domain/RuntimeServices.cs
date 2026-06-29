namespace Navyblue.BaseLibrary.Domain;

public interface IClock
{
    DateTimeOffset Now { get; }
    DateTimeOffset UtcNow { get; }
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

public interface IAuditor
{
    string? CurrentUserId { get; }
    string? CurrentUserName { get; }
}

public interface ITenantResolver
{
    string? CurrentTenantId { get; }
}

public interface IAuditPropertySetter
{
    void SetCreationAudit(object entity);
    void SetModificationAudit(object entity);
    void SetDeletionAudit(object entity);
}
