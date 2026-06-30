// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : EnsuresExtensions.Compare.Int64.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="EnsuresExtensions.Compare.Int64.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace NavyBlue.AspNetCore.Lib;

/// <summary>
///     Extensions of Ensures utility for the <see cref="System.Int64" />.
/// </summary>
public static partial class EnsuresExtensions
{
    /// <summary>
    ///     Checks whether the given value is equal to the specified <paramref name="value" />.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <param name="value">The valid value to compare with.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<long> IsEqualTo(this Ensures<long> ensures, long value)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v == value);
    }

    /// <summary>
    ///     Checks whether the given value is greater or equal to the specified <paramref name="minValue" />.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <param name="minValue">The lowest valid value.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<long> IsGreaterOrEqual(this Ensures<long> ensures, long minValue)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v >= minValue);
    }

    /// <summary>
    ///     Checks whether the given value is greater than the specified <paramref name="minValue" />.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <param name="minValue">The highest invalid value.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<long> IsGreaterThan(this Ensures<long> ensures, long minValue)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v > minValue);
    }

    /// <summary>
    ///     Checks whether the given value is between <paramref name="minValue" /> and
    ///     <paramref name="maxValue" /> (including those values).
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <param name="minValue">The lowest valid value.</param>
    /// <param name="maxValue">The highest valid value.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<long> IsInRange(this Ensures<long> ensures, long minValue, long maxValue)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v >= minValue && v <= maxValue);
    }

    /// <summary>
    ///     Checks whether the given value is less than the specified <paramref name="maxValue" />.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <param name="maxValue">The lowest invalid value.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<long> IsLessThan(this Ensures<long> ensures, long maxValue)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v < maxValue);
    }

    /// <summary>
    ///     Checks whether the given value is smaller or equal to the specified <paramref name="maxValue" />.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <param name="maxValue">The highest valid value.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<long> IsLessThanOrEqual(this Ensures<long> ensures, long maxValue)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v <= maxValue);
    }

    /// <summary>
    ///     Checks whether the given value is unequal to the specified <paramref name="value" />.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <param name="value">The invalid value to compare with.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<long> IsNotEqualTo(this Ensures<long> ensures, long value)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.Not(v => v == value);
    }

    /// <summary>
    ///     Checks whether the given value is not greater or equal to the specified <paramref name="maxValue" />.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <param name="maxValue">The lowest invalid value.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<long> IsNotGreaterOrEqual(this Ensures<long> ensures, long maxValue)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v < maxValue);
    }

    /// <summary>
    ///     Checks whether the given value is not greater than the specified <paramref name="maxValue" />.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <param name="maxValue">The lowest valid value.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<long> IsNotGreaterThan(this Ensures<long> ensures, long maxValue)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v <= maxValue);
    }

    /// <summary>
    ///     Checks whether the given value is not between <paramref name="minValue" /> and
    ///     <paramref name="maxValue" /> (including those values).
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <param name="minValue">The lowest invalid value.</param>
    /// <param name="maxValue">The highest invalid value.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<long> IsNotInRange(this Ensures<long> ensures, long minValue, long maxValue)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v > maxValue || v < minValue);
    }

    /// <summary>
    ///     Checks whether the given value is not less than the specified <paramref name="minValue" />.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <param name="minValue">The lowest valid value.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<long> IsNotLessThan(this Ensures<long> ensures, long minValue)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v >= minValue);
    }

    /// <summary>
    ///     Checks whether the given value is not smaller or equal to the specified <paramref name="minValue" />.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <param name="minValue">The highest invalid value.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<long> IsNotLessThanOrEqual(this Ensures<long> ensures, long minValue)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v > minValue);
    }
}