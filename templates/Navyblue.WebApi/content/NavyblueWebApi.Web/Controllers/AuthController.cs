using NavyblueWebApi.Application.Authentication.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.AspNetCore;
using Navyblue.Foundation.Cqrs;

namespace NavyblueWebApi.Web.Controllers;

/// <summary>
///     Authentication endpoints: login, refresh, logout.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public sealed class AuthController(ICommandBus commandBus) : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ApiResult<AuthCommandResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<AuthCommandResult>>> Login([FromBody] LoginRequest request)
    {
        AuthCommandResult result = await commandBus.Send(new AuthCommand(request.Login, request.Password));
        return this.OkApi(result);
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ApiResult<AuthCommandResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<AuthCommandResult>>> Refresh([FromBody] RefreshRequest request)
    {
        AuthCommandResult result = await commandBus.Send(new RefreshTokenCommand(request.RefreshToken));
        return this.OkApi(result);
    }

    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<string>>> Logout([FromBody] RefreshRequest request)
    {
        IdCommandResult result = await commandBus.Send(new RevokeRefreshTokenCommand(request.RefreshToken));
        return this.OkApi(result.Id, "Logged out.");
    }
}

public sealed record LoginRequest(string Login, string Password);

public sealed record RefreshRequest(string RefreshToken);
