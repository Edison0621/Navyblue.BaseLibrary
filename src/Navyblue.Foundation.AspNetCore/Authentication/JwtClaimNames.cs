// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : JwtClaimNames.cs
// Created          : 2026-07-09  15:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="JwtClaimNames.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.AspNetCore;

/// <summary>
///     Recommended claim type names for Navyblue JWTs.
///     Callers may use any custom claim types; these names align with <c>ICurrentUser</c> / <c>ICurrentTenant</c> resolution.
/// </summary>
public static class JwtClaimNames
{
    /// <summary>
    ///     Tenant identifier claim. Resolved by <c>ICurrentTenant</c> / <c>HttpCurrentTenant</c>.
    /// </summary>
    public const string TenantId = "tenant_id";

    /// <summary>
    ///     Merchant identifier claim. Read via <c>ICurrentUser.FindClaimValue(JwtClaimNames.MerchantId)</c>.
    /// </summary>
    public const string MerchantId = "merchant_id";

    /// <summary>
    ///     Alternate user id claim recognized by <c>ICurrentUser</c>.
    /// </summary>
    public const string UserId = "user_id";
}