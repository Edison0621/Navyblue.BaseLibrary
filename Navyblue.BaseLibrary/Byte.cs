// *****************************************************************************************************************
// Project          : NavyBlue
// File             : Byte.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:51
// *****************************************************************************************************************
// <copyright file="Byte.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Text;

namespace Navyblue.BaseLibrary
{
    /// <summary>
    ///     Extensions of <see cref="System.Byte" /> types.
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        ///     Gets ASCII string of specified byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string ASCII(this byte value)
        {
            return ByteUtility.ASCII(value);
        }

        /// <summary>
        ///     Gets ASCII string of specified byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string ASCII(this byte[] value)
        {
            return ByteUtility.ASCII(value);
        }

        /// <summary>
        ///     Gets the value of ASCII string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBytesOfASCII(this string value)
        {
            return ByteUtility.GetBytesOfASCII(value);
        }

        /// <summary>
        ///     Gets the value of unicode string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBytesOfUnicode(this string value)
        {
            return ByteUtility.GetBytesOfUnicode(value);
        }

        /// <summary>
        ///     Gets the value of Utf8 string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBytesOfUTF8(this string value)
        {
            return ByteUtility.GetBytesOfUTF8(value);
        }

        /// <summary>
        ///     Hexadecimals the specified byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Hex(this byte value)
        {
            return ByteUtility.Hex(value);
        }

        /// <summary>
        ///     Hexadecimals the specified byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Hex(this byte[] value)
        {
            return ByteUtility.Hex(value);
        }

        /// <summary>
        ///     Gets Unicode string of specified byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Unicode(this byte[] value)
        {
            return ByteUtility.Unicode(value);
        }

        /// <summary>
        ///     Gets Utf8 string of specified byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Utf8(this byte[] value)
        {
            return ByteUtility.Utf8(value);
        }
    }

    /// <summary>
    ///     Utilities for working with <see cref="System.Byte" /> type.
    /// </summary>
    public static class ByteUtility
    {
        /// <summary>
        ///     Gets ASCII string of specified byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string ASCII(byte value)
        {
            return Encoding.ASCII.GetString(new[] { value });
        }

        /// <summary>
        ///     Gets ASCII string of specified byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string ASCII(byte[] value)
        {
            if (value == null)
                return null;
            value = value.FixBom();
            return Encoding.ASCII.GetString(value);
        }

        /// <summary>
        ///     Gets the value of ASCII string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBytesOfASCII(string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }

        /// <summary>
        ///     Gets the value of unicode string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBytesOfUnicode(string value)
        {
            return Encoding.Unicode.GetBytes(value);
        }

        /// <summary>
        ///     Gets the value of Utf8 string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetBytesOfUTF8(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        ///     Hexadecimals the specified byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Hex(byte value)
        {
            return value.ToString("x2").ToUpperInvariant();
        }

        /// <summary>
        ///     Hexadecimals the specified byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Hex(byte[] value)
        {
            if (value == null)
                return "";

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
                stringBuilder.Append(value[i].Hex());

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Gets Unicode string of specified byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Unicode(byte[] value)
        {
            return value == null ? null : Encoding.Unicode.GetString(value);
        }

        /// <summary>
        ///     Gets Utf8 string of specified byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string Utf8(byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }

        /// <summary>
        ///     Fixes the bom.
        /// </summary>
        /// <param name="valueToFix">The value to fix.</param>
        /// <returns>System.Byte[].</returns>
        private static byte[] FixBom(this byte[] valueToFix)
        {
            //see BOM - Byte Order Mark : http://en.wikipedia.org/wiki/Byte_order_mark
            //    http://www.verious.com/qa/-239-187-191-characters-appended-to-the-beginning-of-each-file/
            //    http://social.msdn.microsoft.com/Forums/en-US/8956758d-9814-4bd4-9812-e82903640b2f/recieving-239187191-character-symbols-when-loading-text-files-not-containing-them
            if (valueToFix != null && valueToFix.Length > 3 && (valueToFix[0] == '\xEF' && valueToFix[1] == '\xBB' && valueToFix[2] == '\xBF'))
                return valueToFix.Removevalue(2);

            return valueToFix;
        }

        /// <summary>
        ///     Removes the value.
        /// </summary>
        /// <param name="originalvalue">The original value.</param>
        /// <param name="removeFrom">The remove from.</param>
        /// <returns>System.Byte[].</returns>
        private static byte[] Removevalue(this byte[] originalvalue, uint removeFrom)
        {
            if (originalvalue.Length > removeFrom)
            {
                long newSize = originalvalue.Length - removeFrom - 1;
                byte[] value = new byte[newSize];
                Array.Copy(originalvalue, removeFrom + 1, value, 0, newSize);
                return value;
            }

            return new byte[0];
        }
    }
}