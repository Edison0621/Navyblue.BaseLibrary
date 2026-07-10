// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Guid.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="Guid.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary;

/// <summary>
///     Utilities for working with <see cref="System.Guid" /> type.
/// </summary>
public static class GuidUtility
{
    private const long EPOCH_MILLISECONDS = 62135596800000;

    /// <summary>
    ///     Generates a 16 character, Guid based string with very little chance of collision.
    ///     <example>3c4ebc5f5f2c4edc</example>
    ///     .
    /// </summary>
    /// <remarks>Author Mads Kristensen http://madskristensen.net/post/Generate-unique-strings-and-numbers-in-C.aspx</remarks>
    public static string GuidShortCode()
    {
        long i = Guid.NewGuid().ToByteArray().Aggregate<byte, long>(1, (current, b) => current * (b + 1));
        return $"{i - DateTime.Now.Ticks:x}";
    }

    /// <summary>
    ///     Creates a sequential GUID according to SQL Server's ordering rules.
    /// </summary>
    public static Guid NewSequentialGuid()
    {
        // This code was not reviewed to guarantee uniqueness under most conditions, nor
        // completely optimize for avoiding page splits in SQL Server when doing inserts from
        // multiple hosts, so do not re-use in production systems.
        byte[] guidBytes = Guid.NewGuid().ToByteArray();

        // get the milliseconds since Jan 1 1970
        byte[] sequential = BitConverter.GetBytes((DateTime.Now.Ticks / 10000L) - EPOCH_MILLISECONDS);

        // discard the 2 most significant bytes, as we only care about the milliseconds
        // increasing, but the highest ones should be 0 for several thousand years to come (non-issue).
        if (BitConverter.IsLittleEndian)
        {
            guidBytes[10] = sequential[5];
            guidBytes[11] = sequential[4];
            guidBytes[12] = sequential[3];
            guidBytes[13] = sequential[2];
            guidBytes[14] = sequential[1];
            guidBytes[15] = sequential[0];
        }
        else
        {
            Buffer.BlockCopy(sequential, 2, guidBytes, 10, 6);
        }

        return new Guid(guidBytes);
    }

    /// <summary>
    ///     To the unique identifier string.
    /// </summary>
    /// <param name="value">The unique identifier.</param>
    /// <returns>System.String.</returns>
    public static string ToGuidString(this Guid value)
    {
        return value.ToString("N").ToUpperInvariant();
    }
}