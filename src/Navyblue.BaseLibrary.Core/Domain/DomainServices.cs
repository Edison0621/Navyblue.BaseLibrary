namespace Navyblue.BaseLibrary.Domain;

public interface IDomainService;

public abstract class DomainService : IDomainService
{
    protected static void Check(IBusinessRule rule) => CheckRule.Against(rule);
    protected static void Check(bool broken, string message, string code = "domain_rule_violation") => CheckRule.Against(broken, message, code);
}

public interface IDomainPolicy<in TContext>
{
    bool IsSatisfiedBy(TContext context);
    string FailureMessage { get; }
}

public static class DomainPolicyExtensions
{
    public static void EnsureSatisfiedBy<TContext>(this IDomainPolicy<TContext> policy, TContext context, string code = "domain_policy_violation")
    {
        ArgumentNullException.ThrowIfNull(policy);
        if (!policy.IsSatisfiedBy(context))
        {
            throw new DomainRuleViolationException(policy.FailureMessage, code);
        }
    }
}
