// *****************************************************************************************************************
// Project          : NavyBlue
// File             : EnsuresExtensions.Compare.Single.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:50
// *****************************************************************************************************************
// <copyright file="EnsuresExtensions.Compare.Single.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    ///     Extensions of Ensures utility for the <see cref="System.Single" />.
    /// </summary>
    public static partial class EnsuresExtensions
    {
        /// <summary>
        ///     Checks whether the given value is equal to the specified <paramref name="value" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The valid value to compare with.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsEqualTo(this Ensures<float> ensures, float value)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return ensures.That(v => v == value);
        }

        /// <summary>
        ///     Checks whether the given value is greater or equal to the specified <paramref name="minValue" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsGreaterOrEqual(this Ensures<float> ensures, float minValue)
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
        public static Ensures<float> IsGreaterThan(this Ensures<float> ensures, float minValue)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v > minValue);
        }

        /// <summary>
        ///     Checks whether the given value is infinity.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsInfinity(this Ensures<float> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => float.IsInfinity(v));
        }

        /// <summary>
        ///     Checks whether the given value is between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> (including those values).
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest valid value.</param>
        /// <param name="maxValue">The highest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsInRange(this Ensures<float> ensures, float minValue, float maxValue)
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
        public static Ensures<float> IsLessThan(this Ensures<float> ensures, float maxValue)
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
        public static Ensures<float> IsLessThanOrEqual(this Ensures<float> ensures, float maxValue)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v <= maxValue);
        }

        /// <summary>
        ///     Checks whether the given value is a valid number.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsNaN(this Ensures<float> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => float.IsNaN(v));
        }

        /// <summary>
        ///     Checks whether the given value is negative infinity.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsNegativeInfinity(this Ensures<float> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => float.IsNegativeInfinity(v));
        }

        /// <summary>
        ///     Checks whether the given value is unequal to the specified <paramref name="value" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The invalid value to compare with.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsNotEqualTo(this Ensures<float> ensures, float value)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return ensures.Not(v => v == value);
        }

        /// <summary>
        ///     Checks whether the given value is not greater or equal to the specified <paramref name="maxValue" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The lowest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsNotGreaterOrEqual(this Ensures<float> ensures, float maxValue)
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
        public static Ensures<float> IsNotGreaterThan(this Ensures<float> ensures, float maxValue)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v <= maxValue);
        }

        /// <summary>
        ///     Checks whether the given value is not infinity.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsNotInfinity(this Ensures<float> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => float.IsInfinity(v));
        }

        /// <summary>
        ///     Checks whether the given value is not between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> (including those values).
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest invalid value.</param>
        /// <param name="maxValue">The highest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsNotInRange(this Ensures<float> ensures, float minValue, float maxValue)
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
        public static Ensures<float> IsNotLessThan(this Ensures<float> ensures, float minValue)
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
        public static Ensures<float> IsNotLessThanOrEqual(this Ensures<float> ensures, float minValue)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v > minValue);
        }

        /// <summary>
        ///     Checks whether the given value is a not valid number.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsNotNaN(this Ensures<float> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => float.IsNaN(v));
        }

        /// <summary>
        ///     Checks whether the given value is not negative infinity.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsNotNegativeInfinity(this Ensures<float> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => float.IsNegativeInfinity(v));
        }

        /// <summary>
        ///     Checks whether the given value is not positive infinity.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsNotPositiveInfinity(this Ensures<float> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => float.IsPositiveInfinity(v));
        }

        /// <summary>
        ///     Checks whether the given value is positive infinity.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<float> IsPositiveInfinity(this Ensures<float> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => float.IsPositiveInfinity(v));
        }
    }
}