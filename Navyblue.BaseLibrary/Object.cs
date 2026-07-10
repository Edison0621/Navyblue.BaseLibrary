// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Object.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="Object.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary;

/// <summary>
///     Extensions of <see cref="object" />.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    ///     To the specified converter.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="converter">The converter.</param>
    /// <returns></returns>
    public static TResult To<TObject, TResult>(this TObject value, Func<TObject, TResult> converter)
    {
        return converter.Invoke(value);
    }
}