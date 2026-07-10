// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : SHA256.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="SHA256.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Navyblue.BaseLibrary;

/// <summary>
///     Class Sha256Utility.
/// </summary>
[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sha")]
public static class Sha256Utility
{
    /// <summary>
    ///     Hashes the specified payload.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <param name="salt">The salt.</param>
    /// <returns>System.String.</returns>
    public static string Hash(string payload, string salt)
    {
        string stringToHash = payload + salt;
        SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(stringToHash.GetBytesOfUTF8());
        StringBuilder hashString = new StringBuilder();
        foreach (byte b in hashBytes)
            hashString.Append(b.ToString("x2"));
        return hashString.ToString();
    }
}