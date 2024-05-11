// *****************************************************************************************************************
// Project          : NavyBlue
// File             : Ensures.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:49
// *****************************************************************************************************************
// <copyright file="Ensures.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ?  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Diagnostics.CodeAnalysis;
using Navyblue.BaseLibrary;

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    ///     A <see cref="Ensures{T}" /> is used to ensure the predicate is true.
    /// </summary>
    /// <typeparam name="T">The type of the object to test for.</typeparam>
    public class Ensures<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Ensures{T}" /> class.
        /// </summary>
        /// <param name="value">The value to test/ensure of this <see cref="Ensures{T}" />.</param>
        public Ensures(T value)
        {
            this.Value = value;
            this.Result = false;
        }

        /// <summary>
        ///     Gets a value indicating whether the result of this <see cref="Ensures{T}" /> is true.
        /// </summary>
        /// <value><c>true</c> if the result of this <see cref="Ensures{T}" /> is true; otherwise, <c>false</c>.</value>
        public bool Result { get; private set; }

        /// <summary>
        ///     Gets The value to test/ensure of this <see cref="Ensures{T}" />..
        /// </summary>
        public T Value { get; }

        /// <summary>
        ///     Ensures that the given predicate is false.
        /// </summary>
        /// <param name="predicate">Predicate to test/ensure.</param>
        /// <returns>This <see cref="Ensures{T}" /> instance.</returns>
        /// <remarks>The ensure result would be set into the Result property of the instance.</remarks>
        public Ensures<T> Not(Func<T, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            this.Result = !predicate.Invoke(this.Value);
            return this;
        }

        /// <summary>
        ///     Ensures that the given predicate is true.
        /// </summary>
        /// <param name="predicate">Predicate to test/ensure.</param>
        /// <returns>This <see cref="Ensures{T}" /> instance.</returns>
        /// <remarks>The ensure result would be set into the Result property of the instance.</remarks>
        public Ensures<T> That(Func<T, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            this.Result = predicate.Invoke(this.Value);
            return this;
        }

        /// <summary>
        ///     Ensures that the given predicate is true.
        /// </summary>
        /// <param name="predicate">Predicate to test/ensure.</param>
        /// <returns>This <see cref="Ensures{T}" /> instance.</returns>
        /// <remarks>The ensure result would be set into the Result property of the instance.</remarks>
        public Ensures<T> That(Func<bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            this.Result = predicate.Invoke();
            return this;
        }

        /// <summary>
        ///     Throw the exception when the result is false.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <returns><c>true</c> if the Result is true, <c>throw a TException</c> otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public bool WithException<TException>() where TException : Exception
        {
            if (this.Result)
            {
                return this.Result;
            }

            throw ((TException)Activator.CreateInstance(typeof(TException)))!;
        }

        /// <summary>
        ///     Throw the exception when the result is false.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="exception">The exception to throw.</param>
        /// <returns><c>true</c> if the Result is true, <c>throw a TException</c> otherwise.</returns>
        public bool WithException<TException>(TException exception) where TException : Exception
        {
            if (this.Result)
            {
                return this.Result;
            }

            throw exception;
        }

        /// <summary>
        ///     Throw the exception when the result is false.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="message">The exception message.</param>
        /// <param name="args">The args for formating the exception message.</param>
        /// <returns><c>true</c> if the Result is true, <c>throw a TException</c> otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public bool WithException<TException>(string message, params object[] args) where TException : Exception
        {
            if (this.Result)
            {
                return this.Result;
            }

            throw ((TException)Activator.CreateInstance(typeof(TException), message.FormatWith(args)))!;
        }
    }
}