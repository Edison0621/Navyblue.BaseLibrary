// *****************************************************************************************************************
// Project          : NavyBlue
// File             : Path.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:52
// *****************************************************************************************************************
// <copyright file="Path.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;

namespace Navyblue.BaseLibrary
{
    /// <summary>
    ///     Utilities methods for working with resource paths
    /// </summary>
    public static class UrlPathUtility
    {
        /// <summary>
        ///     Combines two URL paths
        /// </summary>
        public static Uri CombineUrlPaths(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path2))
                return new Uri(path1);

            if (string.IsNullOrEmpty(path1))
                return new Uri(path2);

            if (path2.StartsWith("http://", StringComparison.Ordinal) || path2.StartsWith("https://", StringComparison.Ordinal))
                return new Uri(path2);

            char ch = path1[path1.Length - 1];

            return ch != '/' ? new Uri(path1.TrimEnd('/') + '/' + path2.TrimStart('/')) : new Uri(path1 + path2);
        }
    }
}