using Navyblue.Samples.Application.Authentication.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.AspNetCore;
using Navyblue.Foundation.Cqrs;

namespace Navyblue.Samples.Web.Controllers;

/// <summary>
///     Authentication endpoint. Authenticates credentials and returns a JWT.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public sealed class AuthController(ICommandBus commandBus) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResult<AuthCommandResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<AuthCommandResult>>> Login([FromBody] LoginRequest request)
    {
        AuthCommandResult result = await commandBus.Send(new AuthCommand(request.Login, request.Password));
        return this.OkApi(result);
    }
}

public sealed record LoginRequest(string Login, string Password);
