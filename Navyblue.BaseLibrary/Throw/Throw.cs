// *****************************************************************************************************************
// Project          : NavyBlue
// File             : Throw.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:50
// *****************************************************************************************************************
// <copyright file="Throw.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Navyblue.BaseLibrary;

namespace NavyBlue.AspNetCore.Lib
{
    /// <summary>
    ///     All throw logic is factored out of the public extension methods and put in this helper class. This
    ///     allows more methods to be a candidate for inlining by the JIT compiler.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Throw
    {
        /// <summary>
        ///     Collections the should be empty.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldBeEmpty(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldBeEmpty, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should be empty.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldBeEmpty<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldBeEmpty, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should contain.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContain(string objectName, object value, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainX, additionalMessage, objectName, value.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should contain.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContain<TException>(string objectName, object value, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainX, additionalMessage, objectName, value.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should contain all of.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="values">The values.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainAllOf(string objectName, IEnumerable values, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainAllOfX, additionalMessage, objectName, values.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should contain all of.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="values">The values.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainAllOf<TException>(string objectName, IEnumerable values, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainAllOfX, additionalMessage, objectName, values.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should contain at least one of.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="values">The values.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainAtLeastOneOf(string objectName, IEnumerable values, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainAtLeastOneOfX, additionalMessage, objectName, values.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should contain at least one of.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="values">The values.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainAtLeastOneOf<TException>(string objectName, IEnumerable values, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainAtLeastOneOfX, additionalMessage, objectName, values.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should contain less or equal.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainLessOrEqual(string objectName, int numberOfElements, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainXOrLessElements, additionalMessage, objectName, numberOfElements);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should contain less or equal.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainLessOrEqual<TException>(string objectName, int numberOfElements, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainXOrLessElements, additionalMessage, objectName, numberOfElements);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should contain less than.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainLessThan(string objectName, int numberOfElements, string additionalMessage = "")
        {
            string exceptionMessage = numberOfElements == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainLessThan1Element, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainLessThanXElements, additionalMessage, objectName, numberOfElements);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should contain less than.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainLessThan<TException>(string objectName, int numberOfElements, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = numberOfElements == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainLessThan1Element, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainLessThanXElements, additionalMessage, objectName, numberOfElements);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should contain more or equal.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainMoreOrEqual(string objectName, int numberOfElements, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainXOrMoreElements, additionalMessage, objectName, numberOfElements);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should contain more or equal.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainMoreOrEqual<TException>(string objectName, int numberOfElements, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainXOrMoreElements, additionalMessage, objectName, numberOfElements);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should contain more than.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainMoreThan(string objectName, int numberOfElements, string additionalMessage = "")
        {
            string exceptionMessage = numberOfElements == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainMoreThan1Element, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainMoreThanXElements, additionalMessage, objectName, numberOfElements);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should contain more than.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainMoreThan<TException>(string objectName, int numberOfElements, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = numberOfElements == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainMoreThan1Element, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainMoreThanXElements, additionalMessage, objectName, numberOfElements);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should contain number of elements.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainNumberOfElements(string objectName, int numberOfElements, string additionalMessage = "")
        {
            string exceptionMessage = numberOfElements == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContain1Element, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainXElements, additionalMessage, objectName, numberOfElements);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should contain number of elements.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldContainNumberOfElements<TException>(string objectName, int numberOfElements, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = numberOfElements == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContain1Element, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldContainXElements, additionalMessage, objectName, numberOfElements);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should not be empty.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotBeEmpty(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotBeEmpty, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should not be empty.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotBeEmpty<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotBeEmpty, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should not contain.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContain(string objectName, object value, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainX, additionalMessage, objectName, value.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should not contain.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContain<TException>(string objectName, object value, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainX, additionalMessage, objectName, value.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should not contain all of.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="values">The values.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainAllOf(string objectName, IEnumerable values, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainAllOfX, additionalMessage, objectName, values.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should not contain all of.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="values">The values.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainAllOf<TException>(string objectName, IEnumerable values, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainAllOfX, additionalMessage, objectName, values.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should not contain any of.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="values">The values.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainAnyOf(string objectName, IEnumerable values, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainAnyOfX, additionalMessage, objectName, values.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should not contain any of.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="values">The values.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainAnyOf<TException>(string objectName, IEnumerable values, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainAnyOfX, additionalMessage, objectName, values.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should not contain less or equal.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainLessOrEqual(string objectName, int numberOfElements, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainXOrLessElements, additionalMessage, objectName, numberOfElements);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should not contain less or equal.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainLessOrEqual<TException>(string objectName, int numberOfElements, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainXOrLessElements, additionalMessage, objectName, numberOfElements);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should not contain less than.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainLessThan(string objectName, int numberOfElements, string additionalMessage = "")
        {
            string exceptionMessage = numberOfElements == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainLessThan1Element, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainLessThanXElements, additionalMessage, objectName, numberOfElements);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should not contain less than.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainLessThan<TException>(string objectName, int numberOfElements, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = numberOfElements == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainLessThan1Element, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainLessThanXElements, additionalMessage, objectName, numberOfElements);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should not contain more or equal.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainMoreOrEqual(string objectName, int numberOfElements, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainXOrMoreElements, additionalMessage, objectName, numberOfElements);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should not contain more or equal.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainMoreOrEqual<TException>(string objectName, int numberOfElements, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainXOrMoreElements, additionalMessage, objectName, numberOfElements);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should not contain more than.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainMoreThan(string objectName, int numberOfElements, string additionalMessage = "")
        {
            string exceptionMessage = numberOfElements == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainMoreThan1Element, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainMoreThanXElements, additionalMessage, objectName, numberOfElements);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should not contain more than.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainMoreThan<TException>(string objectName, int numberOfElements, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = numberOfElements == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainMoreThan1Element, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainMoreThanXElements, additionalMessage, objectName, numberOfElements);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Collections the should not contain number of elements.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainNumberOfElements(string objectName, int numberOfElements, string additionalMessage = "")
        {
            string exceptionMessage = numberOfElements == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContain1Element, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainXElements, additionalMessage, objectName, numberOfElements);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Collections the should not contain number of elements.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="numberOfElements">The number of elements.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CollectionShouldNotContainNumberOfElements<TException>(string objectName, int numberOfElements, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = numberOfElements == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContain1Element, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.CollectionShouldNotContainXElements, additionalMessage, objectName, numberOfElements);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the should be longer or equal to.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldBeLongerOrEqualTo(string objectName, int minLength, string additionalMessage = "")
        {
            string exceptionMessage = minLength == 1 ? GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeLongerOrEqualTo1Character, additionalMessage, objectName) : GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeLongerOrEqualToXCharacters, additionalMessage, objectName, minLength);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the should be longer or equal to.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldBeLongerOrEqualTo<TException>(string objectName, int minLength, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = minLength == 1 ? GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeLongerOrEqualTo1Character, additionalMessage, objectName) : GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeLongerOrEqualToXCharacters, additionalMessage, objectName, minLength);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the should be longer than.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldBeLongerThan(string objectName, int minLength, string additionalMessage = "")
        {
            string exceptionMessage = minLength == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeLongerThan1Character, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeLongerThanXCharacters, additionalMessage, objectName, minLength);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the should be longer than.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldBeLongerThan<TException>(string objectName, int minLength, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = minLength == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeLongerThan1Character, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeLongerThanXCharacters, additionalMessage, objectName, minLength);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the should be null or white space.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldBeNullOrWhiteSpace(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeNullOrWhiteSpace, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the should be null or white space.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldBeNullOrWhiteSpace<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeNullOrWhiteSpace, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the should be shorter or equal to.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldBeShorterOrEqualTo(string objectName, int maxLength, string additionalMessage = "")
        {
            string exceptionMessage = maxLength == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeShorterOrEqualTo1Character, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeShorterOrEqualToXCharacters, additionalMessage, objectName, maxLength);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the should be shorter or equal to.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldBeShorterOrEqualTo<TException>(string objectName, int maxLength, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = maxLength == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeShorterOrEqualTo1Character, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeShorterOrEqualToXCharacters, additionalMessage, objectName, maxLength);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the should be shorter than.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldBeShorterThan(string objectName, int maxLength, string additionalMessage = "")
        {
            string exceptionMessage = maxLength == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeShorterThan1Character, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeShorterThanXCharacters, additionalMessage, objectName, maxLength);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the should be shorter than.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldBeShorterThan<TException>(string objectName, int maxLength, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = maxLength == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeShorterThan1Character, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeShorterThanXCharacters, additionalMessage, objectName, maxLength);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the should contain.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldContain(string objectName, string value, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldContainX, additionalMessage, objectName, value.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the should contain.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldContain<TException>(string objectName, string value, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldContainX, additionalMessage, objectName, value.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the should end with.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldEndWith(string objectName, string value, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldEndWithX, additionalMessage, objectName, value.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the should end with.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldEndWith<TException>(string objectName, string value, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldEndWithX, additionalMessage, objectName, value.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the length of the should have.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="length">The length.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldHaveLength(string objectName, int length, string additionalMessage = "")
        {
            string exceptionMessage = length == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBe1CharacterLong, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeXCharactersLong, additionalMessage, objectName, length);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the length of the should have.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="length">The length.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldHaveLength<TException>(string objectName, int length, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = length == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBe1CharacterLong, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeXCharactersLong, additionalMessage, objectName, length);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the should not be null or white space.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldNotBeNullOrWhiteSpace(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotBeNullOrWhiteSpace, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the should not be null or white space.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldNotBeNullOrWhiteSpace<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotBeNullOrWhiteSpace, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the should not contain.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldNotContain(string objectName, string value, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotContainX, additionalMessage, objectName, value.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the should not contain.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldNotContain<TException>(string objectName, string value, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotContainX, additionalMessage, objectName, value.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the should not end with.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldNotEndWith(string objectName, string value, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotEndWithX, additionalMessage, objectName, value.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the should not end with.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldNotEndWith<TException>(string objectName, string value, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotEndWithX, additionalMessage, objectName, value.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the length of the should not have.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="length">The length.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldNotHaveLength(string objectName, int length, string additionalMessage = "")
        {
            string exceptionMessage = length == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotBe1CharacterLong, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotBeXCharactersLong, additionalMessage, objectName, length);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the length of the should not have.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="length">The length.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldNotHaveLength<TException>(string objectName, int length, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = length == 1
                ? GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotBe1CharacterLong, additionalMessage, objectName)
                : GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotBeXCharactersLong, additionalMessage, objectName, length);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the should not start with.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldNotStartWith(string objectName, string value, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotStartWithX, additionalMessage, objectName, value.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the should not start with.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldNotStartWith<TException>(string objectName, string value, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotStartWithX, additionalMessage, objectName, value.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Strings the should start with.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldStartWith(string objectName, string value, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldStartWithX, additionalMessage, objectName, value.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Strings the should start with.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void StringShouldStartWith<TException>(string objectName, string value, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldStartWithX, additionalMessage, objectName, value.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be an empty string.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeAnEmptyString(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeEmpty, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be an empty string.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeAnEmptyString<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeEmpty, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be a number.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeANumber(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeANumber, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be a number.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeANumber<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeANumber, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be between.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldBeBetween<T>(string objectName, T minValue, T maxValue, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeBetweenXAndY, additionalMessage, objectName, minValue.Stringify(), maxValue.Stringify());
            throw new ArgumentOutOfRangeException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be between.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldBeBetween<T, TException>(string objectName, T minValue, T maxValue, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeBetweenXAndY, additionalMessage, objectName, minValue.Stringify(), maxValue.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be equal to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeEqualTo<T>(string objectName, T value, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeEqualToX, additionalMessage, objectName, value.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be equal to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeEqualTo<T, TException>(string objectName, T value, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeEqualToX, additionalMessage, objectName, value.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be false.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeFalse(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeFalse, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be false.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeFalse<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeFalse, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be greater than.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldBeGreaterThan<T>(string objectName, T minValue, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeGreaterThanX, additionalMessage, objectName, minValue.Stringify());
            throw new ArgumentOutOfRangeException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be greater than.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldBeGreaterThan<T, TException>(string objectName, T minValue, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeGreaterThanX, additionalMessage, objectName, minValue.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be greater than or equal to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldBeGreaterThanOrEqualTo<T>(string objectName, T minValue, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeGreaterThanOrEqualToX, additionalMessage, objectName, minValue.Stringify());
            throw new ArgumentOutOfRangeException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be greater than or equal to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldBeGreaterThanOrEqualTo<T, TException>(string objectName, T minValue, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeGreaterThanOrEqualToX, additionalMessage, objectName, minValue.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be infinity.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeInfinity(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeInfinity, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be infinity.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeInfinity<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeInfinity, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be negative infinity.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeNegativeInfinity(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeNegativeInfinity, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be negative infinity.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeNegativeInfinity<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeNegativeInfinity, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be null.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeNull(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeNull, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be null.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeNull<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeNull, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be null or an empty string.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeNullOrAnEmptyString(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeNullOrEmpty, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be null or an empty string.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeNullOrAnEmptyString<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldBeNullOrEmpty, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the type of the should be of.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="type">The type.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeOfType(string objectName, Type type, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeOfTypeX, additionalMessage, objectName, type.Name);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the type of the should be of.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="type">The type.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeOfType<TException>(string objectName, Type type, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeOfTypeX, additionalMessage, objectName, type.Name);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be positive infinity.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBePositiveInfinity(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBePositiveInfinity, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be positive infinity.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBePositiveInfinity<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBePositiveInfinity, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be smaller than.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldBeSmallerThan<T>(string objectName, T maxValue, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeSmallerThanX, additionalMessage, objectName, maxValue.Stringify());
            throw new ArgumentOutOfRangeException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be smaller than.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldBeSmallerThan<T, TException>(string objectName, T maxValue, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeSmallerThanX, additionalMessage, objectName, maxValue.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be smaller than or equal to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldBeSmallerThanOrEqualTo<T>(string objectName, T maxValue, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeSmallerThanOrEqualToX, additionalMessage, objectName, maxValue.Stringify());
            throw new ArgumentOutOfRangeException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be smaller than or equal to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldBeSmallerThanOrEqualTo<T, TException>(string objectName, T maxValue, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeSmallerThanOrEqualToX, additionalMessage, objectName, maxValue.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be true.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeTrue(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeTrue, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be true.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeTrue<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeTrue, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should be unequal to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeUnequalTo<T>(string objectName, T value, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeUnequalToX, additionalMessage, objectName, value.Stringify());
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should be unequal to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="value">The value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldBeUnequalTo<T, TException>(string objectName, T value, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldBeUnequalToX, additionalMessage, objectName, value.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should not be an empty string.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBeAnEmptyString(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotBeEmpty, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should not be an empty string.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBeAnEmptyString<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotBeEmpty, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should not be a number.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBeANumber(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeANumber, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should not be a number.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBeANumber<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeANumber, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should not be between.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldNotBeBetween<T>(string objectName, T minValue, T maxValue, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeBetweenXAndY, additionalMessage, objectName, minValue.Stringify(), maxValue.Stringify());
            throw new ArgumentOutOfRangeException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should not be between.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldNotBeBetween<T, TException>(string objectName, T minValue, T maxValue, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeBetweenXAndY, additionalMessage, objectName, minValue.Stringify(), maxValue.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should not be greater than.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldNotBeGreaterThan<T>(string objectName, T minValue, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeGreaterThanX, additionalMessage, objectName, minValue.Stringify());
            throw new ArgumentOutOfRangeException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should not be greater than.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldNotBeGreaterThan<T, TException>(string objectName, T minValue, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeGreaterThanX, additionalMessage, objectName, minValue.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should not be greater than or equal to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldNotBeGreaterThanOrEqualTo<T>(string objectName, T maxValue, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeGreaterThanOrEqualToX, additionalMessage, objectName, maxValue.Stringify());
            throw new ArgumentOutOfRangeException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should not be greater than or equal to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldNotBeGreaterThanOrEqualTo<T, TException>(string objectName, T maxValue, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeGreaterThanOrEqualToX, additionalMessage, objectName, maxValue.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should not be infinity.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBeInfinity(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeInfinity, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should not be infinity.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldNotBeInfinity<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeInfinity, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should not be negative infinity.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBeNegativeInfinity(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeNegativeInfinity, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should not be negative infinity.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBeNegativeInfinity<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeNegativeInfinity, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should not be null.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ValueShouldNotBeNull(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeNull, additionalMessage, objectName);
            throw new ArgumentNullException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should not be null.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ValueShouldNotBeNull<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeNull, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentNullException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should not be null or an empty string.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBeNullOrAnEmptyString(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotBeNullOrEmpty, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should not be null or an empty string.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBeNullOrAnEmptyString<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.StringShouldNotBeNullOrEmpty, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the type of the should not be of.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="type">The type.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBeOfType(string objectName, Type type, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeOfTypeX, additionalMessage, objectName, type.Name);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the type of the should not be of.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="type">The type.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBeOfType<TException>(string objectName, Type type, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeOfTypeX, additionalMessage, objectName, type.Name);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should not be positive infinity.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBePositiveInfinity(string objectName, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBePositiveInfinity, additionalMessage, objectName);
            throw new ArgumentException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should not be positive infinity.
        /// </summary>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ValueShouldNotBePositiveInfinity<TException>(string objectName, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBePositiveInfinity, additionalMessage, objectName);
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should not be smaller than.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldNotBeSmallerThan<T>(string objectName, T minValue, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeSmallerThanX, additionalMessage, objectName, minValue.Stringify());
            throw new ArgumentOutOfRangeException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should not be smaller than.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldNotBeSmallerThan<T, TException>(string objectName, T minValue, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeSmallerThanX, additionalMessage, objectName, minValue.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Values the should not be smaller than or equal to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldNotBeSmallerThanOrEqualTo<T>(string objectName, T minValue, string additionalMessage = "")
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeSmallerThanOrEqualToX, additionalMessage, objectName, minValue.Stringify());
            throw new ArgumentOutOfRangeException(exceptionMessage);
        }

        /// <summary>
        ///     Values the should not be smaller than or equal to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException">The type of the t exception.</typeparam>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ValueShouldNotBeSmallerThanOrEqualTo<T, TException>(string objectName, T minValue, string additionalMessage = "") where TException : Exception
        {
            string exceptionMessage = GetFormattedExceptionMessage(ExceptionMessagesManager.ValueShouldNotBeSmallerThanOrEqualToX, additionalMessage, objectName, minValue.Stringify());
            try
            {
                throw ((TException)Activator.CreateInstance(typeof(TException), exceptionMessage))!;
            }
            catch
            {
                throw new ArgumentOutOfRangeException(exceptionMessage);
            }
        }

        /// <summary>
        ///     Gets the formatted exception message.
        /// </summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="additionalMessage">The additional message.</param>
        /// <param name="resourceFormatArguments">The resource format arguments.</param>
        /// <returns>System.String.</returns>
        private static string GetFormattedExceptionMessage(string resourceKey, string additionalMessage, params object[] resourceFormatArguments)
        {
            return "{0} {1}".FormatWith(ExceptionMessagesManager.GetString(resourceKey, resourceFormatArguments), additionalMessage);
        }
    }
}