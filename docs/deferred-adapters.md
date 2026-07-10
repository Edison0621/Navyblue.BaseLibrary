# Deferred Adapters

Heavy integrations intentionally **not** implemented in this migration round.
Core packages keep abstractions only; adapters will be separate packages when needed.

| Planned package | Source inspiration | Depends on | Suggested contracts already in Foundation |
| --- | --- | --- | --- |
| `Navyblue.Foundation.EntityFrameworkCore` | DotNetCore.EntityFrameworkCore | EF Core | `IRepository<T>`, `IUnitOfWork`, `ISpecification<T>` |
| `Navyblue.Foundation.MongoDB` | DotNetCore.MongoDB | MongoDB.Driver | `IRepository<T>` |
| `Navyblue.Foundation.RabbitMQ` | DotNetCore.RabbitMQ | RabbitMQ.Client | `IEventBus`, `IMessagePublisher`, `IOutboxStore` |
| `Navyblue.Foundation.Redis` | (planned) | StackExchange.Redis | `IDistributedCacheProvider`, `IDistributedLockProvider`, `IIdempotencyStore` |
| `Navyblue.Foundation.Serilog` | DotNetCore.Logging | Serilog | Hosting / logging bootstrap only |
| `Navyblue.Foundation.Mapping` | DotNetCore.Mapping | Mapper of choice | `IObjectMapper` |
| `Navyblue.BaseLibrary.OpenTelemetry` | (planned) | OpenTelemetry | TraceId / CorrelationContext enrichment |
| `Navyblue.BaseLibrary.Excel` | (planned) | ClosedXML / EPPlus | File utilities |

## Split criteria

Introduce an adapter package when the capability would:

1. Pull a heavy SDK or database driver into Core, or
2. Force non-Web / non-data projects to take unwanted dependencies, or
3. Materially increase package size / restore time.

## Interim guidance

- Use `Navyblue.Foundation.Testing` in-memory implementations for local/dev samples (see `samples/Navyblue.Samples`).
- Keep Serilog / AutoMapper / EF as **application-level** package references until the adapter packages exist.
- Prefer Foundation abstractions (`IRepository`, `IOutboxStore`, `IEventBus`) at Application boundaries so swapping adapters later does not rewrite handlers.
