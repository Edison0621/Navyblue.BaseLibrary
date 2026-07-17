# Navyblue.Templates.WebApi

ASP.NET Core Web API project template for **Visual Studio** and **`dotnet new`**, based on the Navyblue layered DDD/CQRS reference with **EF Core + Pomelo + MySQL 8.0** and **Redis** cache.

## Short name

```text
navyblue-webapi
```

## Install (local repo)

From the repository root:

```powershell
# 1) Pack Navyblue foundation packages + this template into artifacts/packages
./scripts/Install-NavyblueWebApiTemplate.ps1

# 2) Create a new solution
dotnet new navyblue-webapi -n Contoso.Catalog -o Contoso.Catalog
cd Contoso.Catalog

# 3) Set MySQL/Redis (or: docker compose up -d), then:
dotnet run --project Contoso.Catalog.Web
```

Optional infra:

```bash
docker compose up -d
```

Open Swagger at the launch URL (default `http://localhost:5180/swagger`).

## Install (NuGet package)

```bash
dotnet new install Navyblue.Templates.WebApi::3.3.0
dotnet new navyblue-webapi -n MyCompany.MyService
```

## Visual Studio

After `dotnet new install`, the template appears under **Create a new project** as
**"Navyblue ASP.NET Core Web API"**.

## What you get

| Project | Role |
|---------|------|
| `*.Domain` | Aggregates / entities (`Navyblue.Foundation.Domain`) |
| `*.Model` | DTOs |
| `*.Application` | Commands / Queries / Handlers (`Navyblue.Foundation.Cqrs`) |
| `*.Infrastructure` | EF Core `AppDbContext`, Pomelo MySQL repos, Redis cache, migrations |
| `*.Web` | ASP.NET Core host — JWT + refresh token (login/refresh/logout), Swagger, MySQL/Redis health, login rate limit, ApiResult |
| `*.Tests` | Handler unit tests |



## Prerequisites

- MySQL **8.0**
- Redis
- `Navyblue.Foundation`, `Navyblue.Foundation.Cqrs`, and `Navyblue.Foundation.AspNetCore` **3.0.0**
  must resolve from NuGet.org or a local feed (the install script configures `artifacts/packages`).

## Uninstall

```bash
dotnet new uninstall Navyblue.Templates.WebApi
```
