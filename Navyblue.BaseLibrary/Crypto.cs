// *****************************************************************************************************************
// Project          : NavyBlue
// File             : Crypto.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:51
// *****************************************************************************************************************
// <copyright file="Crypto.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System.Diagnostics.CodeAnalysis;

namespace Navyblue.BaseLibrary
{
    /// <summary>
    ///     Class Crypto.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Crypto
    {
        /// <summary>
        ///     Gets the encrypted string.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>System.String.</returns>
        public static string PBKDF2(string payload, string salt)
        {
            return PBKDF2Utility.Hash(payload, salt);
        }

        /// <summary>
        ///     Gets the encrypted string.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>System.String.</returns>
        public static string Sha256(string payload, string salt)
        {
            return Sha256Utility.Hash(payload, salt);
        }
    }
}