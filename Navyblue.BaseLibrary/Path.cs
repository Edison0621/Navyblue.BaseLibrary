// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Path.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="Path.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary;

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