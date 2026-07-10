// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ResponseHeaderExtensions.cs
// Created          : 2026-07-09  13:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="ResponseHeaderExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
/// </summary>
public static class ResponseHeaderExtensions
{
    /// <summary>
    ///     Sets the no store.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static HttpResponse SetNoStore(this HttpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);
        response.Headers.CacheControl = "no-store, no-cache, max-age=0";
        response.Headers.Pragma = "no-cache";
        response.Headers.Expires = "0";
        return response;
    }

    /// <summary>
    ///     Sets the cache control.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="maxAge">The maximum age.</param>
    /// <param name="isPublic">if set to <c>true</c> [is public].</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static HttpResponse SetCacheControl(this HttpResponse response, TimeSpan maxAge, bool isPublic = true)
    {
        ArgumentNullException.ThrowIfNull(response);
        response.Headers.CacheControl = $"{(isPublic ? "public" : "private")}, max-age={(int)maxAge.TotalSeconds}";
        return response;
    }

    /// <summary>
    ///     Sets the e tag.
    /// </summary>
    /// <param name="response">The response.</param>
    /// <param name="etag">The etag.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static HttpResponse SetETag(this HttpResponse response, string etag)
    {
        ArgumentNullException.ThrowIfNull(response);
        Guard.NotNullOrWhiteSpace(etag, nameof(etag));
        response.Headers.ETag = etag.StartsWith('"') ? etag : $"\"{etag}\"";
        return response;
    }

    /// <summary>
    ///     Computes the weak e tag.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static string ComputeWeakETag(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return $"W/\"{Convert.ToHexString(hash)[..16]}\"";
    }
}