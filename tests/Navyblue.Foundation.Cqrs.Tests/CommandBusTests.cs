// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : CommandBusTests.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="CommandBusTests.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Navyblue.Foundation.Cqrs.Tests;

/// <summary>
///     The ping command.
/// </summary>
public sealed class PingCommand : Command<IdCommandResult>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PingCommand" /> class.
    /// </summary>
    /// <param name="id">The id.</param>
    public PingCommand(string? id = null) => this.Id = id ?? Guid.NewGuid().ToString("N");

    /// <summary>
    ///     Gets the display name.
    /// </summary>
    public override string DisplayName => "Ping";

    /// <summary>
    ///     Gets the id.
    /// </summary>
    public override string Id { get; }

    /// <summary>
    ///     Gets the message.
    /// </summary>
    public string Message { get; init; } = "ping";

    /// <summary>
    /// </summary>
    /// <param name="validationErrorMessage">The validation error message.</param>
    /// <returns>A bool</returns>
    public override bool Validate(out string validationErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(this.Message))
        {
            validationErrorMessage = "Message is required.";
            return false;
        }

        validationErrorMessage = string.Empty;
        return true;
    }
}

/// <summary>
///     The ping command handler.
/// </summary>
public sealed class PingCommandHandler : CommandHandler<PingCommand, IdCommandResult>
{
    protected override Task<IdCommandResult> ProcessRequest(PingCommand request)
        => Task.FromResult(new IdCommandResult(request.Id, request.Message));
}

/// <summary>
///     The get message query.
/// </summary>
public sealed class GetMessageQuery : Query<string>
{
    /// <summary>
    ///     Gets the display name.
    /// </summary>
    public override string DisplayName => "GetMessage";

    /// <summary>
    ///     Gets the id.
    /// </summary>
    public override string Id { get; } = Guid.NewGuid().ToString("N");

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public string Value { get; init; } = "hello";

    /// <summary>
    /// </summary>
    /// <param name="validationErrorMessage">The validation error message.</param>
    /// <returns>A bool</returns>
    public override bool Validate(out string validationErrorMessage)
    {
        validationErrorMessage = string.Empty;
        return true;
    }
}

/// <summary>
///     The get message query handler.
/// </summary>
public sealed class GetMessageQueryHandler : QueryHandler<GetMessageQuery, string>
{
    protected override Task<string> ProcessRequest(GetMessageQuery request)
        => Task.FromResult(request.Value);
}

/// <summary>
///     The command bus tests.
/// </summary>
public class CommandBusTests
{
    private static ServiceProvider BuildProvider()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddNavyblueCqrs(typeof(PingCommand).Assembly);
        services.AddSingleton<ICqrsUnitOfWork, NoOpUnitOfWork>();
        return services.BuildServiceProvider();
    }

    /// <summary>
    ///     Command bus send returns handler result.
    /// </summary>
    /// <returns>A Task</returns>
    [Fact]
    public async Task CommandBus_Send_ReturnsHandlerResult()
    {
        await using ServiceProvider sp = BuildProvider();
        ICommandBus bus = sp.GetRequiredService<ICommandBus>();
        IdCommandResult result = await bus.Send(new PingCommand { Message = "ok" });
        Assert.True(result.IsSuccesfull);
        Assert.Equal("ok", result.Message);
    }

    /// <summary>
    ///     Query service query returns handler result.
    /// </summary>
    /// <returns>A Task</returns>
    [Fact]
    public async Task QueryService_Query_ReturnsHandlerResult()
    {
        await using ServiceProvider sp = BuildProvider();
        IQueryService queries = sp.GetRequiredService<IQueryService>();
        string result = await queries.Query(new GetMessageQuery { Value = "world" });
        Assert.Equal("world", result);
    }

    /// <summary>
    ///     Inbox skips duplicate request.
    /// </summary>
    /// <returns>A Task</returns>
    [Fact]
    public async Task Inbox_SkipsDuplicateRequest()
    {
        await using ServiceProvider sp = BuildProvider();
        ICommandBus bus = sp.GetRequiredService<ICommandBus>();
        PingCommand command = new PingCommand("fixed-id-1") { Message = "once" };

        IdCommandResult first = await bus.Send(command);
        IdCommandResult second = await bus.Send(command);
        Assert.Equal(first.Id, second.Id);
    }

    #region Nested type: NoOpUnitOfWork

    private sealed class NoOpUnitOfWork : ICqrsUnitOfWork
    {
        #region ICqrsUnitOfWork Members

        public Task BeginAsync() => Task.CompletedTask;
        public Task CommitAsync() => Task.CompletedTask;
        public Task RollbackAsync() => Task.CompletedTask;

        #endregion
    }

    #endregion
}