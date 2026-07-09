# Navyblue.BaseLibrary English Documentation

Navyblue.BaseLibrary is a modern foundation library for .NET enterprise applications. It starts with practical extensions and grows into reusable infrastructure for DDD modeling, application result models, paging, dependency injection, events, caching, idempotency, locks, diagnostics, ASP.NET Core APIs, and testing helpers.

## Design Goals

- Keep external NuGet dependencies low; prefer the BCL and `Microsoft.Extensions.*`.
- Preserve the original `Navyblue.BaseLibrary` code while adding modern .NET features through `ModernExtensions`.
- Support `net6.0`, `net7.0`, `net8.0`, `net9.0`, and `net10.0` in the base package.
- Let Core and AspNetCore reuse extension methods from `Navyblue.BaseLibrary.Extensions`.
- Provide stable and low-intrusion enterprise building blocks without binding applications to a specific ORM or business framework.

## Packages and Target Frameworks

| Package | Target frameworks | Use case |
| --- | --- | --- |
| `Navyblue.BaseLibrary` | `net6.0;net7.0;net8.0;net9.0;net10.0` | Common extensions, JSON, Guid, hash, date, collections, Span, Memory, Stream, URI, HTTP, modern BCL helpers |
| `Navyblue.BaseLibrary.Core` | `net8.0;net10.0` | DDD, application models, Result, paging, events, caching, idempotency, DI, diagnostics, files, HTTP, locks |
| `Navyblue.BaseLibrary.AspNetCore` | `net8.0;net10.0` | Standard Web API responses, exception mapping, TraceId, tenant context, audit, security headers, Minimal API helpers |
| `Navyblue.BaseLibrary.Testing` | `net8.0;net10.0` | Test clock, fake current user, fake tenant, claims principal, in-memory domain event collector |

## Installation

Choose packages by scenario:

```bash
# Common extensions, Span/Memory, JSON, URI, HTTP, Guid v7, modern crypto
dotnet add package Navyblue.BaseLibrary

# DDD, Result, paging, events, caching, idempotency, locks, diagnostics, dependency injection
dotnet add package Navyblue.BaseLibrary.Core

# ASP.NET Core Web API, Minimal API, TraceId, tenant context, exception mapping, security headers
dotnet add package Navyblue.BaseLibrary.AspNetCore

# Unit and integration testing helpers
dotnet add package Navyblue.BaseLibrary.Testing
```

Use project references inside the repository:

```xml
<ItemGroup>
  <ProjectReference Include="..\Navyblue.BaseLibrary\Navyblue.BaseLibrary.csproj" />
  <ProjectReference Include="..\src\Navyblue.BaseLibrary.Core\Navyblue.BaseLibrary.Core.csproj" />
  <ProjectReference Include="..\src\Navyblue.BaseLibrary.AspNetCore\Navyblue.BaseLibrary.AspNetCore.csproj" />
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
using System.Text.Json;

await using Stream stream = File.OpenRead("appsettings.json");
string text = await stream.ReadAllTextAsync();
byte[] bytes = await new FileInfo("appsettings.json").ReadAllBytesAsync();

Uri uri = new Uri("https://api.example.com/orders")
    .AddQueryParameter("page", "1")
    .AddQueryParameter("pageSize", "20");

IReadOnlyDictionary<string, string?> query = uri.GetQueryParameters();

using JsonDocument document = JsonDocument.Parse("""{""name"":""Navyblue"",""enabled"":true}""");
string? name = document.RootElement.GetStringOrDefault("name");
bool enabled = document.RootElement.GetBooleanOrDefault("enabled");
```

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
using Navyblue.BaseLibrary.Domain;
using Navyblue.BaseLibrary.Primitives;

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
using Navyblue.BaseLibrary.Application;
using Navyblue.BaseLibrary.Primitives;

Result<Guid> createResult = Result.Success(Guid.NewGuid());
Result fail = Result.Failure(new Error("order.invalid_status", "Order status is invalid."));

var request = new PageRequest(PageIndex: 1, PageSize: 20).Normalize(maxPageSize: 100);
var items = new[] { new OrderDto(Guid.NewGuid(), "Paid", DateTimeOffset.UtcNow) };

PageResult<OrderDto> page = items.ToPageResult(total: 1, request);
ApiResult<PageResult<OrderDto>> response = ApiResult<PageResult<OrderDto>>.Success(page);

public sealed record OrderDto(Guid Id, string Status, DateTimeOffset PaidAt);
```
## ASP.NET Core Infrastructure

```csharp
using Navyblue.BaseLibrary.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/me", (HttpContext context) =>
{
    return Results.Ok(new
    {
        TraceId = context.GetTraceId(),
        TenantId = context.GetTenantId(),
        UserId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
    });
});

app.Run();
```

Built-in capabilities include global exception mapping, TraceId propagation, tenant resolution, security response headers, request logging, audit logging, Minimal API standard responses, and health endpoints.
## Testing Helpers

```csharp
using Microsoft.Extensions.DependencyInjection;
using Navyblue.BaseLibrary.Application;
using Navyblue.BaseLibrary.Testing;

var currentUser = new FakeCurrentUser
{
    UserId = "u-001",
    UserName = "Alice",
    IsAuthenticated = true
};
currentUser.RoleList.Add("admin");

var principal = TestClaimsPrincipal.Create(
    userId: "u-001",
    userName: "Alice",
    roles: ["admin"],
    tenantId: "tenant-001");
```

## Recommended Layering

```text
Web/API Project
  -> Navyblue.BaseLibrary.AspNetCore
  -> Navyblue.BaseLibrary.Core
  -> Navyblue.BaseLibrary

Application Project
  -> Navyblue.BaseLibrary.Core
  -> Navyblue.BaseLibrary

Domain Project
  -> Navyblue.BaseLibrary.Core
  -> Navyblue.BaseLibrary

Shared/Utilities Project
  -> Navyblue.BaseLibrary
```

## Build

```bash
dotnet build Navyblue.BaseLibrary.sln
```