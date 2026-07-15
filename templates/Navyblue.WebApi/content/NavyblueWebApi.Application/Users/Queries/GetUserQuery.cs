// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : GetUserQuery.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="GetUserQuery.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Cqrs;
using NavyblueWebApi.Model.Users;

namespace NavyblueWebApi.Application.Users.Queries;

/// <summary>
///     Returns a single user by id, or null when not found.
/// </summary>
public sealed class GetUserQuery : Query<UserModel?>
{
    public GetUserQuery(long userId)
    {
        this.UserId = userId;
    }

    public long UserId { get; }

    public override string DisplayName => "GetUser";

    public override string Id { get; } = Guid.NewGuid().ToString("N");

    public override bool Validate(out string validationErrorMessage)
    {
        if (this.UserId <= 0)
        {
            validationErrorMessage = "A positive user Id is required.";
            return false;
        }

        validationErrorMessage = string.Empty;
        return true;
    }
}