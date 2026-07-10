# Capability Migration Checklist

Source projects → Navyblue.BaseLibrary targets.

| Source | Type / Capability | Target | Status |
|--------|-------------------|--------|--------|
| CQRS.Mediatr.Lite | Command/Query/Event buses, handlers, pipeline | `Navyblue.Foundation.Cqrs` | Done |
| CQRS.Mediatr.Lite | Inbox/Outbox (InMemory + File) | `Navyblue.Foundation.Cqrs` | Done |
| CQRS.Mediatr.Lite | `IEventBus` | Renamed `IDomainEventBus` | Done |
| CQRS.Mediatr.Lite | `IUnitOfWork` | Renamed `ICqrsUnitOfWork` | Done |
| CQRS.Mediatr.Lite | AggregateRoot / ValueObject | Use `Navyblue.Foundation.Domain` | Done |
| CQRS.Mediatr.Lite | Enumeration | `Navyblue.Foundation.Cqrs.Domain.Enumeration` | Done |
| CorrelationId | Middleware / Options / Providers / HttpClient | `Navyblue.Foundation.AspNetCore` Correlation | Done |
| DotNetCore.Results | Bridge to ApiResult | `HttpStatusResultBridge` | Done |
| DotNetCore.Objects.Grid | GridParameters / GridResult | `Foundation.Application` | Done |
| DotNetCore.Security | IHashService / HashService | `Foundation.Security` | Done |
| DotNetCore.IoC | AddClassesMatchingInterfaces | `ConventionalServiceRegistration` | Done |
| DotNetCore.Domain | Entity | Use Foundation Domain (not duplicated) | Done |
| DotNetCore.Mediator | Replaced by Cqrs buses | `Navyblue.Foundation.Cqrs` | Done |
| Original Architecture app | End-to-end sample | `samples/Navyblue.Samples` | Done |
| — | Web API template (`navyblue-webapi`) | `templates/Navyblue.WebApi` | Done |
| DotNetCore.EF/Mongo/RabbitMQ/Serilog/Mapping | Heavy adapters | See [deferred-adapters.md](deferred-adapters.md) | Deferred |

## Acceptance

- [x] `Navyblue.Foundation.Cqrs` builds (net10.0)
- [x] CQRS unit tests pass (CommandBus / QueryService / Inbox)
- [x] AspNetCore CorrelationId unified with TraceId / CorrelationContext
- [x] Navyblue.Samples builds and wires Cqrs + ApiResult + JWT + in-memory repos
- [x] Deferred adapters documented
