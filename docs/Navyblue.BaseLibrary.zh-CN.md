# Navyblue.BaseLibrary 中文文档

Navyblue.BaseLibrary 是一个面向 .NET 企业应用的基础库集合。它从常用扩展方法出发，覆盖 DDD 建模、应用层返回模型、分页查询、依赖注入、事件、缓存、幂等、锁、诊断、ASP.NET Core 基础设施和测试辅助等场景，让业务代码更少关注样板代码。

## 设计目标

- 尽量减少外部 NuGet 依赖，优先使用 BCL 和 `Microsoft.Extensions.*`。
- 保留原 `Navyblue.BaseLibrary` 代码，通过 `ModernExtensions` 增加现代 .NET 能力。
- 所有包均支持 `net6.0`、`net7.0`、`net8.0`、`net9.0`、`net10.0`。
- Foundation 与 Foundation.AspNetCore 复用 `Navyblue.BaseLibrary.Extensions` 中的扩展方法。
- 提供稳定、低侵入的企业级基础设施，不绑定特定 ORM 或业务框架。

## 包和目标框架

| Package | Target frameworks | 适用场景 |
| --- | --- | --- |
| `Navyblue.BaseLibrary` | `net6.0;net7.0;net8.0;net9.0;net10.0` | 基础扩展、JSON、Guid、Hash、日期、集合、Span、Memory、Stream、URI、HTTP、现代 BCL 类型扩展 |
| `Navyblue.Foundation` | `net6.0;net7.0;net8.0;net9.0;net10.0` | DDD、应用层模型、Result、分页、事件、缓存、幂等、DI、诊断、文件、HTTP、锁 |
| `Navyblue.Foundation.AspNetCore` | `net6.0;net7.0;net8.0;net9.0;net10.0` | Web API 标准响应、JWT 签发/JwtBearer、异常映射、TraceId、租户、审计、安全响应头、Minimal API 辅助 |
| `Navyblue.Foundation.Testing` | `net6.0;net7.0;net8.0;net9.0;net10.0` | 测试时钟、Fake 用户/租户/审计、内存仓储/缓存/幂等/锁、事件收集与 Spy、AspNetCore/JWT 测试辅助与 DI |

## 安装

按使用场景选择包：

```bash
# 基础扩展、Span/Memory、JSON、URI、HTTP、Guid v7、现代加密
dotnet add package Navyblue.BaseLibrary

# DDD、Result、分页、事件、缓存、幂等、锁、诊断、依赖注入
dotnet add package Navyblue.Foundation

# ASP.NET Core Web API、Minimal API、TraceId、租户、异常映射、安全响应头
dotnet add package Navyblue.Foundation.AspNetCore

# 单元测试和集成测试辅助
dotnet add package Navyblue.Foundation.Testing
```

仓库内项目引用：

```xml
<ItemGroup>
  <ProjectReference Include="..\Navyblue.BaseLibrary\Navyblue.BaseLibrary.csproj" />
  <ProjectReference Include="..\src\Navyblue.Foundation\Navyblue.Foundation.csproj" />
  <ProjectReference Include="..\src\Navyblue.Foundation.AspNetCore\Navyblue.Foundation.AspNetCore.csproj" />
</ItemGroup>
```

## 30 秒快速开始

```csharp
using Navyblue.BaseLibrary;
using Navyblue.BaseLibrary.Extensions;

var json = new { Id = 1, Name = "Navyblue" }.ToJson();
var model = json.FromJson<Dictionary<string, object>>();

var slug = "OrderDetailPage".ToKebabCase();
var id = ModernGuidV7.NewGuidV7();
var hash = "hello"u8.ToArray().AsSpan().Sha256();
```

## 5 分钟上手（按场景选包）

| 你在做什么 | 安装 | 下一步 |
| --- | --- | --- |
| 控制台 / 工具类 / 扩展方法 | `Navyblue.BaseLibrary` | 看下方「基础扩展方法」 |
| 领域建模、Result、分页、DI | 再加 `Navyblue.Foundation` | 看「DDD 建模」「Result、API Result 和分页」 |
| Web API / Minimal API | 再加 `Navyblue.Foundation.AspNetCore` | 看「ASP.NET Core 基础设施」「JWT 签发与自动解析」 |
| 单元测试 Fake | 再加 `Navyblue.Foundation.Testing` | 看「测试辅助」 |

最短 Web API 路径：安装 `Navyblue.Foundation.AspNetCore`（会传递引用 Foundation 与 BaseLibrary）→ 复制「ASP.NET Core 基础设施」示例 → 运行后访问 `/_navyblue/ping` 和 `/orders`。

## 基础扩展方法

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

## Span、Memory 和高性能文本处理

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

## .NET 6-10 新类型和现代能力

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

`net7.0+` 支持通用解析和大整数辅助：

```csharp
int number = "42".ParseOrDefault<int>();
decimal amount = "19.90".ParseOrDefault<decimal>();
Int128 big = Int128.Parse("170141183460469231731687303715884105727");
string invariant = big.ToInvariantString();
```

`net8.0+` 支持 `SearchValues`、Frozen 集合、`TimeProvider`、SHA-3 和 HMAC-SHA3 辅助：

```csharp
var separators = ",;|".ToSearchValues();
bool hasSeparator = "a,b".AsSpan().ContainsAny(separators);

var frozen = new Dictionary<string, int>
{
    ["read"] = 1,
    ["write"] = 2
}.ToFrozenDictionarySafe(StringComparer.OrdinalIgnoreCase);
```

## 安全、编码和加密

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

## IO、URI、JSON 和异步流

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

> 提示：`ReadAllTextAsync` / `ReadAllBytesAsync` 也支持 `FileInfo`；生产代码里再换成真实文件路径即可。

## HTTP、表达式、树形结构和集合差异

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
## DDD 建模

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
## Result、API Result 和分页

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
## ASP.NET Core 基础设施

在 Web 项目中安装 `Navyblue.Foundation.AspNetCore` 后，用下面这段作为最小可运行入口（`Program.cs`）：

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

运行后可验证：

- `GET /_navyblue/ping` — 健康探测
- `GET /orders` — 标准分页 `ApiResult<PageResult<T>>`
- 请求头带 `X-Trace-Id` / `X-Tenant-Id` — 中间件会透传与解析

内置能力包括全局异常映射、TraceId 透传、租户解析、安全响应头、请求日志、审计日志、Minimal API 标准返回和健康检查端点。Controller 场景可用 `OkApi` / `FailApi`；模型验证失败可接 `AddNavyblueApiBehavior()`。

> 说明：`AddNavyblueRateLimiting` / `UseNavyblueRateLimiting` 与 `AddNavyblueProblemDetails` 仅在 **.NET 7+** 可用；`UseNavyblueStatusCodePages` 在 net6 起均可用。

## JWT 签发与自动解析

JWT 为 **opt-in**（与 ApiKey 相同），不默认启用。业务自行决定写入哪些 Claims；校验成功后 JwtBearer 会填充 `HttpContext.User`，现有 `ICurrentUser` / `ICurrentTenant` 可直接读取。

管道顺序：

```text
UseNavyblueJwtAuthentication()   // UseAuthentication
UseAuthorization()               // 宿主按需
UseNavyblueFramework()           // TraceId / Tenant / RequestContext 才能读到 User
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
    options.SigningKey = builder.Configuration["Jwt:SigningKey"]!; // 建议 >= 32 字节
    options.Expire = TimeSpan.FromHours(2);
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseNavyblueJwtAuthentication();
app.UseAuthorization();
app.UseNavyblueFramework();

app.MapPost("/login", (LoginRequest request, IJwtTokenService tokens) =>
{
    // 此处省略真实账号校验；Claims 完全由业务决定（UserId / MerchantId / 任意自定义字段）
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

也可用字典一次性写入任意业务字段：

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

推荐写入（便于开箱即用）：

| Claim | 用途 |
| --- | --- |
| `Subject` / `WithSubject` → `sub` + `NameIdentifier` | `ICurrentUser.UserId` |
| `WithUserName` / `ClaimTypes.Name` | `ICurrentUser.UserName` |
| `WithRoles` / `ClaimTypes.Role` | `ICurrentUser.Roles` / `IsInRole` |
| `WithTenantId` / `tenant_id` | `ICurrentTenant.TenantId` |
| `WithMerchantId` / `merchant_id` | `ICurrentUser.FindClaimValue(JwtClaimNames.MerchantId)` |
| `WithClaim("任意类型", value)` | 任意业务字段，解析后同样用 `FindClaimValue` 读取 |

`SigningKey` 未配置时 `AddNavyblueJwt` 会抛异常。
## 测试辅助

```csharp
using Microsoft.Extensions.DependencyInjection;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Testing;

var services = new ServiceCollection();
services.AddNavyblueTestingFoundation(); // 注册 TestClock、Fake 用户/租户、事件收集、内存缓存/幂等等

// 或 Web/集成测试：
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

常用类型：

| 类型 | 用途 |
| --- | --- |
| `TestClock` / `IClock` | 可控时间，`Advance` / `SetUtcNow` |
| `FakeCurrentUser` / `FakeCurrentTenant` | 替换 HTTP 上下文用户与租户 |
| `InMemoryDomainEventCollector` | 断言领域事件 |
| `SpyEventBus` | 断言集成事件 |
| `InMemoryRepository<T>` / `InMemoryUnitOfWork` | 无数据库仓储与 UoW |
| `InMemoryCacheProvider` / `InMemoryIdempotencyStore` / `InMemoryDistributedLockProvider` | 基础设施 Fake |
| `FakeAuditor` / `FakePermissionChecker` / `SequentialIdGenerator` | 领域与应用层 Fake |
| `HttpContextTestHelper` / `JwtTestHelper` / `InMemoryAuditLogSink` | AspNetCore 与 JWT 测试 |

在集成测试里用 `AddNavyblueTestingFoundation()` 或 `AddNavyblueTestingAspNetCore()` 一键替换生产实现；用 `TestClaimsPrincipal` / `JwtTestHelper.CreateTokenService()` 构造 Claims 与真实签发服务。

## 推荐分层引用

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

## 构建

```bash
dotnet build Navyblue.BaseLibrary.sln
```