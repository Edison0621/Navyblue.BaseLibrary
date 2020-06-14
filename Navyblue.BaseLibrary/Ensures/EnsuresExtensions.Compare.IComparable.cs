// *****************************************************************************************************************
// Project          : NavyBlue
// File             : EnsuresExtensions.Compare.IComparable.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:49
// *****************************************************************************************************************
// <copyright file="EnsuresExtensions.Compare.IComparable.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    ///     Extensions of Ensures utility for the <see cref="System.IComparable" />.
    /// </summary>
    public static partial class EnsuresExtensions
    {
        /// <summary>
        ///     Checks whether the given value is equal to the specified <paramref name="value" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The valid value to compare with.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsEqualTo<T>(this Ensures<T> ensures, T value) where T : IComparable
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T>.Default.Compare(v, value) == 0);
        }

        /// <summary>
        ///     Checks whether the given value is equal to the specified <paramref name="value" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The valid value to compare with.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsEqualTo<T>(this Ensures<T?> ensures, T? value) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T?>.Default.Compare(v, value) == 0);
        }

        /// <summary>
        ///     Checks whether the given value is equal to the specified <paramref name="value" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The valid value to compare with.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsEqualTo<T>(this Ensures<T?> ensures, T value) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T?>.Default.Compare(v, value) == 0);
        }

        /// <summary>
        ///     Checks whether the given value is greater or equal to the specified <paramref name="minValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsGreaterOrEqual<T>(this Ensures<T> ensures, T minValue) where T : IComparable
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T>.Default.Compare(v, minValue) >= 0);
        }

        /// <summary>
        ///     Checks whether the given value is greater or equal to the specified <paramref name="minValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsGreaterOrEqual<T>(this Ensures<T?> ensures, T? minValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T?>.Default.Compare(v, minValue) >= 0);
        }

        /// <summary>
        ///     Checks whether the given value is greater or equal to the specified <paramref name="minValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsGreaterOrEqual<T>(this Ensures<T?> ensures, T minValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T?>.Default.Compare(v, minValue) >= 0);
        }

        /// <summary>
        ///     Checks whether the given value is greater than the specified <paramref name="minValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The highest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsGreaterThan<T>(this Ensures<T> ensures, T minValue) where T : IComparable
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T>.Default.Compare(v, minValue) > 0);
        }

        /// <summary>
        ///     Checks whether the given value is greater than the specified <paramref name="minValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The highest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsGreaterThan<T>(this Ensures<T?> ensures, T? minValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T?>.Default.Compare(v, minValue) > 0);
        }

        /// <summary>
        ///     Checks whether the given value is greater than the specified <paramref name="minValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The highest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsGreaterThan<T>(this Ensures<T?> ensures, T minValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T?>.Default.Compare(v, minValue) > 0);
        }

        /// <summary>
        ///     Checks whether the given value is between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> (including those values).
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest valid value.</param>
        /// <param name="maxValue">The highest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsInRange<T>(this Ensures<T> ensures, T minValue, T maxValue) where T : IComparable
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            Comparer<T> defaultComparer = Comparer<T>.Default;
            return ensures.That(v => defaultComparer.Compare(v, minValue) >= 0 &&
                                     defaultComparer.Compare(v, maxValue) <= 0);
        }

        /// <summary>
        ///     Checks whether the given value is between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> (including those values).
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest valid value.</param>
        /// <param name="maxValue">The highest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsInRange<T>(this Ensures<T?> ensures, T? minValue, T? maxValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            Comparer<T?> defaultComparer = Comparer<T?>.Default;
            return ensures.That(v => defaultComparer.Compare(v, minValue) >= 0 &&
                                     defaultComparer.Compare(v, maxValue) <= 0);
        }

        /// <summary>
        ///     Checks whether the given value is between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> (including those values).
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest valid value.</param>
        /// <param name="maxValue">The highest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsInRange<T>(this Ensures<T?> ensures, T minValue, T maxValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            Comparer<T?> defaultComparer = Comparer<T?>.Default;
            return ensures.That(v => defaultComparer.Compare(v, minValue) >= 0 &&
                                     defaultComparer.Compare(v, maxValue) <= 0);
        }

        /// <summary>
        ///     Checks whether the given value is less than the specified <paramref name="maxValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The lowest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsLessThan<T>(this Ensures<T> ensures, T maxValue) where T : IComparable
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T>.Default.Compare(v, maxValue) < 0);
        }

        /// <summary>
        ///     Checks whether the given value is less than the specified <paramref name="maxValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The lowest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsLessThan<T>(this Ensures<T?> ensures, T? maxValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T?>.Default.Compare(v, maxValue) < 0);
        }

        /// <summary>
        ///     Checks whether the given value is less than the specified <paramref name="maxValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The lowest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsLessThan<T>(this Ensures<T?> ensures, T maxValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T?>.Default.Compare(v, maxValue) < 0);
        }

        /// <summary>
        ///     Checks whether the given value is smaller or equal to the specified <paramref name="maxValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The highest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsLessThanOrEqual<T>(this Ensures<T> ensures, T maxValue) where T : IComparable
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T>.Default.Compare(v, maxValue) <= 0);
        }

        /// <summary>
        ///     Checks whether the given value is smaller or equal to the specified <paramref name="maxValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The highest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsLessThanOrEqual<T>(this Ensures<T?> ensures, T? maxValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T?>.Default.Compare(v, maxValue) <= 0);
        }

        /// <summary>
        ///     Checks whether the given value is smaller or equal to the specified <paramref name="maxValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The highest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsLessThanOrEqual<T>(this Ensures<T?> ensures, T maxValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => Comparer<T?>.Default.Compare(v, maxValue) <= 0);
        }

        /// <summary>
        ///     Checks whether the given value is unequal to the specified <paramref name="value" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The invalid value to compare with.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsNotEqualTo<T>(this Ensures<T> ensures, T value) where T : IComparable
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T>.Default.Compare(v, value) == 0);
        }

        /// <summary>
        ///     Checks whether the given value is unequal to the specified <paramref name="value" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The invalid value to compare with.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotEqualTo<T>(this Ensures<T?> ensures, T? value) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T?>.Default.Compare(v, value) == 0);
        }

        /// <summary>
        ///     Checks whether the given value is unequal to the specified <paramref name="value" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The invalid value to compare with.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotEqualTo<T>(this Ensures<T?> ensures, T value) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T?>.Default.Compare(v, value) == 0);
        }

        /// <summary>
        ///     Checks whether the given value is not greater or equal to the specified <paramref name="maxValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The lowest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsNotGreaterOrEqual<T>(this Ensures<T> ensures, T maxValue) where T : IComparable
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T>.Default.Compare(v, maxValue) >= 0);
        }

        /// <summary>
        ///     Checks whether the given value is not greater or equal to the specified <paramref name="maxValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The lowest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotGreaterOrEqual<T>(this Ensures<T?> ensures, T? maxValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T?>.Default.Compare(v, maxValue) >= 0);
        }

        /// <summary>
        ///     Checks whether the given value is not greater or equal to the specified <paramref name="maxValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The lowest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotGreaterOrEqual<T>(this Ensures<T?> ensures, T maxValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T?>.Default.Compare(v, maxValue) >= 0);
        }

        /// <summary>
        ///     Checks whether the given value is not greater than the specified <paramref name="maxValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The lowest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsNotGreaterThan<T>(this Ensures<T> ensures, T maxValue) where T : IComparable
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T>.Default.Compare(v, maxValue) > 0);
        }

        /// <summary>
        ///     Checks whether the given value is not greater than the specified <paramref name="maxValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The lowest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotGreaterThan<T>(this Ensures<T?> ensures, T? maxValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T?>.Default.Compare(v, maxValue) > 0);
        }

        /// <summary>
        ///     Checks whether the given value is not greater than the specified <paramref name="maxValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxValue">The lowest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotGreaterThan<T>(this Ensures<T?> ensures, T maxValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T?>.Default.Compare(v, maxValue) > 0);
        }

        /// <summary>
        ///     Checks whether the given value is not between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> (including those values).
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest invalid value.</param>
        /// <param name="maxValue">The highest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsNotInRange<T>(this Ensures<T> ensures, T minValue, T maxValue) where T : IComparable
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            Comparer<T> defaultComparer = Comparer<T>.Default;
            return ensures.Not(v => defaultComparer.Compare(v, minValue) >= 0 &&
                                    defaultComparer.Compare(v, maxValue) <= 0);
        }

        /// <summary>
        ///     Checks whether the given value is not between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> (including those values).
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest invalid value.</param>
        /// <param name="maxValue">The highest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotInRange<T>(this Ensures<T?> ensures, T? minValue, T? maxValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            Comparer<T?> defaultComparer = Comparer<T?>.Default;
            return ensures.Not(v => defaultComparer.Compare(v, minValue) >= 0 &&
                                    defaultComparer.Compare(v, maxValue) <= 0);
        }

        /// <summary>
        ///     Checks whether the given value is not between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> (including those values).
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest invalid value.</param>
        /// <param name="maxValue">The highest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotInRange<T>(this Ensures<T?> ensures, T minValue, T maxValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            Comparer<T?> defaultComparer = Comparer<T?>.Default;
            return ensures.Not(v => defaultComparer.Compare(v, minValue) >= 0 &&
                                    defaultComparer.Compare(v, maxValue) <= 0);
        }

        /// <summary>
        ///     Checks whether the given value is not less than the specified <paramref name="minValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsNotLessThan<T>(this Ensures<T> ensures, T minValue) where T : IComparable
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T>.Default.Compare(v, minValue) < 0);
        }

        /// <summary>
        ///     Checks whether the given value is not less than the specified <paramref name="minValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotLessThan<T>(this Ensures<T?> ensures, T? minValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T?>.Default.Compare(v, minValue) < 0);
        }

        /// <summary>
        ///     Checks whether the given value is not less than the specified <paramref name="minValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The lowest valid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotLessThan<T>(this Ensures<T?> ensures, T minValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T?>.Default.Compare(v, minValue) < 0);
        }

        /// <summary>
        ///     Checks whether the given value is not smaller or equal to the specified <paramref name="minValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The highest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsNotLessThanOrEqual<T>(this Ensures<T> ensures, T minValue) where T : IComparable
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T>.Default.Compare(v, minValue) <= 0);
        }

        /// <summary>
        ///     Checks whether the given value is not smaller or equal to the specified <paramref name="minValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The highest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotLessThanOrEqual<T>(this Ensures<T?> ensures, T? minValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T?>.Default.Compare(v, minValue) <= 0);
        }

        /// <summary>
        ///     Checks whether the given value is not smaller or equal to the specified <paramref name="minValue" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minValue">The highest invalid value.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotLessThanOrEqual<T>(this Ensures<T?> ensures, T minValue) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => Comparer<T?>.Default.Compare(v, minValue) <= 0);
        }
    }
}