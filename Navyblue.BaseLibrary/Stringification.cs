// *****************************************************************************************************************
// Project          : NavyBlue
// File             : Stringification.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:53
// *****************************************************************************************************************
// <copyright file="Stringification.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;

namespace Navyblue.BaseLibrary
{
    /// <summary>
    ///     A helper class with extension methods for converting an object to a string representation.
    /// </summary>
    public static class Stringification
    {
        private const int MAXIMUM_NUMBER_OF_RECURSIVE_CALLS = 10;

        /// <summary>
        ///     Transforms an object into a string representation that can be used to represent it's value in an
        ///     exception message. When the value is a null reference, the string "null" will be returned, when
        ///     the specified value is a string or a char, it will be surrounded with single quotes.
        /// </summary>
        /// <param name="value">The value to be transformed.</param>
        /// <returns>A string representation of the supplied <paramref name="value" />.</returns>
        public static string Stringify(this object value)
        {
            try
            {
                return StringifyInternal(value, MAXIMUM_NUMBER_OF_RECURSIVE_CALLS);
            }
            catch (InvalidOperationException)
            {
                // Stack overflow prevented. We can not build a string representation of the supplied object.
                // We return the default representation of the object.
                return value.ToString();
            }
        }

        /// <summary>
        ///     Transforms an object into a json string representation. When the value is a null reference, the nullValue will be returned.
        /// </summary>
        /// <param name="value">The value to be transformed.</param>
        /// <param name="nullValue">The string to return when the value is a null reference.Default is null</param>
        /// <returns>A string representation of the supplied <paramref name="value" />.</returns>
        public static string ToJson(this object value, string nullValue = null)
        {
            if (value == null)
            {
                return nullValue;
            }

            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch
            {
                return value.ToString();
            }
        }

        private static string StringifyCollection(IEnumerable collection, int maximumNumberOfRecursiveCalls)
        {
            return "{" + string.Join(",", (from object o in collection select StringifyInternal(o, maximumNumberOfRecursiveCalls - 1)).ToArray()) + "}";
        }

        private static string StringifyInternal(object value, int maximumNumberOfRecursiveCalls)
        {
            if (value == null)
            {
                return "null";
            }

            if (maximumNumberOfRecursiveCalls < 0)
            {
                // Prevent stack overflow exceptions.
                throw new InvalidOperationException();
            }

            if (value is string || value is char)
            {
                return "'" + value + "'";
            }

            IEnumerable collection = value as IEnumerable;

            return collection != null ? StringifyCollection(collection, maximumNumberOfRecursiveCalls) : value.ToString();
        }
    }
}