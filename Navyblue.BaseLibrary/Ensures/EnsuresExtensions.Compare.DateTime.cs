// *****************************************************************************************************************
// Project          : NavyBlue
// File             : EnsuresExtensions.Compare.DateTime.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:49
// *****************************************************************************************************************
// <copyright file="EnsuresExtensions.Compare.DateTime.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    ///     Extensions of Ensures utility for the <see cref="System.DateTime" />.
    /// </summary>
    public static partial class EnsuresExtensions
    {
        /// <summary>
        ///     Checks whether the given value is equal to the specified <paramref name="value" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The valid value to compare with.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<DateTime> IsEqualTo(this Ensures<DateTime> ensures, DateTime value)
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
        public static Ensures<DateTime> IsGreaterOrEqual(this Ensures<DateTime> ensures, DateTime minValue)
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
        public static Ensures<DateTime> IsGreaterThan(this Ensures<DateTime> ensures, DateTime minValue)
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
        public static Ensures<DateTime> IsInRange(this Ensures<DateTime> ensures, DateTime minValue, DateTime maxValue)
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
        public static Ensures<DateTime> IsLessThan(this Ensures<DateTime> ensures, DateTime maxValue)
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
        public static Ensures<DateTime> IsLessThanOrEqual(this Ensures<DateTime> ensures, DateTime maxValue)
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
        public static Ensures<DateTime> IsNotEqualTo(this Ensures<DateTime> ensures, DateTime value)
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
        public static Ensures<DateTime> IsNotGreaterOrEqual(this Ensures<DateTime> ensures, DateTime maxValue)
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
        public static Ensures<DateTime> IsNotGreaterThan(this Ensures<DateTime> ensures, DateTime maxValue)
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
        public static Ensures<DateTime> IsNotInRange(this Ensures<DateTime> ensures, DateTime minValue, DateTime maxValue)
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
        public static Ensures<DateTime> IsNotLessThan(this Ensures<DateTime> ensures, DateTime minValue)
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
        public static Ensures<DateTime> IsNotLessThanOrEqual(this Ensures<DateTime> ensures, DateTime minValue)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v > minValue);
        }
    }
}