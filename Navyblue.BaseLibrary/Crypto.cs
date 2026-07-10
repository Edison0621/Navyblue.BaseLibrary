// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Crypto.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="Crypto.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics.CodeAnalysis;

namespace Navyblue.BaseLibrary;

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