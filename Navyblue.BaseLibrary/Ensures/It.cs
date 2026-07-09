// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : It.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:00
// ****************************************************************************************************************************************
// <copyright file="It.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace NavyBlue.AspNetCore.Lib;

/// <summary>
///     Use <see cref="It" /> class to construct <see cref="Ensures{T}" /> instance.
/// </summary>
public static class It
{
    /// <summary>
    ///     Construct a <see cref="Ensures{T}" /> instance for the value.
    /// </summary>
    /// <typeparam name="T">Type of the value to test/ensure.</typeparam>
    /// <param name="value">The value to test/ensure of the <see cref="Ensures{T}" /> instance.</param>
    /// <returns>The specified <see cref="Ensures{T}" /> instance.</returns>
    public static Ensures<T> Ensures<T>(T value)
    {
        return new Ensures<T>(value);
    }

    /// <summary>
    ///     Construct a <see cref="Ensures{T}" /> instance for an object.
    /// </summary>
    /// <returns>The specified <see cref="Ensures{T}" /> instance for an object.</returns>
    public static Ensures<object> Ensures()
    {
        return new Ensures<object>(new object());
    }
}