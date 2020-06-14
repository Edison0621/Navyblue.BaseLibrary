// *****************************************************************************************************************
// Project          : NavyBlue
// File             : EnsuresExtensions.Compare.Byte.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:49
// *****************************************************************************************************************
// <copyright file="EnsuresExtensions.Compare.Byte.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    ///     Extensions of Ensures utility for the <see cref="System.Byte" />.
    /// </summary>
    public static partial class EnsuresExtensions
    {
        /// <summary>
        ///     Checks whether the given value is equal to the specified <paramref name="value" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The valid value to compare with.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<byte> IsEqualTo(this Ensures<byte> ensures, byte value)
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
        public static Ensures<byte> IsGreaterOrEqual(this Ensures<byte> ensures, byte minValue)
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
        public static Ensures<byte> IsGreaterThan(this Ensures<byte> ensures, byte minValue)
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
        public static Ensures<byte> IsInRange(this Ensures<byte> ensures, byte minValue, byte maxValue)
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
        public static Ensures<byte> IsLessThan(this Ensures<byte> ensures, byte maxValue)
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
        public static Ensures<byte> IsLessThanOrEqual(this Ensures<byte> ensures, byte maxValue)
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
        public static Ensures<byte> IsNotEqualTo(this Ensures<byte> ensures, byte value)
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
        public static Ensures<byte> IsNotGreaterOrEqual(this Ensures<byte> ensures, byte maxValue)
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
        public static Ensures<byte> IsNotGreaterThan(this Ensures<byte> ensures, byte maxValue)
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
        public static Ensures<byte> IsNotInRange(this Ensures<byte> ensures, byte minValue, byte maxValue)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v < minValue || v > maxValue);
        }

        /// <summary>
        ///     Checks whether the given value is not less than the specified <paramref name="minValue" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<byte> IsNotLessThan(this Ensures<byte> ensures, byte minValue)
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
        public static Ensures<byte> IsNotLessThanOrEqual(this Ensures<byte> ensures, byte minValue)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v > minValue);
        }
    }
}