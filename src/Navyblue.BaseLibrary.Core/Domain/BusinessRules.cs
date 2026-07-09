// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : BusinessRules.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="BusinessRules.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Domain;

/// <summary>
///     The business rule interface.
/// </summary>
public interface IBusinessRule
{
    /// <summary>
    ///     Gets the code.
    /// </summary>
    /// <value>
    ///     The code.
    /// </value>
    string Code { get; }

    /// <summary>
    ///     Gets the message.
    /// </summary>
    /// <value>
    ///     The message.
    /// </value>
    string Message { get; }

    /// <summary>
    ///     Checks if is broken.
    /// </summary>
    /// <returns>
    ///     A bool
    /// </returns>
    bool IsBroken();
}

/// <summary>
///     The business rule.
/// </summary>
/// <seealso cref="Navyblue.BaseLibrary.Domain.IBusinessRule" />
public abstract class BusinessRule : IBusinessRule
{
    #region IBusinessRule Members

    /// <summary>
    ///     Gets the code.
    /// </summary>
    /// <value>
    ///     The code.
    /// </value>
    public virtual string Code => this.GetType().Name;

    /// <summary>
    ///     Gets the message.
    /// </summary>
    /// <value>
    ///     The message.
    /// </value>
    public abstract string Message { get; }

    /// <summary>
    ///     Checks if is broken.
    /// </summary>
    /// <returns>
    ///     A bool
    /// </returns>
    public abstract bool IsBroken();

    #endregion
}

/// <summary>
///     The check rule.
/// </summary>
public static class CheckRule
{
    /// <summary>
    ///     Againsts the specified rule.
    /// </summary>
    /// <param name="rule">The rule.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DomainRuleViolationException"></exception>
    public static void Against(IBusinessRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        if (rule.IsBroken())
        {
            throw new DomainRuleViolationException(rule.Message, rule.Code);
        }
    }

    /// <summary>
    ///     Againsts the specified broken.
    /// </summary>
    /// <param name="broken">If true, broken.</param>
    /// <param name="message">The message.</param>
    /// <param name="code">The code.</param>
    /// <exception cref="DomainRuleViolationException"></exception>
    public static void Against(bool broken, string message, string code = "domain_rule_violation")
    {
        if (broken)
        {
            throw new DomainRuleViolationException(message, code);
        }
    }
}

/// <summary>
///     The delegate business rule.
/// </summary>
/// <seealso cref="Navyblue.BaseLibrary.Domain.IBusinessRule" />
/// <param name="message">The message.</param>
/// <param name="isBroken">The is broken.</param>
/// <param name="code">The code.</param>
public sealed class DelegateBusinessRule(string message, Func<bool> isBroken, string code = "domain_rule_violation") : IBusinessRule
{
    #region IBusinessRule Members

    /// <summary>
    ///     Gets the code.
    /// </summary>
    /// <value>
    ///     The code.
    /// </value>
    public string Code { get; } = code;

    /// <summary>
    ///     Gets the message.
    /// </summary>
    /// <value>
    ///     The message.
    /// </value>
    public string Message { get; } = message;

    /// <summary>
    ///     Checks if is broken.
    /// </summary>
    /// <returns>
    ///     A bool
    /// </returns>
    public bool IsBroken() => isBroken();

    #endregion
}