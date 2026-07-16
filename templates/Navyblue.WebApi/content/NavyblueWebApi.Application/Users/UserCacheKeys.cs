// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : UserCacheKeys.cs
// Created          : 2026-07-13  11:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="UserCacheKeys.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace NavyblueWebApi.Application.Users;

/// <summary>
///     Cache key helpers for user read models (Redis / <c>IDistributedCacheProvider</c>).
/// </summary>
public static class UserCacheKeys
{
    public static string ById(long userId) => $"user:id:{userId}";
}