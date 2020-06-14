// *****************************************************************************************************************
// Project          : NavyBlue
// File             : EnsuresExtensions.Type.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:50
// *****************************************************************************************************************
// <copyright file="EnsuresExtensions.Type.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    ///     Extensions of Ensures utility for the <see cref="System.Type" />.
    /// </summary>
    public static partial class EnsuresExtensions
    {
        /// <summary>
        ///     Checks whether the <see cref="Type" /> of the given value is not of <paramref name="type" />.
        ///     When the given value is a null reference, the check will always pass, regardless of the specified
        ///     <paramref name="type" />. Please use the <b>IsNotNull</b> method to check for null references.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="type">The <see cref="Type" /> that will be used to perform the check.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsNotOfType<T>(this Ensures<T> ensures, Type type) where T : class
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.Not(v => type.IsInstanceOfType(v));
        }

        /// <summary>
        ///     Checks whether the <see cref="Type" /> of the given value is of <paramref name="type" />.
        ///     When the given value is a null reference, the check will always pass, regardless of the specified
        ///     <paramref name="type" />. Please use the <b>IsNotNull</b> method to check for null references.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Ensures{T}">Value</see> of the specified <paramref name="ensures" />.</typeparam>
        /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
        /// <param name="type">The <see cref="Type" /> that will be used to perform the check.</param>
        /// <returns>The specified <paramref name="ensures" /> instance.</returns>
        public static Ensures<T> IsOfType<T>(this Ensures<T> ensures, Type type) where T : class
        {
            if (ensures == null)
            {
                throw new ArgumentNullException(nameof(ensures));
            }

            return ensures.That(v => type.IsInstanceOfType(v));
        }
    }
}