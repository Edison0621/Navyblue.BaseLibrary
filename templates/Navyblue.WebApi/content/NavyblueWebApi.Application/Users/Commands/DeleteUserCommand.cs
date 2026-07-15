// ****************************************************************************************************************************************
// Project          : NavyblueWebApi
// File             : DeleteUserCommand.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-15  14:44
// ****************************************************************************************************************************************
// <copyright file="DeleteUserCommand.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Cqrs;

namespace NavyblueWebApi.Application.Users.Commands;

/// <summary>
///     Deletes a user permanently.
/// </summary>
public sealed class DeleteUserCommand : Command<IdCommandResult>
{
    public DeleteUserCommand(long userId)
    {
        this.UserId = userId;
    }

    public long UserId { get; }

    public override string DisplayName => "DeleteUser";

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