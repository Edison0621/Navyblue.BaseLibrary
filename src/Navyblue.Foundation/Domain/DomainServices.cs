// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : DomainServices.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="DomainServices.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Domain;

/// <summary>
///     The domain service interface.
/// </summary>
public interface IDomainService;

/// <summary>
///     The domain service.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Domain.IDomainService" />
public abstract class DomainService : IDomainService
{
    /// <summary>
    ///     Checks the specified rule.
    /// </summary>
    /// <param name="rule">The rule.</param>
    protected static void Check(IBusinessRule rule) => CheckRule.Against(rule);

    /// <summary>
    ///     Checks the specified broken.
    /// </summary>
    /// <param name="broken">if set to <c>true</c> [broken].</param>
    /// <param name="message">The message.</param>
    /// <param name="code">The code.</param>
    protected static void Check(bool broken, string message, string code = "domain_rule_violation") => CheckRule.Against(broken, message, code);
}

/// <summary>
///     The domain policy interface.
/// </summary>
/// <typeparam name="TContext">The type of the context.</typeparam>
public interface IDomainPolicy<in TContext>
{
    /// <summary>
    ///     Gets the failure message.
    /// </summary>
    /// <value>
    ///     The failure message.
    /// </value>
    string FailureMessage { get; }

    /// <summary>
    ///     Checks if is satisfied by.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>
    ///     A bool
    /// </returns>
    bool IsSatisfiedBy(TContext context);
}

/// <summary>
///     The domain policy extensions.
/// </summary>
public static class DomainPolicyExtensions
{
    /// <summary>
    ///     Checks if is satisfied by.
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <param name="policy">The policy.</param>
    /// <param name="context">The context.</param>
    /// <param name="code">The code.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DomainRuleViolationException"></exception>
    public static void EnsureSatisfiedBy<TContext>(this IDomainPolicy<TContext> policy, TContext context, string code = "domain_policy_violation")
    {
        ArgumentNullException.ThrowIfNull(policy);
        if (!policy.IsSatisfiedBy(context))
        {
            throw new DomainRuleViolationException(policy.FailureMessage, code);
        }
    }
}