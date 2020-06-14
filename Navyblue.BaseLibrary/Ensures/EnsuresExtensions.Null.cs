// *****************************************************************************************************************
// Project          : NavyBlue
// File             : EnsuresExtensions.Null.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:50
// *****************************************************************************************************************
// <copyright file="EnsuresExtensions.Null.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Diagnostics.CodeAnalysis;

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    ///     Extensions of Ensures utility for the null value.
    /// </summary>
    public static partial class EnsuresExtensions
    {
        /// <summary>
        ///     Checks whether the given value is not null.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsNotNull<T>(this Ensures<T> ensures) where T : class
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v != null);
        }

        /// <summary>
        ///     Checks whether the given value is not null.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNotNull<T>(this Ensures<T?> ensures) where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v.HasValue);
        }

        /// <summary>
        ///     Checks whether the given value is null.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsNull<T>(this Ensures<T> ensures) where T : class
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v == null);
        }

        /// <summary>
        ///     Checks whether the given value is null. An exception is thrown otherwise.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<T?> IsNull<T>(this Ensures<T?> ensures)
            where T : struct
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => !v.HasValue);
        }
    }
}