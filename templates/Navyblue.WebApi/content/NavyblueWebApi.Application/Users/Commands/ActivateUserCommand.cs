// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : ActivateUserCommand.cs
// Created          : 2026-07-10  18:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="ActivateUserCommand.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Cqrs;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Reactivates an inactive user.
/// </summary>
public sealed class ActivateUserCommand : Command<IdCommandResult>
{
    public ActivateUserCommand(long userId) => this.UserId = userId;

    public long UserId { get; }
    public override string DisplayName => "ActivateUser";
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