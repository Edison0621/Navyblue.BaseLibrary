namespace Navyblue.BaseLibrary.Domain;

public interface IBusinessRule
{
    string Code { get; }
    string Message { get; }
    bool IsBroken();
}

public abstract class BusinessRule : IBusinessRule
{
    public virtual string Code => GetType().Name;
    public abstract string Message { get; }
    public abstract bool IsBroken();
}

public static class CheckRule
{
    public static void Against(IBusinessRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        if (rule.IsBroken())
        {
            throw new DomainRuleViolationException(rule.Message, rule.Code);
        }
    }

    public static void Against(bool broken, string message, string code = "domain_rule_violation")
    {
        if (broken)
        {
            throw new DomainRuleViolationException(message, code);
        }
    }
}

public sealed class DelegateBusinessRule(string message, Func<bool> isBroken, string code = "domain_rule_violation") : IBusinessRule
{
    public string Code { get; } = code;
    public string Message { get; } = message;
    public bool IsBroken() => isBroken();
}
