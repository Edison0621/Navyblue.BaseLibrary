// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : EnsuresExtensions.Compare.Boolean.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:00
// ****************************************************************************************************************************************
// <copyright file="EnsuresExtensions.Compare.Boolean.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics.CodeAnalysis;

namespace NavyBlue.AspNetCore.Lib;

/// <summary>
///     Extensions of Ensures utility for the <see cref="System.Boolean" />.
/// </summary>
public static partial class EnsuresExtensions
{
    /// <summary>
    ///     Checks whether the given value is <b>false</b>.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<bool> IsFalse(this Ensures<bool> ensures)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => !v);
    }

    /// <summary>
    ///     Checks whether the given value is <b>false</b>.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Ensures<bool?> IsFalse(this Ensures<bool?> ensures)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v.HasValue && !v.Value);
    }

    /// <summary>
    ///     Checks whether the given value is <b>true</b>.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    public static Ensures<bool> IsTrue(this Ensures<bool> ensures)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v);
    }

    /// <summary>
    ///     Checks whether the given value is <b>true</b>.
    /// </summary>
    /// <param name="ensures">The <see cref="Ensures{T}" /> that holds the value that has to be test/ensure.</param>
    /// <returns>The specified <paramref name="ensures" /> instance.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static Ensures<bool?> IsTrue(this Ensures<bool?> ensures)
    {
        if (ensures == null)
        {
            throw new ArgumentNullException(nameof(ensures));
        }

        return ensures.That(v => v.HasValue && v.Value);
    }
}