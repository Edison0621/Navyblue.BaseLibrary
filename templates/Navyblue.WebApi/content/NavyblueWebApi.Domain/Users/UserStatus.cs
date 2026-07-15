// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : UserStatus.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="UserStatus.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace NavyblueWebApi.Domain.Users;

/// <summary>
///     User account status lifecycle.
/// </summary>
public enum UserStatus
{
    /// <summary>Account is active and can authenticate.</summary>
    Active = 1,

    /// <summary>Account is disabled and cannot authenticate.</summary>
    Inactive = 2
}