using NavyblueWebApi.Application.Users.Commands;
using NavyblueWebApi.Application.Users.Queries;
using NavyblueWebApi.Model.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.AspNetCore;
using Navyblue.Foundation.Cqrs;
using Navyblue.Foundation.Data;

namespace NavyblueWebApi.Web.Controllers;

/// <summary>
///     User management endpoints (requires JWT). Commands/queries flow through CQRS buses.
/// </summary>
[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController(ICommandBus commandBus, IQueryService queryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PageData<UserModel>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResult<PageData<UserModel>>>> List(
        [FromQuery] string? keyword,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        PageData<UserModel> users = await queryService.Query(new ListUsersQuery(keyword, pageIndex, pageSize));
        return this.OkApi(users);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResult<UserModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<UserModel>>> Get(long id)
    {
        UserModel? user = await queryService.Query(new GetUserQuery(id));
        if (user is null)
            return this.NotFound(ApiResult<UserModel>.Fail(BusinessCode.NotFound, $"User '{id}' was not found.", this.HttpContext.GetTraceId()));
        return this.OkApi(user);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResult<string>>> Create([FromBody] CreateUserRequest request)
    {
        IdCommandResult result = await commandBus.Send(new AddUserCommand(request.Name, request.Email, request.Password));
        return this.CreatedAtAction(nameof(Get), new { id = long.Parse(result.Id) },
            ApiResult<string>.Success(result.Id, "User created.", this.HttpContext.GetTraceId()));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResult<string>>> Update(long id, [FromBody] UpdateUserRequest request)
    {
        IdCommandResult result = await commandBus.Send(new UpdateUserCommand(id, request.Name, request.Email));
        return this.OkApi(result.Id, "User updated.");
    }

    [HttpPut("{id:long}/inactivate")]
    public async Task<ActionResult<ApiResult<string>>> Inactivate(long id)
    {
        IdCommandResult result = await commandBus.Send(new InactivateUserCommand(id));
        return this.OkApi(result.Id, "User inactivated.");
    }

    [HttpPut("{id:long}/activate")]
    public async Task<ActionResult<ApiResult<string>>> Activate(long id)
    {
        IdCommandResult result = await commandBus.Send(new ActivateUserCommand(id));
        return this.OkApi(result.Id, "User activated.");
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResult<string>>> Delete(long id)
    {
        IdCommandResult result = await commandBus.Send(new DeleteUserCommand(id));
        return this.OkApi(result.Id, "User deleted.");
    }
}

public sealed record CreateUserRequest(string Name, string Email, string? Password = null);

public sealed record UpdateUserRequest(string? Name, string? Email);
