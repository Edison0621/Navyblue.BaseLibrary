// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : MD5.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="MD5.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Cryptography;

namespace Navyblue.BaseLibrary;

/// <summary>
///     MD5Hash.
/// </summary>
public static class MD5Hash
{
    /// <summary>
    ///     Computes the file hash of the file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>Hash bytes</returns>
    public static byte[] ComputeHashForTheFile(string filePath)
    {
        using (MD5 md5 = MD5.Create())
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                return md5.ComputeHash(stream);
            }
        }
    }

    /// <summary>
    ///     Computes the file hash of the file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>Hash string</returns>
    public static string ComputeHashStringForTheFile(string filePath)
    {
        using (MD5 md5 = MD5.Create())
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                byte[] bytes = md5.ComputeHash(stream);
                return BitConverter.ToString(bytes).Remove("-").ToLowerInvariant();
            }
        }
    }

    /// <summary>
    ///     Computes the MD5 hash string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public static byte[] ComputeMD5Hash(string value)
    {
        if (value == null)
            return null;

        MD5 md5 = MD5.Create();
        return md5.ComputeHash(value.GetBytesOfUTF8());
    }

    /// <summary>
    ///     Computes the MD5 hash string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public static string ComputeMD5HashString(string value)
    {
        if (value == null)
            return "";

        MD5 md5 = MD5.Create();
        byte[] data = md5.ComputeHash(value.GetBytesOfUTF8());
        return data.Hex().ToLowerInvariant();
    }
}