# Navyblue.BaseLibrary

Navyblue.BaseLibrary is a modern .NET foundation library for enterprise applications. It provides common extensions, modern BCL helpers, DDD primitives, application result models, ASP.NET Core infrastructure, and testing helpers while keeping external dependencies low.

Navyblue.BaseLibrary 是一个面向 .NET 企业应用的现代化基础库，覆盖常用扩展方法、现代 BCL 类型、DDD 基础建模、应用层返回模型、ASP.NET Core 基础设施和测试辅助，同时尽量减少外部依赖。

## Documentation / 文档

- [中文文档](docs/Navyblue.BaseLibrary.zh-CN.md)（推荐新人从「5 分钟上手」开始）
- [English Documentation](docs/Navyblue.BaseLibrary.en-US.md) (start with **5-Minute Onboarding**)

## Packages

| Package | Target frameworks | Purpose |
| --- | --- | --- |
| `Navyblue.BaseLibrary` | `net6.0;net7.0;net8.0;net9.0;net10.0` | Common extensions, JSON, Guid, hash, Span/Memory, stream, URI, HTTP, modern BCL helpers |
| `Navyblue.Foundation` | `net6.0;net7.0;net8.0;net9.0;net10.0` | DDD, Result, paging, events, caching, idempotency, locks, diagnostics, DI |
| `Navyblue.Foundation.AspNetCore` | `net6.0;net7.0;net8.0;net9.0;net10.0` | Standard Web API responses, JWT issuance/JwtBearer, exception mapping, TraceId, tenant context, audit, security headers, Minimal API helpers |
| `Navyblue.Foundation.Testing` | `net6.0;net7.0;net8.0;net9.0;net10.0` | Fake current user, fake tenant, test clock, claims principal, in-memory domain events |

## Quick Start / 快速开始

### 1. Extensions only / 仅扩展方法

```bash
dotnet add package Navyblue.BaseLibrary
```

```csharp
using Navyblue.BaseLibrary;
using Navyblue.BaseLibrary.Extensions;

var json = new { Id = 1, Name = "Navyblue" }.ToJson();
var model = json.FromJson<Dictionary<string, object>>();

var slug = "OrderDetailPage".ToKebabCase();
var endOfMonth = DateOnly.FromDateTime(DateTime.Today).EndOfMonth();
var hash = "hello"u8.ToArray().AsSpan().Sha256();
```

### 2. Web API (recommended path) / Web API（推荐路径）

```bash
dotnet add package Navyblue.Foundation.AspNetCore
```

```csharp
using Navyblue.Foundation.Application;
using Navyblue.Foundation.AspNetCore;
using Navyblue.Foundation.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNavyblueFoundation();
builder.Services.AddNavyblueFramework();

var app = builder.Build();

app.UseNavyblueFramework();
app.MapNavybluePing();

app.MapGet("/orders/{id:guid}", (Guid id, HttpContext context) =>
    NavyblueResults.Ok(new { Id = id, Status = "Paid" }, traceId: context.GetTraceId()));

app.Run();
```

Then open `GET /_navyblue/ping` and `GET /orders/{guid}`. Full samples (DDD, paging, JWT, testing) are in the docs above.

然后访问 `GET /_navyblue/ping` 与 `GET /orders/{guid}`。完整示例（DDD、分页、JWT、测试）见上方文档。

### 3. JWT (opt-in) / JWT（可选）

JWT is opt-in. Your app decides which claims to embed (`UserId`, `MerchantId`, `store_id`, or any custom field). After validation, JwtBearer fills `HttpContext.User`; `ICurrentUser` / `ICurrentTenant` read them automatically.

JWT 为 opt-in。业务自行决定写入哪些 Claims（`UserId`、`MerchantId`、`store_id` 或任意自定义字段）。校验成功后 JwtBearer 填充 `HttpContext.User`，`ICurrentUser` / `ICurrentTenant` 可直接读取。

**Pipeline / 管道顺序**

```text
UseNavyblueJwtAuthentication()   // UseAuthentication
UseAuthorization()               // host as needed / 宿主按需
UseNavyblueFramework()           // must run after auth / 必须在认证之后
```

```csharp
using System.Security.Claims;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.AspNetCore;
using Navyblue.Foundation.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNavyblueFoundation();
builder.Services.AddNavyblueFramework();
builder.Services.AddNavyblueJwt(options =>
{
    options.Issuer = "Navyblue";
    options.Audience = "Navyblue.Api";
    options.SigningKey = builder.Configuration["Jwt:SigningKey"]!; // prefer >= 32 bytes / 建议 >= 32 字节
    options.Expire = TimeSpan.FromHours(2);
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseNavyblueJwtAuthentication();
app.UseAuthorization();
app.UseNavyblueFramework();

app.MapPost("/login", (LoginRequest request, IJwtTokenService tokens) =>
{
    // Claims are fully caller-defined / Claims 完全由业务决定
    string jwt = tokens.CreateToken(d => d
        .WithSubject(request.UserId)
        .WithUserName(request.UserName)
        .WithRoles("admin")
        .WithTenantId(request.TenantId)
        .WithMerchantId(request.MerchantId)
        .WithClaim("store_id", request.StoreId)
        .WithClaim("dept_id", "D01"));
    return NavyblueResults.Ok(new { accessToken = jwt, tokenType = "Bearer" });
});

app.MapGet("/me", (ICurrentUser user, ICurrentTenant tenant, HttpContext context) =>
{
    return NavyblueResults.Ok(new
    {
        user.UserId,
        user.UserName,
        Roles = user.Roles,
        tenant.TenantId,
        MerchantId = user.FindClaimValue(JwtClaimNames.MerchantId),
        StoreId = user.FindClaimValue("store_id"),
        DeptId = user.FindClaimValue("dept_id")
    }, traceId: context.GetTraceId());
}).RequireAuthorization();

app.Run();

public sealed record LoginRequest(string UserId, string UserName, string TenantId, string MerchantId, string StoreId);
```

**Dictionary overload / 字典重载**（任意业务字段一次性写入）:

```csharp
string jwt = tokens.CreateToken(request.UserId, new Dictionary<string, string>
{
    [ClaimTypes.Name] = request.UserName,
    [JwtClaimNames.TenantId] = request.TenantId,
    [JwtClaimNames.MerchantId] = request.MerchantId,
    ["store_id"] = request.StoreId,
    ["channel"] = "pos"
});
```

**Recommended claims / 推荐 Claim**

| Claim | Used by / 用途 |
| --- | --- |
| `WithSubject` → `sub` + `NameIdentifier` | `ICurrentUser.UserId` |
| `WithUserName` / `ClaimTypes.Name` | `ICurrentUser.UserName` |
| `WithRoles` / `ClaimTypes.Role` | `ICurrentUser.Roles` / `IsInRole` |
| `WithTenantId` / `tenant_id` | `ICurrentTenant.TenantId` |
| `WithMerchantId` / `merchant_id` | `ICurrentUser.FindClaimValue(JwtClaimNames.MerchantId)` |
| `WithClaim("any-type", value)` | Any business field / 任意业务字段 → `FindClaimValue` |

`AddNavyblueJwt` throws if `SigningKey` is missing. / 未配置 `SigningKey` 时会抛异常。

More detail: [中文文档](docs/Navyblue.BaseLibrary.zh-CN.md#jwt-签发与自动解析) · [English docs](docs/Navyblue.BaseLibrary.en-US.md#jwt-issuance-and-automatic-parsing)

## Build / 构建

```bash
dotnet build Navyblue.BaseLibrary.sln
```
