# NavyblueWebApi

Layered DDD/CQRS ASP.NET Core Web API generated from the **Navyblue** project template.

## Projects

| Project | Role |
|---------|------|
| `NavyblueWebApi.Domain` | Aggregates / entities |
| `NavyblueWebApi.Model` | DTOs |
| `NavyblueWebApi.Application` | Commands, queries, handlers |
| `NavyblueWebApi.Infrastructure` | In-memory repositories |
| `NavyblueWebApi.Web` | Host — JWT, Swagger, health checks, ApiResult |

## Run

```bash
dotnet restore
dotnet run --project NavyblueWebApi.Web
```

Open Swagger (Development): `/swagger`

Default admin (seeded):

- Login: `admin@navyblue.local`
- Password: `Admin@123`

```bash
curl -X POST http://localhost:5180/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"login\":\"admin@navyblue.local\",\"password\":\"Admin@123\"}"
```

Use the returned JWT as `Authorization: Bearer <token>` for `/api/users/*`.

## Configuration

See `NavyblueWebApi.Web/appsettings.json` (`Jwt`, `Navyblue`, `Seed`).

## Packages

Requires `Navyblue.Foundation` / `Navyblue.Foundation.Cqrs` / `Navyblue.Foundation.AspNetCore` **3.0.0**.
If restore fails, pack them from the Navyblue.BaseLibrary repo:

```powershell
./scripts/Install-NavyblueWebApiTemplate.ps1
```
