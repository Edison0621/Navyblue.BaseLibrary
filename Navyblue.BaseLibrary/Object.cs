// *****************************************************************************************************************
// Project          : NavyBlue
// File             : Object.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:52
// *****************************************************************************************************************
// <copyright file="Object.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;

namespace Navyblue.BaseLibrary
{
    /// <summary>
    ///     Extensions of <see cref="object" />.
    /// </summary>
    public static class ObjectExtensions
    {
        public static TResult To<TObject, TResult>(this TObject value, Func<TObject, TResult> converter)
        {
            return converter.Invoke(value);
        }
    }
}