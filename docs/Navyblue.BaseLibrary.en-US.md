# Navyblue.BaseLibrary English Documentation

Navyblue.BaseLibrary is a modern foundation library for .NET enterprise applications. It starts with practical extensions and grows into reusable infrastructure for DDD modeling, application result models, paging, dependency injection, events, caching, idempotency, locks, diagnostics, ASP.NET Core APIs, and testing helpers.

## Design Goals

- Keep external NuGet dependencies low; prefer the BCL and `Microsoft.Extensions.*`.
- Preserve the original `Navyblue.BaseLibrary` code while adding modern .NET features through `ModernExtensions`.
- Support `net6.0`, `net7.0`, `net8.0`, `net9.0`, and `net10.0` across all packages.
- Let Foundation and Foundation.AspNetCore reuse extension methods from `Navyblue.BaseLibrary.Extensions`.
- Provide stable and low-intrusion enterprise building blocks without binding applications to a specific ORM or business framework.

## Packages and Target Frameworks

| Package | Target frameworks | Use case |
| --- | --- | --- |
| `Navyblue.BaseLibrary` | `net6.0;net7.0;net8.0;net9.0;net10.0` | Common extensions, JSON, Guid, hash, date, collections, Span, Memory, Stream, URI, HTTP, modern BCL helpers |
| `Navyblue.Foundation` | `net6.0;net7.0;net8.0;net9.0;net10.0` | DDD, application models, Result, paging, events, caching, idempotency, DI, diagnostics, files, HTTP, locks |
| `Navyblue.Foundation.AspNetCore` | `net6.0;net7.0;net8.0;net9.0;net10.0` | Standard Web API responses, JWT issuance/JwtBearer, exception mapping, TraceId, tenant context, audit, security headers, Minimal API helpers |
| `Navyblue.Foundation.Testing` | `net6.0;net7.0;net8.0;net9.0;net10.0` | Test clock, fake user/tenant/auditor, in-memory repo/cache/idempotency/lock, event spies, AspNetCore/JWT helpers, DI |

## Installation

Choose packages by scenario:

```bash
# Common extensions, Span/Memory, JSON, URI, HTTP, Guid v7, modern crypto
dotnet add package Navyblue.BaseLibrary

# DDD, Result, paging, events, caching, idempotency, locks, diagnostics, dependency injection
dotnet add package Navyblue.Foundation

# ASP.NET Core Web API, Minimal API, TraceId, tenant context, exception mapping, security headers
dotnet add package Navyblue.Foundation.AspNetCore

# Unit and integration testing helpers
dotnet add package Navyblue.Foundation.Testing
```

Use project references inside the repository:

```xml
<ItemGroup>
  <ProjectReference Include="..\Navyblue.BaseLibrary\Navyblue.BaseLibrary.csproj" />
  <ProjectReference Include="..\src\Navyblue.Foundation\Navyblue.Foundation.csproj" />
  <ProjectReference Include="..\src\Navyblue.Foundation.AspNetCore\Navyblue.Foundation.AspNetCore.csproj" />
</ItemGroup>
```

## 30-Second Quick Start

```csharp
using Navyblue.BaseLibrary;
using Navyblue.BaseLibrary.Extensions;

var json = new { Id = 1, Name = "Navyblue" }.ToJson();
var model = json.FromJson<Dictionary<string, object>>();

var slug = "OrderDetailPage".ToKebabCase();
var id = ModernGuidV7.NewGuidV7();
var hash = "hello"u8.ToArray().AsSpan().Sha256();
```

## 5-Minute Onboarding (Pick Packages by Scenario)

| What you are building | Install | Next section |
| --- | --- | --- |
| Console / utilities / extensions | `Navyblue.BaseLibrary` | Common Extensions |
| Domain modeling, Result, paging, DI | also `Navyblue.Foundation` | DDD Modeling; Result, API Result, and Paging |
| Web API / Minimal API | also `Navyblue.Foundation.AspNetCore` | ASP.NET Core Infrastructure; JWT Issuance and Automatic Parsing |
| Unit-test fakes | also `Navyblue.Foundation.Testing` | Testing Helpers |

Shortest Web API path: install `Navyblue.Foundation.AspNetCore` (pulls Foundation and BaseLibrary transitively) → copy the ASP.NET Core Infrastructure sample → run and hit `/_navyblue/ping` and `/orders`.

## Common Extensions

```csharp
using Navyblue.BaseLibrary;
using Navyblue.BaseLibrary.Extensions;

var json = new { Id = 1, Name = "Navyblue" }.ToJson();
var model = json.FromJson<Dictionary<string, object>>();

var slug = "OrderDetailPage".ToKebabCase();
var field = "OrderDetailPage".ToSnakeCase();
var normalized = "hello\r\nnavyblue".NormalizeLineEndings();

var page = Enumerable.Range(1, 100).GetPage(pageIndex: 0, pageSize: 10);
var chunks = Enumerable.Range(1, 10).ChunkBy(3);

var cache = new Dictionary<string, int>();
var retry = cache.GetOrAdd("retry", _ => 3);
```

## Span, Memory, and High-Performance Text

```csharp
using Navyblue.BaseLibrary.Extensions;
using System.Security.Cryptography;

ReadOnlySpan<char> text = "id,name,createdAt";
foreach (var part in text.Split(','))
{
    Console.WriteLine(part.ToString());
}

if ("12345".AsSpan().TryParseInt32(out var number))
{
    Console.WriteLine(number);
}

ReadOnlySpan<byte> bytes = SHA256.HashData("hello"u8.ToArray());
string hex = bytes.ToHexStringLower();
string base64 = bytes.ToBase64String();
```

## .NET 6-10 Type Helpers

```csharp
using Navyblue.BaseLibrary.Extensions;

DateOnly today = DateOnly.FromDateTime(DateTime.Today);
DateOnly endOfMonth = today.EndOfMonth();

TimeOnly start = new(22, 0);
TimeOnly end = new(2, 0);
bool inNightShift = new TimeOnly(23, 30).IsBetween(start, end);

Guid id = ModernGuidV7.NewGuidV7();
bool isV7 = id.IsVersion7();
DateTimeOffset? createdAt = id.GetVersion7Timestamp();
```

`net7.0+` provides generic parsing and large integer helpers:

```csharp
int number = "42".ParseOrDefault<int>();
decimal amount = "19.90".ParseOrDefault<decimal>();
Int128 big = Int128.Parse("170141183460469231731687303715884105727");
string invariant = big.ToInvariantString();
```

`net8.0+` provides helpers for `SearchValues`, frozen collections, `TimeProvider`, SHA-3, and HMAC-SHA3:

```csharp
var separators = ",;|".ToSearchValues();
bool hasSeparator = "a,b".AsSpan().ContainsAny(separators);

var frozen = new Dictionary<string, int>
{
    ["read"] = 1,
    ["write"] = 2
}.ToFrozenDictionarySafe(StringComparer.OrdinalIgnoreCase);
```

## Security, Encoding, and Crypto

```csharp
using Navyblue.BaseLibrary.Extensions;
using System.Security.Cryptography;
using System.Text;

byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
string token = tokenBytes.AsSpan().ToBase64UrlString();
byte[] restoredToken = token.FromBase64UrlString();

byte[] key = RandomNumberGenerator.GetBytes(32);
byte[] plaintext = Encoding.UTF8.GetBytes("hello navyblue");
AesGcmPayload encrypted = plaintext.AsSpan().EncryptAesGcm(key);
byte[] decrypted = encrypted.DecryptAesGcm(key);
```

## IO, URI, JSON, and Async Streams

```csharp
using Navyblue.BaseLibrary.Extensions;
using System.Text;
using System.Text.Json;

await using Stream stream = new MemoryStream(Encoding.UTF8.GetBytes("""{"name":"Navyblue"}"""));
string text = await stream.ReadAllTextAsync();

Uri uri = new Uri("https://api.example.com/orders")
    .AddQueryParameter("page", "1")
    .AddQueryParameter("pageSize", "20");

IReadOnlyDictionary<string, string?> query = uri.GetQueryParameters();

using JsonDocument document = JsonDocument.Parse("""{"name":"Navyblue","enabled":true}""");
string? name = document.RootElement.GetStringOrDefault("name");
bool enabled = document.RootElement.GetBooleanOrDefault("enabled");
```

> Tip: `ReadAllTextAsync` / `ReadAllBytesAsync` also work on `FileInfo`; swap in a real path in production code.

## HTTP, Expressions, Trees, and Collection Diffs

```csharp
using Navyblue.BaseLibrary.Extensions;
using System.Linq.Expressions;
using System.Net.Http;

using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders")
    .WithJsonAccept()
    .WithBearerToken("access-token")
    .WithCorrelationId(Guid.NewGuid().ToString("N"));

Expression<Func<OrderDto, bool>> paid = x => x.Status == "Paid";
Expression<Func<OrderDto, bool>> recent = x => x.PaidAt > DateTimeOffset.UtcNow.AddDays(-30);
var predicate = paid.And(recent);

var currentOrders = new[]
{
    new OrderDto(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Created", DateTimeOffset.UtcNow.AddDays(-40))
};
var nextOrders = new[]
{
    new OrderDto(currentOrders[0].Id, "Paid", DateTimeOffset.UtcNow),
    new OrderDto(Guid.NewGuid(), "Paid", DateTimeOffset.UtcNow)
};

var diff = currentOrders.DiffBy(nextOrders, current => current.Id, next => next.Id);

var menus = new[]
{
    new MenuItem(1, null, "System"),
    new MenuItem(2, 1, "Users")
};
var menuTree = menus.ToTree(idSelector: x => x.Id, parentIdSelector: x => x.ParentId);

public sealed record OrderDto(Guid Id, string Status, DateTimeOffset PaidAt);
public sealed record MenuItem(int Id, int? ParentId, string Name);
```
## DDD Modeling

```csharp
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;

var order = new Order(Guid.NewGuid(), "SO202607090001");
order.AddLine(Guid.NewGuid(), 2);
order.Pay();

public sealed class Order : FullAuditedAggregateRoot<Guid>
{
    private readonly List<OrderLine> _lines = [];

    public Order(Guid id, string number) : base(id)
    {
        Number = Guard.NotNullOrWhiteSpace(number, nameof(number));
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public string Number { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Created;
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    public void AddLine(Guid productId, int quantity)
    {
        Guard.GreaterThanZero(quantity, nameof(quantity));
        _lines.Add(new OrderLine(Guid.NewGuid(), productId, quantity));
        IncrementVersion();
    }

    public void Pay()
    {
        if (Status != OrderStatus.Created)
        {
            throw new DomainRuleViolationException("Only created orders can be paid.");
        }

        Status = OrderStatus.Paid;
        AddDomainEvent(new OrderPaid(Id));
        IncrementVersion();
    }
}

public sealed class OrderLine(Guid id, Guid productId, int quantity) : Entity<Guid>(id)
{
    public Guid ProductId { get; } = productId;
    public int Quantity { get; } = quantity;
}

public enum OrderStatus
{
    Created,
    Paid,
    Cancelled
}

public sealed record OrderPaid(Guid OrderId) : DomainEvent;
```
## Result, API Result, and Paging

```csharp
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Primitives;

Result<Guid> createResult = Result.Success(Guid.NewGuid());
Result fail = Result.Failure(new Error("order.invalid_status", "Order status is invalid."));

var request = new PageRequest(PageIndex: 1, PageSize: 20).Normalize(maxPageSize: 100);
var items = new[] { new OrderDto(Guid.NewGuid(), "Paid", DateTimeOffset.UtcNow) };

PageResult<OrderDto> page = items.ToPageResult(total: 1, request);
ApiResult<PageResult<OrderDto>> response = ApiResult<PageResult<OrderDto>>.Success(page);

public sealed record OrderDto(Guid Id, string Status, DateTimeOffset PaidAt);
```
## ASP.NET Core Infrastructure

After installing `Navyblue.Foundation.AspNetCore` in a Web project, use this as a minimal runnable `Program.cs`:

```csharp
using Navyblue.Foundation.Application;
using Navyblue.Foundation.AspNetCore;
using Navyblue.Foundation.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNavyblueFoundation();
builder.Services.AddNavyblueFramework(options =>
{
    options.TraceHeaderName = "X-Trace-Id";
    options.TenantHeaderName = "X-Tenant-Id";
    options.EnableSecurityHeaders = true;
    options.EnableAuditLogging = true;
});

var app = builder.Build();

app.UseNavyblueFramework();
app.MapNavybluePing();
app.MapNavyblueInfo("/_info", "Payment.Api", "2.1.0");

app.MapGet("/orders/{id:guid}", (Guid id, HttpContext context) =>
{
    var order = new { Id = id, Status = "Paid" };
    return NavyblueResults.Ok(order, traceId: context.GetTraceId());
});

app.MapGet("/orders", (HttpContext context) =>
{
    var items = new[] { new { Id = Guid.NewGuid(), Status = "Paid" } };
    var page = items.ToPageResult(total: 1, new PageRequest(1, 20));
    return NavyblueResults.Page(page, traceId: context.GetTraceId());
});

app.MapGet("/me", (HttpContext context) =>
{
    return NavyblueResults.Ok(new
    {
        TraceId = context.GetTraceId(),
        TenantId = context.GetTenantId(),
        UserId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
    }, traceId: context.GetTraceId());
});

app.Run();
```

Verify after running:

- `GET /_navyblue/ping` — health probe
- `GET /orders` — standard paged `ApiResult<PageResult<T>>`
- Send `X-Trace-Id` / `X-Tenant-Id` headers — middleware propagates and resolves them

Built-in capabilities include global exception mapping, TraceId propagation, tenant resolution, security response headers, request logging, audit logging, Minimal API standard responses, and health endpoints. For Controllers, use `OkApi` / `FailApi`; for model validation failures, wire `AddNavyblueApiBehavior()`.

> Note: `AddNavyblueRateLimiting` / `UseNavyblueRateLimiting` and `AddNavyblueProblemDetails` are available on **.NET 7+** only; `UseNavyblueStatusCodePages` works from net6 onward.

## JWT Issuance and Automatic Parsing

JWT is **opt-in** (same as ApiKey) and is not enabled by default. Your app decides which claims to embed; after validation, JwtBearer populates `HttpContext.User`, so existing `ICurrentUser` / `ICurrentTenant` work without changes.

Pipeline order:

```text
UseNavyblueJwtAuthentication()   // UseAuthentication
UseAuthorization()               // host as needed
UseNavyblueFramework()           // TraceId / Tenant / RequestContext can read User
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
    options.SigningKey = builder.Configuration["Jwt:SigningKey"]!; // prefer >= 32 bytes
    options.Expire = TimeSpan.FromHours(2);
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseNavyblueJwtAuthentication();
app.UseAuthorization();
app.UseNavyblueFramework();

app.MapPost("/login", (LoginRequest request, IJwtTokenService tokens) =>
{
    // Skip real credential checks in this sample; claims are fully caller-defined
    // (user id, merchant id, or any custom business fields).
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

You can also pass an arbitrary claim map:

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

Recommended claims for out-of-the-box current-user support:

| Claim | Used by |
| --- | --- |
| `Subject` / `WithSubject` → `sub` + `NameIdentifier` | `ICurrentUser.UserId` |
| `WithUserName` / `ClaimTypes.Name` | `ICurrentUser.UserName` |
| `WithRoles` / `ClaimTypes.Role` | `ICurrentUser.Roles` / `IsInRole` |
| `WithTenantId` / `tenant_id` | `ICurrentTenant.TenantId` |
| `WithMerchantId` / `merchant_id` | `ICurrentUser.FindClaimValue(JwtClaimNames.MerchantId)` |
| `WithClaim("any-type", value)` | Any business field; read back with `FindClaimValue` |

`AddNavyblueJwt` throws if `SigningKey` is missing.
## Testing Helpers

```csharp
using Microsoft.Extensions.DependencyInjection;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Testing;

var services = new ServiceCollection();
services.AddNavyblueTestingFoundation(); // TestClock, fake user/tenant, event collector, in-memory infra

// Or for Web/integration tests:
// services.AddNavyblueTestingAspNetCore();

using ServiceProvider provider = services.BuildServiceProvider();
var clock = provider.GetRequiredService<TestClock>();
clock.SetUtcNow(new DateTimeOffset(2026, 7, 9, 6, 0, 0, TimeSpan.Zero));

ICurrentUser user = provider.GetRequiredService<ICurrentUser>();
ICurrentTenant tenant = provider.GetRequiredService<ICurrentTenant>();
var events = provider.GetRequiredService<InMemoryDomainEventCollector>();

var principal = TestClaimsPrincipal.Create(
    userId: "u-001",
    userName: "Alice",
    roles: ["admin"],
    tenantId: "tenant-001",
    merchantId: "m-001");

FakeCurrentUser fromPrincipal = FakeCurrentUser.FromPrincipal(principal);
```

Common types:

| Type | Purpose |
| --- | --- |
| `TestClock` / `IClock` | Controllable time via `Advance` / `SetUtcNow` |
| `FakeCurrentUser` / `FakeCurrentTenant` | Replace HTTP current user/tenant |
| `InMemoryDomainEventCollector` | Assert domain events |
| `SpyEventBus` | Assert integration events |
| `InMemoryRepository<T>` / `InMemoryUnitOfWork` | Repository and UoW without a database |
| `InMemoryCacheProvider` / `InMemoryIdempotencyStore` / `InMemoryDistributedLockProvider` | Infrastructure fakes |
| `FakeAuditor` / `FakePermissionChecker` / `SequentialIdGenerator` | Domain/application fakes |
| `HttpContextTestHelper` / `JwtTestHelper` / `InMemoryAuditLogSink` | AspNetCore and JWT testing |

Use `AddNavyblueTestingFoundation()` or `AddNavyblueTestingAspNetCore()` to swap production implementations in integration tests; build claims with `TestClaimsPrincipal` and issue tokens with `JwtTestHelper.CreateTokenService()`.

## Recommended Layering

```text
Web/API Project
  -> Navyblue.Foundation.AspNetCore
  -> Navyblue.Foundation
  -> Navyblue.BaseLibrary

Application Project
  -> Navyblue.Foundation
  -> Navyblue.BaseLibrary

Domain Project
  -> Navyblue.Foundation
  -> Navyblue.BaseLibrary

Shared/Utilities Project
  -> Navyblue.BaseLibrary
```

## Build

```bash
dotnet build Navyblue.BaseLibrary.sln
```