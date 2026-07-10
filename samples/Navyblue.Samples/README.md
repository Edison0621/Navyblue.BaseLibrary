# Navyblue.Samples

End-to-end DDD/CQRS sample on **Navyblue Foundation** — the reference implementation behind the
`navyblue-webapi` project template.

> Targets **net10.0**. Persistence is in-memory (no EF). Validation lives in each
> `Command.Validate()` / `Query.Validate()`.

## Layout

```
samples/Navyblue.Samples/
├── Navyblue.Samples.sln
├── Navyblue.Samples.Domain
├── Navyblue.Samples.Model
├── Navyblue.Samples.Application
├── Navyblue.Samples.Infrastructure
└── Navyblue.Samples.Web
```

## Features

- CQRS via `Navyblue.Foundation.Cqrs` (`ICommandBus` / `IQueryService`)
- Unified `ApiResult` responses + exception mapping
- JWT login (`POST /api/auth/login`) — user APIs require `[Authorize]`
- Swagger UI (Development), health check (`/health`), ping (`/_navyblue/ping`)
- Correlation / TraceId middleware
- Seeded admin: `admin@navyblue.local` / `Admin@123`

## Run

```bash
dotnet build samples/Navyblue.Samples/Navyblue.Samples.sln
dotnet run --project samples/Navyblue.Samples/Navyblue.Samples.Web
```

Swagger: `http://localhost:5180/swagger`

## Create your own project from the template

```powershell
# From repo root — packs packages + installs template
./scripts/Install-NavyblueWebApiTemplate.ps1

dotnet new navyblue-webapi -n Contoso.Catalog -o Contoso.Catalog
```

See [templates/Navyblue.WebApi/README.md](../../templates/Navyblue.WebApi/README.md).
