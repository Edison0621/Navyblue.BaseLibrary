// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Validation.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="Validation.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Validation;

/// <summary>
///     The validation rules.
/// </summary>
public static class ValidationRules
{
    /// <summary>
    ///     Checks if is email.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A bool</returns>
    public static bool IsEmail(string? value) => !string.IsNullOrWhiteSpace(value) && value.Contains('@') && value.Contains('.');

    /// <summary>
    ///     Checks if is mobile china.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A bool</returns>
    public static bool IsMobileChina(string? value) => !string.IsNullOrWhiteSpace(value) && value.Length == 11 && value.All(char.IsDigit) && value[0] == '1';
}

/// <summary>
///     The required guid attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class RequiredGuidAttribute : Attribute;