// *****************************************************************************************************************
// Project          : NavyBlue
// File             : EnsuresExtensions.String.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:50
// *****************************************************************************************************************
// <copyright file="EnsuresExtensions.String.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using Navyblue.BaseLibrary;

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    ///     Extensions of Ensures utility for the <see cref="System.String" />.
    /// </summary>
    public static partial class EnsuresExtensions
    {
        /// <summary>
        ///     Checks whether the given value contains the specified <paramref name="value" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> Contains(this Ensures<string> ensures, string value)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => (value == null && v == null) ||
                                     (value != null && v != null && v.Contains(value)));
        }

        /// <summary>
        ///     Checks whether the given value does not contain the specified <paramref name="value" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The value to compare.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> DoesNotContain(this Ensures<string> ensures, string value)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => (value == null && v == null) ||
                                    (value != null && v != null && v.Contains(value)));
        }

        /// <summary>
        ///     Checks whether the end of the given value does not match the specified <paramref name="value" />
        ///     using the specified comparison option.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonType">
        ///     One of the <see cref="StringComparison" /> values that determines how
        ///     this string and value are compared
        /// </param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> DoesNotEndWith(this Ensures<string> ensures, string value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => (value == null && v == null) ||
                                    (value != null && v != null && v.EndsWith(value, comparisonType)));
        }

        /// <summary>
        ///     Checks whether the given value is unequal in length to <paramref name="length" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="length">The invalid length.</param>
        /// <param name="additionalMessage">The additional message that should combine into the exception message.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> DoesNotHaveLength(this Ensures<string> ensures, int length, string additionalMessage = null)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            if (additionalMessage == null) throw new ArgumentNullException(nameof(additionalMessage));
            return ensures.Not(v => v != null && v.Length == length);
        }

        /// <summary>
        ///     Checks whether the given value does not start with the specified <paramref name="value" /> using the
        ///     specified comparison option.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonType">
        ///     One of the <see cref="StringComparison" /> values that determines how
        ///     this string and value are compared
        /// </param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> DoesNotStartWith(this Ensures<string> ensures, string value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => (value == null && v == null) ||
                                    (value != null && v != null && v.StartsWith(value, comparisonType)));
        }

        /// <summary>
        ///     Checks whether the end of the given value matches the specified <paramref name="value" /> using the
        ///     specified comparison option.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonType">
        ///     One of the <see cref="StringComparison" /> values that determines how
        ///     this string and value are compared
        /// </param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> EndsWith(this Ensures<string> ensures, string value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => (value == null && v == null) ||
                                     (value != null && v != null && v.EndsWith(value, comparisonType)));
        }

        /// <summary>
        ///     Checks whether the given value is equal in length to <paramref name="length" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="length">The valid length.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> HasLength(this Ensures<string> ensures, int length)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v != null && v.Length == length);
        }

        /// <summary>
        ///     Checks whether the given value is an <see cref="String.Empty" /> string. An exception is thrown
        ///     otherwise.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> IsEmpty(this Ensures<string> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.HasLength(0);
        }

        /// <summary>
        ///     Checks whether the given value is longer or equal in length than <paramref name="minLength" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minLength">The smallest valid length.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> IsLongerOrEqual(this Ensures<string> ensures, int minLength)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v != null && v.Length >= minLength);
        }

        /// <summary>
        ///     Checks whether the given value is longer in length than <paramref name="minLength" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="minLength">The biggest invalid length.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> IsLongerThan(this Ensures<string> ensures, int minLength)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v != null && v.Length > minLength);
        }

        /// <summary>
        ///     Checks whether the given value is not an <see cref="String.Empty" /> string. An exception is thrown
        ///     otherwise.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> IsNotEmpty(this Ensures<string> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v != null && v.Length == 0);
        }

        /// <summary>
        ///     Checks whether the given value is not null and not an <see cref="String.Empty" /> string.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> IsNotNullOrEmpty(this Ensures<string> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v.IsNotNullOrEmpty());
        }

        /// <summary>
        ///     Checks whether the given value is not <b>null</b> (Nothing in Visual Basic), not empty, and does
        ///     not consists only of white-space characters.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> IsNotNullOrWhiteSpace(this Ensures<string> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures;
        }

        /// <summary>
        ///     Checks whether the given value is null or an <see cref="String.Empty" /> string.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> IsNullOrEmpty(this Ensures<string> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v.IsNotNullOrEmpty());
        }

        /// <summary>
        ///     Checks whether the given value is <b>null</b> (Nothing in Visual Basic), empty, or consists only
        ///     of white-space characters.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> IsNullOrWhiteSpace(this Ensures<string> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v.IsNullOrWhiteSpace());
        }

        /// <summary>
        ///     Checks whether the given value is shorter or equal in length than <paramref name="maxLength" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxLength">The biggest valid length.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> IsShorterOrEqual(this Ensures<string> ensures, int maxLength)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v != null && v.Length <= maxLength);
        }

        /// <summary>
        ///     Checks whether the given value is shorter in length than <paramref name="maxLength" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="maxLength">The smallest invalid length.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> IsShorterThan(this Ensures<string> ensures, int maxLength)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v != null && v.Length < maxLength);
        }

        /// <summary>
        ///     Checks whether the given value starts with the specified <paramref name="value" /> using the
        ///     specified <paramref name="comparisonType" />.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="value">The value to compare.</param>
        /// <param name="comparisonType">
        ///     One of the <see cref="StringComparison" /> values that determines how
        ///     this string and value are compared
        /// </param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<string> StartsWith(this Ensures<string> ensures, string value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => (value == null && v == null) ||
                                     (value != null && v != null && v.StartsWith(value, comparisonType)));
        }
    }
}