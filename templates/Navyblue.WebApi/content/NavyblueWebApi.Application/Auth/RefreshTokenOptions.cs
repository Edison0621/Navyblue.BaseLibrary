// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : RefreshTokenOptions.cs
// Created          : 2026-07-13  11:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="RefreshTokenOptions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace NavyblueWebApi.Application.Authentication;

/// <summary>
///     Refresh-token lifetime options (bound from <c>RefreshToken</c> configuration section).
/// </summary>
public sealed class RefreshTokenOptions
{
    public const string SECTION_NAME = "RefreshToken";

    /// <summary>How long a refresh token remains valid. Default: 7 days.</summary>
    public TimeSpan Expire { get; set; } = TimeSpan.FromDays(7);
}