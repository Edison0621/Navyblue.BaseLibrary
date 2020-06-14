// *****************************************************************************************************************
// Project          : NavyBlue
// File             : Type.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:53
// *****************************************************************************************************************
// <copyright file="Type.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Navyblue.BaseLibrary
{
    /// <summary>
    ///     Extensions of <see cref="System.Type" /> types.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Gets the type of nullable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        public static Type GetTypeOfNullable(this Type type)
        {
            return type.GetGenericArguments()[0];
        }

        /// <summary>
        ///     Determines whether the <paramref name="type" /> implements <typeparamref name="T" />.
        /// </summary>
        public static bool Implements<T>(this Type type)
        {
            return type != null && typeof(T).IsAssignableFrom(type);
        }

        /// <summary>
        ///     Determines whether [is collection type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is collection type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsCollectionType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
                return true;
            return type.GetInterfaces().Where(t => t.IsGenericType).Select(t => t.GetGenericTypeDefinition()).Any(t => t == typeof(ICollection<>));
        }

        /// <summary>
        ///     Determines whether [is dictionary type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is dictionary type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsDictionaryType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                return true;
            return type.GetInterfaces().Where(t => t.IsGenericType).Select(t => t.GetGenericTypeDefinition()).Any(t => t == typeof(IDictionary<,>));
        }

        /// <summary>
        ///     Determines whether [is enumerable type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is enumerable type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsEnumerableType(this Type type)
        {
            return type.GetInterfaces().Contains(typeof(IEnumerable));
        }

        /// <summary>
        ///     Determines whether [is list or dictionary type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is list or dictionary type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsListOrDictionaryType(this Type type)
        {
            return IsListType(type) || IsDictionaryType(type);
        }

        /// <summary>
        ///     Determines whether [is list type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is list type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsListType(this Type type)
        {
            return type != null && type.GetInterfaces().Contains(typeof(IList));
        }

        /// <summary>
        ///     Determines whether [is nullable type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is nullable type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsNullableType(this Type type)
        {
            if (type != null && type.IsGenericType)
                return type.GetGenericTypeDefinition() == typeof(Nullable<>);
            return false;
        }
    }
}