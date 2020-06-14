// *****************************************************************************************************************
// Project          : NavyBlue
// File             : It.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:50
// *****************************************************************************************************************
// <copyright file="It.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    ///     Use <see cref="It" /> class to construct <see cref="Ensures{T}" /> instance.
    /// </summary>
    public static class It
    {
        /// <summary>
        ///     Construct a <see cref="Ensures{T}" /> instance for the value.
        /// </summary>
        /// <typeparam name="T">Type of the value to test/ensure.</typeparam>
        /// <param name="value">The value to test/ensure of the <see cref="Ensures{T}" /> instance.</param>
        /// <returns>The specified <see cref="Ensures{T}" /> instance.</returns>
        public static Ensures<T> Ensures<T>(T value)
        {
            return new Ensures<T>(value);
        }

        /// <summary>
        ///     Construct a <see cref="Ensures{T}" /> instance for an object.
        /// </summary>
        /// <returns>The specified <see cref="Ensures{T}" /> instance for an object.</returns>
        public static Ensures<object> Ensures()
        {
            return new Ensures<object>(new object());
        }
    }
}