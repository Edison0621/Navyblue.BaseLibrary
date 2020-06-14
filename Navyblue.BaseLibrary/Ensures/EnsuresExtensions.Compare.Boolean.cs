// *****************************************************************************************************************
// Project          : NavyBlue
// File             : EnsuresExtensions.Compare.Boolean.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:49
// *****************************************************************************************************************
// <copyright file="EnsuresExtensions.Compare.Boolean.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Diagnostics.CodeAnalysis;

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    ///     Extensions of Ensures utility for the <see cref="System.Boolean" />.
    /// </summary>
    public static partial class EnsuresExtensions
    {
        /// <summary>
        ///     Checks whether the given value is <b>false</b>.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<bool> IsFalse(this Ensures<bool> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v == false);
        }

        /// <summary>
        ///     Checks whether the given value is <b>false</b>.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<bool?> IsFalse(this Ensures<bool?> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v.HasValue && v.Value == false);
        }

        /// <summary>
        ///     Checks whether the given value is <b>true</b>.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<bool> IsTrue(this Ensures<bool> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v);
        }

        /// <summary>
        ///     Checks whether the given value is <b>true</b>.
        /// </summary>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Ensures<bool?> IsTrue(this Ensures<bool?> ensures)
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => v.HasValue && v.Value);
        }
    }
}