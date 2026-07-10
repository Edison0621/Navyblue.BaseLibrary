# Navyblue.BaseLibrary vNext Capability Map

The vNext foundation is organized around common software development workflows while preserving the existing `Navyblue.BaseLibrary` project unchanged.

## Workflow Coverage

| Workflow stage | vNext package | Scope |
| --- | --- | --- |
| API input and domain modeling | `Navyblue.Foundation.Primitives` | Guard, Result, Error, SequentialGuid (paging: Application.PageResult) |
| Everyday coding | `Navyblue.BaseLibrary.Extensions` | String, enumerable, number, date/time, GUID helpers |
| Data exchange | `Navyblue.Foundation.Serialization` | System.Text.Json helpers with AOT-friendly overloads |
| Security baseline | `Navyblue.Foundation.Security` | Hashing, HMAC, PBKDF2, Hex, Base64Url, `IHashService` |
| Input validation | `Navyblue.Foundation.Validation` | Validation helpers and DataAnnotations attributes |
| Collection modeling | `Navyblue.Foundation.Collections` | Weighted selection and collection result helpers |
| Observability context | `Navyblue.Foundation.Diagnostics` + AspNetCore Correlation | CorrelationContext, OperationTimer, CorrelationId middleware / HttpClient forwarding |
| Resilience | `Navyblue.Foundation.Resilience` | Lightweight retry helpers |
| CQRS / Mediator | `Navyblue.Foundation.Cqrs` | Command/Query/Event buses, pipeline, inbox/outbox |
| Grid / paging | `Navyblue.Foundation.Application` | `PageResult`, `QueryRequest`, `GridParameters`, `GridResult` |
| Web hosting | `Navyblue.Foundation.AspNetCore` | ApiResult, JWT, TraceId/Correlation, tenant, audit |

## Migration Strategy

- Existing `Navyblue.BaseLibrary` source stays in place for compatibility.
- New capabilities are added under `src/` (Foundation, AspNetCore, Cqrs, Testing).
- Modules stay low-dependency until a capability explicitly requires an integration package.
- Heavy integrations (EF Core, MongoDB, RabbitMQ, Serilog, Mapping, OpenTelemetry, Excel) are deferred — see [deferred-adapters.md](deferred-adapters.md).
- End-to-end acceptance sample: [samples/Navyblue.Samples](../samples/Navyblue.Samples/README.md).
- Source mapping checklist: [migration-capability-checklist.md](migration-capability-checklist.md).
