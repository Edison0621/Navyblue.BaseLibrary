// *****************************************************************************************************************
// Project          : NavyBlue
// File             : Dictionary.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:51
// *****************************************************************************************************************
// <copyright file="Dictionary.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Collections.Generic;

namespace Navyblue.BaseLibrary
{
    /// <summary>
    ///     Extension methods for <see cref="System.Collections.Generic.IDictionary{TKey, TValue}" />
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        ///     Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" /> if the key does not exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void AddIfNotExist<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (dictionary.ContainsKey(key)) return;

            dictionary.Add(key, value);
        }

        /// <summary>
        ///     Gets the value associated with the specified key or the <paramref name="defaultValue" /> if it does not exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="dictionary">The dictionary object.</param>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="defaultValue">The default value to return if an item with the specified <paramref name="key" /> does not exist.</param>
        /// <returns>The value associated with the specified key or the <paramref name="defaultValue" /> if it does not exist.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }
    }
}