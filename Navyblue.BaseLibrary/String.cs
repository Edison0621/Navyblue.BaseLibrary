// *****************************************************************************************************************
// Project          : NavyBlue
// File             : String.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:52
// *****************************************************************************************************************
// <copyright file="String.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Navyblue.BaseLibrary
{
    /// <summary>
    ///     Extensions for <see cref="System.String" />.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Converts a string to a strongly typed value of the specified data type.
        /// </summary>
        /// <typeparam name="TValue">The data type to convert to.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static TValue As<TValue>(this string value)
        {
            return StringUtility.As<TValue>(value);
        }

        /// <summary>
        ///     Converts a string to the specified data type and specifies a default value.
        /// </summary>
        /// <typeparam name="TValue">The data type to convert to.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        public static TValue As<TValue>(this string value, TValue defaultValue)
        {
            return StringUtility.As(value, defaultValue);
        }

        /// <summary>
        ///     Converts a string to a Boolean (true/false) value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static bool AsBoolean(this string value)
        {
            return StringUtility.AsBoolean(value);
        }

        /// <summary>
        ///     Converts a string to a Boolean (true/false) value and specifies a default value.
        /// </summary>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static bool AsBoolean(this string value, bool defaultValue)
        {
            return StringUtility.AsBoolean(value, defaultValue);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.DateTime" /> value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static DateTime AsDateTime(this string value)
        {
            return StringUtility.AsDateTime(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.DateTime" /> value and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value. The default is the minimum time value on the system.</param>
        /// <returns>The converted value.</returns>
        public static DateTime AsDateTime(this string value, DateTime defaultValue)
        {
            return StringUtility.AsDateTime(value, defaultValue);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Decimal" /> number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static decimal AsDecimal(this string value)
        {
            return StringUtility.AsDecimal(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Decimal" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or invalid.</param>
        /// <returns>The converted value.</returns>
        public static decimal AsDecimal(this string value, decimal defaultValue)
        {
            return StringUtility.AsDecimal(value, defaultValue);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Double" /> number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double AsDouble(this string value)
        {
            return StringUtility.AsDouble(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Double" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or invalid.</param>
        /// <returns>The converted value.</returns>
        public static double AsDouble(this string value, double defaultValue)
        {
            return StringUtility.AsDouble(value, defaultValue);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.float" /> number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        public static float AsFloat(this string value)
        {
            return StringUtility.AsFloat(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.float" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        public static float AsFloat(this string value, float defaultValue)
        {
            return StringUtility.AsFloat(value, defaultValue);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Guid" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">The format of the value.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or invalid.</param>
        /// <returns>The converted value.</returns>
        public static Guid AsGuid(this string value, string format = "N", Guid? defaultValue = null)
        {
            Guid result;
            return !Guid.TryParseExact(value, format, out result) ? defaultValue.GetValueOrDefault(Guid.Empty) : result;
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.int" /> number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        public static int AsInt(this string value)
        {
            return StringUtility.AsInt(value);
        }

        /// <summary>
        ///     Converts a string to to a <see cref="T:System.int" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        public static int AsInt(this string value, int defaultValue)
        {
            return StringUtility.AsInt(value, defaultValue);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Int16" /> number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static short AsInt16(this string value)
        {
            return StringUtility.AsInt16(value);
        }

        /// <summary>
        ///     Converts a string to to a <see cref="T:System.int" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        public static short AsInt16(this string value, short defaultValue)
        {
            return StringUtility.AsInt16(value, defaultValue);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Int32" /> number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static int AsInt32(this string value)
        {
            return StringUtility.AsInt32(value);
        }

        /// <summary>
        ///     Converts a string to to a <see cref="T:System.int" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        public static int AsInt32(this string value, int defaultValue)
        {
            return StringUtility.AsInt32(value, defaultValue);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Int64" /> number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static long AsInt64(this string value)
        {
            return StringUtility.AsInt64(value);
        }

        /// <summary>
        ///     Converts a string to to a <see cref="T:System.int" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        public static long AsInt64(this string value, long defaultValue)
        {
            return StringUtility.AsInt64(value, defaultValue);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.long" /> number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long")]
        public static long AsLong(this string value)
        {
            return StringUtility.AsLong(value);
        }

        /// <summary>
        ///     Converts a string to to a <see cref="T:System.int" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long")]
        public static long AsLong(this string value, long defaultValue)
        {
            return StringUtility.AsLong(value, defaultValue);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Single" /> number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float AsSingle(this string value)
        {
            return StringUtility.AsSingle(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Single" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        public static float AsSingle(this string value, float defaultValue)
        {
            return StringUtility.AsSingle(value, defaultValue);
        }

        /// <summary>
        ///     Concats the specified target.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>System.String.</returns>
        public static string Concat(this string source, string target)
        {
            return string.Concat(source, target);
        }

        /// <summary>
        ///     Checks if the <paramref name="source" /> contains the <paramref name="input" /> based on the provided <paramref name="comparison" /> rules.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="input">The input.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns><c>true</c> if [contains] [the specified input]; otherwise, <c>false</c>.</returns>
        public static bool Contains(this string source, string input, StringComparison comparison)
        {
            return StringUtility.Contains(source, input, comparison);
        }

        /// <summary>
        ///     Checks if the <paramref name="source" /> contains the <paramref name="input" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="input">The input.</param>
        /// <returns><c>true</c> if [contains] [the specified input]; otherwise, <c>false</c>.</returns>
        public static bool Contains(this string source, string input)
        {
            return StringUtility.Contains(source, input);
        }

        /// <summary>
        ///     A nicer way of calling <see cref="System.String.Format(string, object[])" />
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return StringUtility.FormatWith(format, args);
        }

        /// <summary>
        ///     A nicer way of calling <see cref="System.String.Format(string, object[])" />
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="provider">String format provider</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            return StringUtility.FormatWith(format, provider, args);
        }

        /// <summary>
        ///     Deserialize a json string to the instance of <typeparamref name="T" />. If the <paramref name="value" /> is null or empty, <c>null</c> will be returned.
        /// </summary>
        /// <typeparam name="T">The type of the instance to deserialize from the json string.</typeparam>
        /// <param name="value">The json string .</param>
        /// <returns>The deserialized instance of <typeparamref name="T" />. If the <paramref name="value" /> is null or empty, <c>null</c> will be returned.</returns>
        public static T FromJson<T>(this string value)
        {
            return StringUtility.FromJson<T>(value);
        }

        /// <summary>
        ///     Gets the substring of the first chars.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        public static string GetFirst(this string value, int count = 1)
        {
            return StringUtility.GetFirst(value, count);
        }

        /// <summary>
        ///     Gets the substring of the last chars.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        public static string GetLast(this string value, int count = 1)
        {
            return StringUtility.GetLast(value, count);
        }

        /// <summary>
        ///     Converts a string that has been HTML-encoded for HTTP transmission into a decoded string.
        /// </summary>
        /// <param name="value">The string to decode. </param>
        /// <returns>
        ///     A decoded string.
        /// </returns>
        public static string HtmlDecode(this string value)
        {
            return StringUtility.HtmlDecode(value);
        }

        /// <summary>
        ///     Converts a string to an HTML-encoded string.
        /// </summary>
        /// <param name="value">The string to encode. </param>
        /// <returns>
        ///     An encoded string.
        /// </returns>
        public static string HtmlEncode(this string value)
        {
            return StringUtility.HtmlEncode(value);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the specified data type.
        /// </summary>
        /// <typeparam name="TValue">The data type to convert to.</typeparam>
        /// <param name="value">The value to test/ensure.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static bool Is<TValue>(this string value)
        {
            return StringUtility.Is<TValue>(value);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the Boolean (true/false) type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool")]
        public static bool IsBool(this string value)
        {
            return StringUtility.IsBool(value);
        }

        /// <summary>
        ///     Validates whether the provided <paramref name="value">string</paramref> is a valid cellphone number(13712341234).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is cellphone; otherwise, <c>false</c>.</returns>
        public static bool IsCellphone(this string value)
        {
            return StringUtility.IsCellphone(value);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the <see cref="T:System.DateTime" /> type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        public static bool IsDateTime(this string value)
        {
            return StringUtility.IsDateTime(value);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the <see cref="T:System.Decimal" /> type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        public static bool IsDecimal(this string value)
        {
            return StringUtility.IsDecimal(value);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the Boolean (true/false) type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        public static bool IsDouble(this string value)
        {
            return StringUtility.IsDouble(value);
        }

        //// <param name="value">The string value to test.</param>
        /// <summary>
        ///     Validates whether the provided <paramref name="value">string</paramref> is a valid Email Address.
        /// </summary>
        /// <returns><c>true</c> if the specified value is email; otherwise, <c>false</c>.</returns>
        public static bool IsEmail(this string value)
        {
            return StringUtility.IsEmail(value);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the <see cref="T:System.Single" /> type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        public static bool IsFloat(this string value)
        {
            return StringUtility.IsFloat(value);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the Guid (true/false) type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <param name="format">The format of the value.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        public static bool IsGuid(this string value, string format = "N")
        {
            return Guid.TryParseExact(value, format, out Guid _);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the <see cref="T:System.int" /> type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        public static bool IsInt(this string value)
        {
            return StringUtility.IsInt(value);
        }

        /// <summary>
        ///     Checks whether a string can be converted to to the <see cref="T:System.Int16" /> type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        public static bool IsInt16(this string value)
        {
            return StringUtility.IsInt16(value);
        }

        /// <summary>
        ///     Checks whether a string can be converted to to the <see cref="T:System.Int32" /> type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        public static bool IsInt32(this string value)
        {
            return StringUtility.IsInt32(value);
        }

        /// <summary>
        ///     Checks whether a string can be converted to to the <see cref="T:System.Int64" /> type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        public static bool IsInt64(this string value)
        {
            return StringUtility.IsInt64(value);
        }

        /// <summary>
        ///     Validates whether the provided <paramref name="value">string</paramref> is a valid IP Address.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns><c>true</c> if [is ip address] [the specified value]; otherwise, <c>false</c>.</returns>
        public static bool IsIPAddress(this string value)
        {
            return StringUtility.IsIPAddress(value);
        }

        /// <summary>
        ///     Checks whether a string can be converted to a long.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        public static bool IsLong(this string value)
        {
            return StringUtility.IsLong(value);
        }

        /// <summary>
        ///     A nicer way of calling the inverse of <see cref="System.String.IsNullOrEmpty(string)" />
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is not null or an empty string (""); otherwise, false.</returns>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return StringUtility.IsNotNullOrEmpty(value);
        }

        /// <summary>
        ///     A nicer way of calling the inverse of <see cref="System.String.IsNullOrWhiteSpace(string)" />
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is not null or an empty string (""); otherwise, false.</returns>
        public static bool IsNotNullOrWhiteSpace(this string value)
        {
            return StringUtility.IsNotNullOrWhiteSpace(value);
        }

        /// <summary>
        ///     A nicer way of calling <see cref="System.String.IsNullOrEmpty(string)" />
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return StringUtility.IsNullOrEmpty(value);
        }

        /// <summary>
        ///     A nicer way of calling <see cref="System.String.IsNullOrWhiteSpace(string)" />
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return StringUtility.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        ///     Validates whether the provided <paramref name="value">string</paramref> is a valid (absolute) URL.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns><c>true</c> if the specified value is URL; otherwise, <c>false</c>.</returns>
        public static bool IsUrl(this string value) // absolute
        {
            return StringUtility.IsUrl(value);
        }

        /// <summary>
        ///     Joins the specified string array with the separator.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>System.String.</returns>
        public static string Join(this IEnumerable<string> items, string separator = "")
        {
            return string.Join(separator, items.ToArray());
        }

        /// <summary>
        ///     Limits the length of the <paramref name="source" /> to the specified <paramref name="maxLength" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>System.String.</returns>
        public static string Limit(this string source, int maxLength, string suffix = "")
        {
            return StringUtility.Limit(source, maxLength, suffix);
        }

        /// <summary>
        ///     Matches the specified regex.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="regex">The regex.</param>
        /// <returns><c>true</c> if match, <c>false</c> otherwise.</returns>
        public static bool Match(this string value, Regex regex)
        {
            return StringUtility.Match(value, regex);
        }

        /// <summary>
        ///     Gets the string md5 hash.
        /// </summary>
        /// <param name="value">The string to hash.</param>
        /// <returns>System.String.</returns>
        public static string MD5Hash(this string value)
        {
            return StringUtility.MD5Hash(value);
        }

        /// <summary>
        ///     Removes the specified target string from value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="target">The target.</param>
        /// <returns>System.String.</returns>
        public static string Remove(this string value, string target)
        {
            return StringUtility.Remove(value, target);
        }

        /// <summary>
        ///     Separates a PascalCase string
        /// </summary>
        /// <example>
        ///     "ThisIsPascalCase".SeparatePascalCase(); // returns "This Is Pascal Case"
        /// </example>
        /// <param name="value">The value to split</param>
        /// <returns>The original string separated on each uppercase character.</returns>
        public static string SeparatePascalCase(this string value)
        {
            return StringUtility.SeparatePascalCase(value);
        }

        /// <summary>
        ///     Returns a string array containing the trimmed substrings in this <paramref name="value" />
        ///     that are delimited by the provided <paramref name="separators" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="separators">The separators.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> SplitAndTrim(this string value, params char[] separators)
        {
            return StringUtility.SplitAndTrim(value, separators);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.bool" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool")]
        public static bool ToBool(this string value)
        {
            return StringUtility.ToBool(value);
        }

        /// <summary>
        ///     Converts a string to the CamelCase style.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static string ToCamelCase(string value)
        {
            return StringUtility.ToCamelCase(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.DateTime" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static DateTime ToDateTime(this string value)
        {
            return StringUtility.ToDateTime(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.decimal" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static decimal ToDecimal(this string value)
        {
            return StringUtility.ToDecimal(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.double" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double ToDouble(this string value)
        {
            return StringUtility.ToDouble(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.float" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        public static float ToFloat(this string value)
        {
            return StringUtility.ToFloat(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Guid" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">The format of the value.</param>
        /// <returns>The converted value.</returns>
        public static Guid ToGuid(this string value, string format = "N")
        {
            return Guid.ParseExact(value, format);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.int" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        public static int ToInt(this string value)
        {
            return StringUtility.ToInt(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Int16" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static short ToInt16(this string value)
        {
            return StringUtility.ToInt16(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Int32" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static int ToInt32(this string value)
        {
            return StringUtility.ToInt32(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Int64" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static long ToInt64(this string value)
        {
            return StringUtility.ToInt64(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.long" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long")]
        public static long ToLong(this string value)
        {
            return StringUtility.ToLong(value);
        }

        /// <summary>
        ///     Hide some chars of the string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static string ToMosaicString(string value)
        {
            return StringUtility.ToMosaicString(value);
        }

        /// <summary>
        ///     Allows for using strings in null coalescing operations
        /// </summary>
        /// <param name="value">The string value to check</param>
        /// <returns>Null if <paramref name="value" /> is empty or the original value of <paramref name="value" />.</returns>
        public static string ToNullIfEmpty(this string value)
        {
            return StringUtility.ToNullIfEmpty(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Single" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float ToSingle(this string value)
        {
            return StringUtility.ToSingle(value);
        }

        /// <summary>
        ///     Converts a string to the underscope style.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static string ToUnderScope(this string value)
        {
            return StringUtility.ToUnderScope(value);
        }
    }

    /// <summary>
    ///     Utilities for working with <see cref="System.String" /> type.
    /// </summary>
    public static class StringUtility
    {
        private const int LOWER_CASE_OFFSET = 'a' - 'A';

        /// <summary>
        ///     Converts a string to the specified data type and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        /// <typeparam name="TValue">The data type to convert to.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static TValue As<TValue>(string value, TValue defaultValue = default(TValue))
        {
            try
            {
                TypeConverter converter1 = TypeDescriptor.GetConverter(typeof(TValue));
                if (converter1.CanConvertFrom(typeof(string)))
                    return (TValue)converter1.ConvertFrom(value);
                TypeConverter converter2 = TypeDescriptor.GetConverter(typeof(string));
                if (converter2.CanConvertTo(typeof(TValue)))
                    return (TValue)converter2.ConvertTo(value, typeof(TValue));
            }
            catch
            {
                // ignored
            }

            return defaultValue;
        }

        /// <summary>
        ///     Converts a string to a Boolean (true/false) value and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        public static bool AsBoolean(string value, bool defaultValue = false)
        {
            bool result;
            return !bool.TryParse(value, out result) ? defaultValue : result;
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.DateTime" /> value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static DateTime AsDateTime(string value)
        {
            return AsDateTime(value, DateTime.UtcNow);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.DateTime" /> value and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value. The default is the minimum time value on the system.</param>
        public static DateTime AsDateTime(string value, DateTime defaultValue)
        {
            DateTime result;
            return !DateTime.TryParse(value, out result) ? defaultValue : result;
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Decimal" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or invalid.</param>
        public static decimal AsDecimal(string value, decimal defaultValue = 0m)
        {
            decimal result;
            return !decimal.TryParse(value, out result) ? defaultValue : result;
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Double" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or invalid.</param>
        public static double AsDouble(string value, double defaultValue = 0d)
        {
            double result;
            return !double.TryParse(value, out result) ? defaultValue : result;
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.float" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float")]
        public static float AsFloat(string value, float defaultValue = 0.0f)
        {
            float result;
            return !float.TryParse(value, out result) ? defaultValue : result;
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Guid" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">The format of the value.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or invalid.</param>
        /// <returns>The converted value.</returns>
        public static Guid AsGuid(string value, string format = "N", Guid? defaultValue = null)
        {
            Guid result;
            return !Guid.TryParseExact(value, format, out result) ? defaultValue.GetValueOrDefault(Guid.Empty) : result;
        }

        /// <summary>
        ///     Converts a string to to a <see cref="T:System.int" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        public static int AsInt(string value, int defaultValue = 0)
        {
            int result;
            return !int.TryParse(value, out result) ? defaultValue : result;
        }

        /// <summary>
        ///     Converts a string to to a <see cref="T:System.int" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        public static short AsInt16(string value, short defaultValue = 0)
        {
            short result;
            return !short.TryParse(value, out result) ? defaultValue : result;
        }

        /// <summary>
        ///     Converts a string to to a <see cref="T:System.int" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        public static int AsInt32(string value, int defaultValue = 0)
        {
            int result;
            return !int.TryParse(value, out result) ? defaultValue : result;
        }

        /// <summary>
        ///     Converts a string to to a <see cref="T:System.int" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        public static long AsInt64(string value, long defaultValue = 0)
        {
            long result;
            return !long.TryParse(value, out result) ? defaultValue : result;
        }

        /// <summary>
        ///     Converts a string to to a <see cref="T:System.int" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        public static long AsLong(string value, long defaultValue = 0L)
        {
            long result;
            return !long.TryParse(value, out result) ? defaultValue : result;
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Single" /> number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if <paramref name="value" /> is null or is an invalid value.</param>
        /// <returns>The converted value.</returns>
        public static float AsSingle(string value, float defaultValue = 0.0f)
        {
            float result;
            return !float.TryParse(value, out result) ? defaultValue : result;
        }

        /// <summary>
        ///     Checks if the <paramref name="source" /> contains the <paramref name="input" /> based on the provided <paramref name="comparison" /> rules.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="input">The input.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns><c>true</c> if [contains] [the specified source]; otherwise, <c>false</c>.</returns>
        public static bool Contains(string source, string input, StringComparison comparison)
        {
            return source.IndexOf(input, comparison) >= 0;
        }

        /// <summary>
        ///     Checks if the <paramref name="source" /> contains the <paramref name="input" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="input">The input.</param>
        /// <returns><c>true</c> if [contains] [the specified source]; otherwise, <c>false</c>.</returns>
        public static bool Contains(string source, string input)
        {
            return source.IndexOf(input, StringComparison.Ordinal) >= 0;
        }

        /// <summary>
        ///     A nicer way of calling <see cref="System.String.Format(string, object[])" />
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string FormatWith(string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        ///     A nicer way of calling <see cref="System.String.Format(string, object[])" />
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="provider">String format provider</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
        public static string FormatWith(string format, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, format, args);
        }

        /// <summary>
        ///     Deserialize a json string to the instance of <typeparamref name="T" />. If the <paramref name="value" /> is null or empty, <c>null</c> will be returned.
        /// </summary>
        /// <typeparam name="T">The type of the instance to deserialize from the json string.</typeparam>
        /// <param name="value">The json string .</param>
        /// <returns>The deserialized instance of <typeparamref name="T" />. If the <paramref name="value" /> is null or empty, <c>null</c> will be returned.</returns>
        public static T FromJson<T>(string value)
        {
            return value.IsNullOrEmpty() ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        ///     Gets the substring of the first number chars.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        public static string GetFirst(string source, int count = 1)
        {
            return SubString(source, 0, count);
        }

        /// <summary>
        ///     Gets the substring of the last number chars.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        public static string GetLast(string source, int count = 1)
        {
            int start = source.Length - count;
            if (start < 0)
            {
                start = 0;
            }

            return SubString(source, start, count);
        }

        /// <summary>
        ///     Converts a string that has been HTML-encoded for HTTP transmission into a decoded string.
        /// </summary>
        /// <param name="value">The string to decode. </param>
        /// <returns>
        ///     A decoded string.
        /// </returns>
        public static string HtmlDecode(string value)
        {
            value = value ?? string.Empty;
            return HttpUtility.HtmlDecode(value);
        }

        /// <summary>
        ///     Converts a string to an HTML-encoded string.
        /// </summary>
        /// <param name="value">The string to encode. </param>
        /// <returns>
        ///     An encoded string.
        /// </returns>
        public static string HtmlEncode(string value)
        {
            value = value ?? string.Empty;
            return HttpUtility.HtmlEncode(value);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the specified data type.
        /// </summary>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false./// </returns>
        /// <param name="value">The value to test/ensure.</param>
        /// <typeparam name="TValue">The data type to convert to.</typeparam>
        public static bool Is<TValue>(string value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TValue));
            try
            {
                if (value != null)
                {
                    if (!converter.CanConvertFrom(null, value.GetType()))
                        goto label_5;
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                converter.ConvertFrom(null, CultureInfo.CurrentCulture, value);
                return true;
            }
            catch
            {
                // ignored
            }

            label_5:
            return false;
        }

        /// <summary>
        ///     Checks whether a string can be converted to the Boolean (true/false) type.
        /// </summary>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false./// </returns>
        /// <param name="value">The string value to test.</param>
        public static bool IsBool(string value)
        {
            return bool.TryParse(value, out bool _);
        }

        /// <summary>
        ///     Validates whether the provided
        ///     <paramref name="value">string</paramref>
        ///     is a valid cellphone number(13712341234).
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns><c>true</c> if the specified value is cellphone; otherwise, <c>false</c>.</returns>
        public static bool IsCellphone(string value)
        {
            return Match(value, RegexUtility.CellphoneRegex);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the <see cref="T:System.DateTime" /> type.
        /// </summary>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false./// </returns>
        /// <param name="value">The string value to test.</param>
        public static bool IsDateTime(string value)
        {
            return DateTime.TryParse(value, out DateTime _);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the <see cref="T:System.Decimal" /> type.
        /// </summary>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false./// </returns>
        /// <param name="value">The string value to test.</param>
        public static bool IsDecimal(string value)
        {
            return decimal.TryParse(value, out decimal _);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the Boolean (true/false) type.
        /// </summary>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false./// </returns>
        /// <param name="value">The string value to test.</param>
        public static bool IsDouble(string value)
        {
            return double.TryParse(value, out double _);
        }

        /// <summary>
        ///     Validates whether the provided
        ///     <paramref name="value">string</paramref>
        ///     is a valid Email Address.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns><c>true</c> if the specified value is email; otherwise, <c>false</c>.</returns>
        public static bool IsEmail(string value)
        {
            return Match(value, RegexUtility.EmailRegex);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the <see cref="T:System.Single" /> type.
        /// </summary>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false./// </returns>
        /// <param name="value">The string value to test.</param>
        public static bool IsFloat(string value)
        {
            return float.TryParse(value, out float _);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the Guid (true/false) type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <param name="format">The format of the value.</param>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false.</returns>
        public static bool IsGuid(string value, string format = "N")
        {
            return Guid.TryParseExact(value, format, out Guid _);
        }

        /// <summary>
        ///     Checks whether a string can be converted to the <see cref="T:System.int" /> type.
        /// </summary>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false./// </returns>
        /// <param name="value">The string value to test.</param>
        public static bool IsInt(string value)
        {
            return int.TryParse(value, out int _);
        }

        /// <summary>
        ///     Checks whether a string can be converted to to the <see cref="T:System.Int16" /> type.
        /// </summary>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false./// </returns>
        /// <param name="value">The string value to test.</param>
        public static bool IsInt16(string value)
        {
            return short.TryParse(value, out short _);
        }

        /// <summary>
        ///     Checks whether a string can be converted to to the <see cref="T:System.Int32" /> type.
        /// </summary>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false./// </returns>
        /// <param name="value">The string value to test.</param>
        public static bool IsInt32(string value)
        {
            return int.TryParse(value, out int _);
        }

        /// <summary>
        ///     Checks whether a string can be converted to to the <see cref="T:System.Int64" /> type.
        /// </summary>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false./// </returns>
        /// <param name="value">The string value to test.</param>
        public static bool IsInt64(string value)
        {
            return long.TryParse(value, out long _);
        }

        /// <summary>
        ///     Validates whether the provided
        ///     <paramref name="value">string</paramref>
        ///     is a valid IP Address.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [is ip address] [the specified value]; otherwise, <c>false</c>.</returns>
        public static bool IsIPAddress(string value)
        {
            return Match(value, RegexUtility.IPAddressRegex);
        }

        /// <summary>
        ///     Checks whether a string can be converted to a long.
        /// </summary>
        /// <returns>true if <paramref name="value" /> can be converted to the specified type; otherwise, false./// </returns>
        /// <param name="value">The string value to test.</param>
        public static bool IsLong(string value)
        {
            return long.TryParse(value, out long _);
        }

        /// <summary>
        ///     A nicer way of calling the inverse of <see cref="System.String.IsNullOrEmpty(string)" />
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is not null or an empty string (""); otherwise, false.</returns>
        public static bool IsNotNullOrEmpty(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        ///     A nicer way of calling the inverse of <see cref="System.String.IsNullOrWhiteSpace(string)" />
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is not null or an empty string (""); otherwise, false.</returns>
        public static bool IsNotNullOrWhiteSpace(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        ///     A nicer way of calling <see cref="System.String.IsNullOrEmpty(string)" />
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        ///     A nicer way of calling <see cref="System.String.IsNullOrWhiteSpace(string)" />
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrWhiteSpace(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        ///     Validates whether the provided
        ///     <paramref name="value">string</paramref>
        ///     is a valid (absolute) URL.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is URL; otherwise, <c>false</c>.</returns>
        public static bool IsUrl(string value) // absolute
        {
            return Match(value, RegexUtility.UrlRegex);
        }

        /// <summary>
        ///     Limits the length of the <paramref name="source" /> to the specified <paramref name="maxLength" />.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>System.String.</returns>
        public static string Limit(string source, int maxLength, string suffix = "")
        {
            if (suffix.IsNotNullOrEmpty())
            {
                maxLength = maxLength - suffix.Length;
            }

            return source.Length <= maxLength ? source : string.Concat(source.Substring(0, maxLength).Trim(), suffix ?? string.Empty);
        }

        /// <summary>
        ///     Matches the specified regex.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="regex">The regex.</param>
        /// <returns><c>true</c> if match, <c>false</c> otherwise.</returns>
        public static bool Match(string value, Regex regex)
        {
            return value.IsNotNullOrEmpty() || regex.IsMatch(value);
        }

        /// <summary>
        ///     Gets the string md5 hash.
        /// </summary>
        /// <param name="stringToHash">The string to hash.</param>
        /// <returns>System.String.</returns>
        public static string MD5Hash(string stringToHash)
        {
            return BaseLibrary.MD5Hash.ComputeMD5HashString(stringToHash);
        }

        /// <summary>
        ///     Removes the specified target string from value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="target">The target.</param>
        /// <returns>System.String.</returns>
        public static string Remove(string value, string target)
        {
            return value?.Replace(target, "");
        }

        /// <summary>
        ///     Separates a PascalCase string
        /// </summary>
        /// <example>
        ///     "ThisIsPascalCase".SeparatePascalCase(); // returns "This Is Pascal Case"
        /// </example>
        /// <param name="value">The value to split</param>
        /// <returns>The original string separated on each uppercase character.</returns>
        public static string SeparatePascalCase(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : Regex.Replace(value, "([A-Z])", " $1").Trim();
        }

        /// <summary>
        ///     Returns a string array containing the trimmed substrings in this <paramref name="value" />
        ///     that are delimited by the provided <paramref name="separators" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="separators">The separators.</param>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> SplitAndTrim(string value, params char[] separators)
        {
            value = value ?? string.Empty;
            return value.Trim().Split(separators, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
        }

        /// <summary>
        ///     SubString
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static string SubString(string source, int start, int count)
        {
            return source.Length - count - start < 0 ? source.Substring(start) : source.Substring(start, count);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.bool" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static bool ToBool(string value)
        {
            return bool.Parse(value);
        }

        /// <summary>
        ///     Converts a string to the CamelCase style.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static string ToCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            int len = value.Length;
            char[] newValue = new char[len];
            bool firstPart = true;

            for (int i = 0; i < len; ++i)
            {
                char c0 = value[i];
                char c1 = i < len - 1 ? value[i + 1] : 'A';
                bool c0IsUpper = c0 >= 'A' && c0 <= 'Z';
                bool c1IsUpper = c1 >= 'A' && c1 <= 'Z';

                if (firstPart && c0IsUpper && (c1IsUpper || i == 0))
                    c0 = (char)(c0 + LOWER_CASE_OFFSET);
                else
                    firstPart = false;

                newValue[i] = c0;
            }

            return new string(newValue);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.DateTime" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static DateTime ToDateTime(string value)
        {
            return DateTime.Parse(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.decimal" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static decimal ToDecimal(string value)
        {
            return decimal.Parse(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.double" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double ToDouble(string value)
        {
            return double.Parse(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.float" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float ToFloat(string value)
        {
            return float.Parse(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Guid" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">The format of the value.</param>
        /// <returns>The converted value.</returns>
        public static Guid ToGuid(string value, string format = "N")
        {
            return Guid.ParseExact(value, format);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.int" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static int ToInt(string value)
        {
            return int.Parse(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Int16" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static short ToInt16(string value)
        {
            return short.Parse(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Int32" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static int ToInt32(string value)
        {
            return int.Parse(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Int64" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static long ToInt64(string value)
        {
            return long.Parse(value);
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.long" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static long ToLong(string value)
        {
            return long.Parse(value);
        }

        /// <summary>
        ///     Hide some chars of the string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static string ToMosaicString(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            if (value.Length >= 12)
            {
                sb.Append(GetFirst(value, 4));
                (value.Length - 8).Times().Do(() => sb.Append("*"));
                sb.Append(GetLast(value, 4));
            }
            else
            {
                (value.Length - 4).Times().Do(() => sb.Append("*"));
                sb.Append(GetLast(value, 4));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Allows for using strings in null coalescing operations
        /// </summary>
        /// <param name="value">The string value to check</param>
        /// <returns>Null if <paramref name="value" /> is empty or the original value of <paramref name="value" />.</returns>
        public static string ToNullIfEmpty(string value)
        {
            return value.IsNullOrEmpty() ? null : value;
        }

        /// <summary>
        ///     Converts a string to a <see cref="T:System.Single" /> or throw an exception if the string is in a bad format.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float ToSingle(string value)
        {
            return float.Parse(value);
        }

        /// <summary>
        ///     Converts a string to the underscope style.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static string ToUnderScope(string value)
        {
            return string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()));
        }
    }
}