# NavyblueWebApi

Layered DDD/CQRS ASP.NET Core Web API generated from the **Navyblue** project template.  
Persistence: **EF Core + Pomelo + MySQL 8.0** · Cache: **Redis** (StackExchange.Redis).

## Projects

| Project | Role |
|---------|------|
| `NavyblueWebApi.Domain` | Aggregates / entities |
| `NavyblueWebApi.Model` | DTOs |
| `NavyblueWebApi.Application` | Commands, queries, handlers |
| `NavyblueWebApi.Infrastructure` | EF Core, Redis cache, migrations |
| `NavyblueWebApi.Web` | Host — JWT, Swagger, Serilog, health, rate limit |
| `NavyblueWebApi.Tests` | Unit tests (handlers + fakes) |

## Quick start

```bash
docker compose up -d
dotnet restore
dotnet run --project NavyblueWebApi.Web --environment Development
```

Swagger: `/swagger` · Health: `/health`

Demo admin (hard-coded in `Program.cs` when DB is empty):

- Login: `admin@navyblue.local`
- Password: `Admin@123`

## Configuration

Base `appsettings.json` leaves connection strings / JWT key empty.  
Local defaults: `appsettings.Development.json` · Production logging: `appsettings.Production.json`.

Override via environment variables or user-secrets:

```bash
dotnet user-secrets init --project NavyblueWebApi.Web
dotnet user-secrets set "ConnectionStrings:Default" "Server=...;Database=navyblue_webapi;User=...;Password=...;" --project NavyblueWebApi.Web
dotnet user-secrets set "Navyblue:Redis:ConnectionString" "127.0.0.1:6379" --project NavyblueWebApi.Web
dotnet user-secrets set "Jwt:SigningKey" "your-production-signing-key-32chars+" --project NavyblueWebApi.Web
```

Env var form: `ConnectionStrings__Default`, `Navyblue__Redis__ConnectionString`, `Jwt__SigningKey`.

## API notes

- `GET /api/users?keyword=&pageIndex=1&pageSize=20` → paged `PageData<UserModel>`
- `POST /api/auth/login` → access JWT + refresh token (rate limited)
- `POST /api/auth/refresh` → rotate refresh token, return new pair
- `POST /api/auth/logout` → revoke refresh token
- `DELETE /api/users/{id}` → **soft delete** (`IsDeleted`); frees email for reuse
- Redis cache-aside on `GetUser`; write commands invalidate `user:id:{id}`
- User audit fields: `CreatedBy` / `ModifiedBy` / `DeletedBy` via `ICurrentUser`

Refresh tokens are stored as SHA-256 hashes in `refresh_tokens`. Access token lifetime: `Jwt:Expire` (default 30m). Refresh lifetime: `RefreshToken:Expire` (default 7d).

## Logging

Serilog console sink; levels from `Serilog` section in appsettings. Request logging via `UseSerilogRequestLogging()`.

## Tests

```bash
dotnet test NavyblueWebApi.Tests
```

## Packages

- `Pomelo.EntityFrameworkCore.MySql` **9.0.0**
- `StackExchange.Redis`
- `Navyblue.Foundation*` **3.0.0**
