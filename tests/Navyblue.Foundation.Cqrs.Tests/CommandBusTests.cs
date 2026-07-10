using Microsoft.Extensions.DependencyInjection;
using Navyblue.Foundation.Cqrs;
using Xunit;

namespace Navyblue.Foundation.Cqrs.Tests;

public sealed class PingCommand : Command<IdCommandResult>
{
    private readonly string _id;

    public PingCommand(string? id = null) => this._id = id ?? Guid.NewGuid().ToString("N");

    public override string DisplayName => "Ping";
    public override string Id => this._id;
    public string Message { get; init; } = "ping";

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

public sealed class PingCommandHandler : CommandHandler<PingCommand, IdCommandResult>
{
    protected override Task<IdCommandResult> ProcessRequest(PingCommand request)
        => Task.FromResult(new IdCommandResult(request.Id, request.Message));
}

public sealed class GetMessageQuery : Query<string>
{
    public override string DisplayName => "GetMessage";
    public override string Id { get; } = Guid.NewGuid().ToString("N");
    public string Value { get; init; } = "hello";

    public override bool Validate(out string validationErrorMessage)
    {
        validationErrorMessage = string.Empty;
        return true;
    }
}

public sealed class GetMessageQueryHandler : QueryHandler<GetMessageQuery, string>
{
    protected override Task<string> ProcessRequest(GetMessageQuery request)
        => Task.FromResult(request.Value);
}

public class CommandBusTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddNavyblueCqrs(typeof(PingCommand).Assembly);
        services.AddSingleton<ICqrsUnitOfWork, NoOpUnitOfWork>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task CommandBus_Send_ReturnsHandlerResult()
    {
        await using ServiceProvider sp = BuildProvider();
        ICommandBus bus = sp.GetRequiredService<ICommandBus>();
        IdCommandResult result = await bus.Send(new PingCommand { Message = "ok" });
        Assert.True(result.IsSuccesfull);
        Assert.Equal("ok", result.Message);
    }

    [Fact]
    public async Task QueryService_Query_ReturnsHandlerResult()
    {
        await using ServiceProvider sp = BuildProvider();
        IQueryService queries = sp.GetRequiredService<IQueryService>();
        string result = await queries.Query(new GetMessageQuery { Value = "world" });
        Assert.Equal("world", result);
    }

    [Fact]
    public async Task Inbox_SkipsDuplicateRequest()
    {
        await using ServiceProvider sp = BuildProvider();
        ICommandBus bus = sp.GetRequiredService<ICommandBus>();
        var command = new PingCommand("fixed-id-1") { Message = "once" };

        IdCommandResult first = await bus.Send(command);
        IdCommandResult second = await bus.Send(command);
        Assert.Equal(first.Id, second.Id);
    }

    private sealed class NoOpUnitOfWork : ICqrsUnitOfWork
    {
        public Task BeginAsync() => Task.CompletedTask;
        public Task CommitAsync() => Task.CompletedTask;
        public Task RollbackAsync() => Task.CompletedTask;
    }
}
