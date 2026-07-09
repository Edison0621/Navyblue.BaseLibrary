# Navyblue.BaseLibrary

Navyblue.BaseLibrary 是一个面向 .NET 企业应用的基础库集合。它从常用扩展方法起步，逐步覆盖 DDD 建模、应用层返回模型、分页查询、依赖注入、事件、缓存、幂等、锁、诊断、ASP.NET Core 基础设施等场景，让业务代码更少关注样板代码，把注意力放在页面、交互和业务流程本身。

## 适用目标框架

| Package | Target frameworks | 适用场景 |
| --- | --- | --- |
| `Navyblue.BaseLibrary` | `net6.0;net7.0;net8.0;net9.0;net10.0` | 基础扩展、JSON、Guid、Hash、日期、集合、Span、现代 BCL 类型扩展 |
| `Navyblue.BaseLibrary.Core` | `net8.0;net10.0` | DDD、应用层模型、Result、分页、事件、缓存、幂等、DI、诊断、文件、HTTP、锁 |
| `Navyblue.BaseLibrary.AspNetCore` | `net8.0;net10.0` | Web API 标准响应、异常映射、TraceId、租户、审计、安全响应头、Minimal API 辅助 |
| `Navyblue.BaseLibrary.Testing` | `net8.0;net10.0` | 测试时钟、测试用户、测试租户、内存事件收集等测试辅助 |

## 安装

NuGet 使用方式：

```bash
dotnet add package Navyblue.BaseLibrary
```

如果项目还没有发布拆分包，可以在 solution 内通过项目引用使用：

```xml
<ItemGroup>
  <ProjectReference Include="..\Navyblue.BaseLibrary\Navyblue.BaseLibrary.csproj" />
  <ProjectReference Include="..\src\Navyblue.BaseLibrary.Core\Navyblue.BaseLibrary.Core.csproj" />
  <ProjectReference Include="..\src\Navyblue.BaseLibrary.AspNetCore\Navyblue.BaseLibrary.AspNetCore.csproj" />
</ItemGroup>
```

## 5 分钟 Demo：一个标准 Minimal API

```csharp
using Navyblue.BaseLibrary.Application;
using Navyblue.BaseLibrary.AspNetCore;
using Navyblue.BaseLibrary.DependencyInjection;
using Navyblue.BaseLibrary.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNavyblueCore(options =>
{
    options.WorkerId = 1;
    options.DataCenterId = 1;
});

builder.Services.AddNavyblueFramework(options =>
{
    options.EnableTraceId = true;
    options.EnableRequestLogging = true;
    options.EnableSecurityHeaders = true;
    options.EnableTenantResolution = true;
});

builder.Services.AddNavyblueConventionalServicesFrom(typeof(Program).Assembly);

var app = builder.Build();

app.UseNavyblueFramework();

app.MapNavybluePing();
app.MapNavyblueInfo(applicationName: "Order.Api", version: "1.0.0");

app.MapGet("/orders/{id:guid}", (Guid id, IOrderAppService service, HttpContext context) =>
{
    var order = service.Get(id);
    return order is null
        ? NavyblueResults.Fail(BusinessCode.NotFound, "Order not found.", StatusCodes.Status404NotFound, context.GetTraceId())
        : NavyblueResults.Ok(order, traceId: context.GetTraceId());
});

app.Run();

public interface IOrderAppService
{
    OrderDto? Get(Guid id);
}

public sealed class OrderAppService : IOrderAppService, IScopedDependency
{
    public OrderDto? Get(Guid id) => new(id, "Paid", DateTimeOffset.UtcNow);
}

public sealed record OrderDto(Guid Id, string Status, DateTimeOffset PaidAt);
```

启动后可以直接访问：

```text
GET /_navyblue/ping
GET /_navyblue/info
GET /orders/{id}
```

## 基础扩展方法

```csharp
using Navyblue.BaseLibrary;
using Navyblue.BaseLibrary.Extensions;

var json = new { Id = 1, Name = "Navyblue" }.ToJson();
var model = json.FromJson<Dictionary<string, object>>();

var slug = "OrderDetailPage".ToKebabCase();        // order-detail-page
var field = "OrderDetailPage".ToSnakeCase();       // order_detail_page
var title = "  hello  ".NormalizeLineEndings();

var page = Enumerable.Range(1, 100).GetPage(pageIndex: 0, pageSize: 10);
var chunks = Enumerable.Range(1, 10).ChunkBy(3);

var cache = new Dictionary<string, int>();
var value = cache.GetOrAdd("retry", _ => 3);

Guid id = GuidUtility.NewSequentialGuid();
string idText = id.ToGuidString();
```

## Span / Memory / 高性能字符串处理

```csharp
using Navyblue.BaseLibrary.Extensions;
using System.Security.Cryptography;

ReadOnlySpan<char> text = "id,name,createdAt";

foreach (var part in text.Split(','))
{
    Console.WriteLine(part.ToString());
}

ReadOnlySpan<char> number = "12345";
if (number.TryParseInt32(out var parsed))
{
    Console.WriteLine(parsed);
}

ReadOnlySpan<byte> bytes = SHA256.HashData("hello"u8.ToArray());
string hex = bytes.ToHexStringLower();
string base64 = bytes.ToBase64String();

ReadOnlyMemory<byte> payload = "hello"u8.ToArray();
string body = payload.ToUtf8String();
```

## .NET 6-10 新增类型扩展

`Navyblue.BaseLibrary` 会按目标框架暴露对应能力。低版本不会看到高版本不可用的 API。

```csharp
using Navyblue.BaseLibrary.Extensions;

DateOnly today = DateOnly.FromDateTime(DateTime.Today);
DateOnly endOfMonth = today.EndOfMonth();
bool inQuarter = today.IsBetween(new DateOnly(2026, 1, 1), new DateOnly(2026, 3, 31));

TimeOnly start = new(22, 0);
TimeOnly end = new(2, 0);
bool inNightShift = new TimeOnly(23, 30).IsBetween(start, end);

var queue = new PriorityQueue<string, int>();
queue.EnqueueRange([
    ("send-email", 10),
    ("write-audit-log", 20)
]);
queue.TryDequeue(out var nextJob);
```

`net7.0+`：

```csharp
using Navyblue.BaseLibrary.Extensions;

int number = "42".ParseOrDefault<int>();
decimal amount = "19.90".ParseOrDefault<decimal>();

Int128 big = Int128.Parse("170141183460469231731687303715884105727");
string invariant = big.ToInvariantString();
```

`net8.0+`：

```csharp
using Navyblue.BaseLibrary.Extensions;

var separators = ",;|".ToSearchValues();
bool hasSeparator = "a,b".AsSpan().ContainsAny(separators);

var frozen = new Dictionary<string, int>
{
    ["read"] = 1,
    ["write"] = 2
}.ToFrozenDictionarySafe(StringComparer.OrdinalIgnoreCase);

await TimeProvider.System.DelayAsync(TimeSpan.FromMilliseconds(50));
```

## Guid v7、Base64Url 和现代加密

```csharp
using Navyblue.BaseLibrary.Extensions;
using System.Security.Cryptography;
using System.Text;

Guid id = ModernGuidV7.NewGuidV7();
bool isV7 = id.IsVersion7();
DateTimeOffset? createdAt = id.GetVersion7Timestamp();

byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
string token = tokenBytes.AsSpan().ToBase64UrlString();
byte[] restoredToken = token.FromBase64UrlString();

byte[] key = RandomNumberGenerator.GetBytes(32);
byte[] plaintext = Encoding.UTF8.GetBytes("hello navyblue");
AesGcmPayload encrypted = plaintext.AsSpan().EncryptAesGcm(key);
byte[] decrypted = encrypted.DecryptAesGcm(key);
```

`net8.0+` 可以使用 SHA-3 和 HMAC-SHA3。SHA-3 是否可用取决于运行平台，调用前可以检查 `ModernSha3Extensions.Sha3IsSupported`。

```csharp
using Navyblue.BaseLibrary.Extensions;
using System.Security.Cryptography;

byte[] payload = "hello"u8.ToArray();
byte[] key = RandomNumberGenerator.GetBytes(32);

if (ModernSha3Extensions.Sha3IsSupported)
{
    byte[] sha3 = payload.AsSpan().Sha3_256();
    byte[] mac = payload.AsSpan().HmacSha3_256(key);
}
```
## IO、URI、JSON、异步枚举扩展

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

List<int> values = await GetNumbersAsync().WhereAsync(x => x > 10).ToListAsync();

static async IAsyncEnumerable<int> GetNumbersAsync()
{
    for (var i = 0; i < 20; i++)
    {
        await Task.Yield();
        yield return i;
    }
}
```

## HTTP、表达式、树形结构和集合差异

```csharp
using Navyblue.BaseLibrary.Extensions;
using System.Linq.Expressions;
using System.Net.Http;

using var client = new HttpClient();
var order = await client.GetJsonOrDefaultAsync<OrderDto>("https://api.example.com/orders/1");

using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders")
    .WithJsonAccept()
    .WithBearerToken("access-token")
    .WithCorrelationId(Guid.NewGuid().ToString("N"));

Expression<Func<OrderDto, bool>> paid = x => x.Status == "Paid";
Expression<Func<OrderDto, bool>> recent = x => x.PaidAt > DateTimeOffset.UtcNow.AddDays(-30);
var predicate = paid.And(recent);

var diff = currentOrders.DiffBy(
    nextOrders,
    current => current.Id,
    next => next.Id);

var menuTree = menus.ToTree(
    idSelector: x => x.Id,
    parentIdSelector: x => x.ParentId);
```
## DDD 建模 Demo

```csharp
using Navyblue.BaseLibrary.Domain;
using Navyblue.BaseLibrary.Primitives;

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

public sealed record OrderPaid(Guid OrderId, Guid EventId, DateTimeOffset OccurredAt)
    : DomainEvent(EventId, OccurredAt)
{
    public OrderPaid(Guid orderId) : this(orderId, Guid.NewGuid(), DateTimeOffset.UtcNow)
    {
    }
}
```

## Result / API Result / 分页

```csharp
using Navyblue.BaseLibrary.Application;
using Navyblue.BaseLibrary.Primitives;

Result<Guid> createResult = Result.Success(Guid.NewGuid());

Result fail = Result.Failure(new Error("order.invalid_status", "Order status is invalid."));

var request = new PageRequest(PageIndex: 1, PageSize: 20).Normalize(maxPageSize: 100);
var items = new[]
{
    new OrderDto(Guid.NewGuid(), "Paid", DateTimeOffset.UtcNow)
};

PageResult<OrderDto> page = items.ToPageResult(total: 1, request);
ApiResult<PageResult<OrderDto>> response = ApiResult<PageResult<OrderDto>>.Success(page);
```

## 约定式依赖注入

实现 `IScopedDependency`、`ITransientDependency`、`ISingletonDependency` 后，可以按程序集自动注册。

```csharp
using Navyblue.BaseLibrary.DependencyInjection;

builder.Services.AddNavyblueCore(options =>
{
    options.WorkerId = 1;
    options.DataCenterId = 1;
});

builder.Services.AddNavyblueConventionalServicesFrom(typeof(Program).Assembly);

public interface IClockReader
{
    DateTimeOffset Now();
}

public sealed class ClockReader : IClockReader, IScopedDependency
{
    public DateTimeOffset Now() => DateTimeOffset.UtcNow;
}
```

## ASP.NET Core Web API 基础设施

```csharp
using Navyblue.BaseLibrary.AspNetCore;

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
    var user = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    var tenant = context.GetTenantId();
    return NavyblueResults.Ok(new { user, tenant }, traceId: context.GetTraceId());
});
```

内置能力包括：

- 全局异常到标准 `ApiResult` 的映射
- `X-Trace-Id` 透传和响应头输出
- 租户解析和请求上下文
- 安全响应头
- 请求日志和审计日志
- Minimal API 标准返回 `NavyblueResults`
- `/_navyblue/ping`、`/_navyblue/info` 健康检查和应用信息端点

## 推荐分层使用方式

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

## 测试辅助

```csharp
using Microsoft.Extensions.DependencyInjection;
using Navyblue.BaseLibrary.Application;
using Navyblue.BaseLibrary.Testing;

var services = new ServiceCollection();

var currentUser = new FakeCurrentUser
{
    UserId = "u-001",
    UserName = "Alice",
    IsAuthenticated = true
};
currentUser.RoleList.Add("admin");

services.AddSingleton<ICurrentUser>(currentUser);
services.AddSingleton<ICurrentTenant>(new FakeCurrentTenant
{
    TenantId = "tenant-001",
    TenantName = "Demo Tenant"
});

var principal = TestClaimsPrincipal.Create(
    userId: "u-001",
    userName: "Alice",
    roles: ["admin"],
    tenantId: "tenant-001");
```
## 设计原则

- 尽量少依赖外部包，优先使用 BCL 和 `Microsoft.Extensions.*`
- 扩展方法集中在 `Navyblue.BaseLibrary.Extensions`，Core/AspNetCore 通过引用原始库复用
- 多目标框架按能力开放 API，避免低版本框架引用不存在的类型
- DDD 和应用层基础设施只提供稳定抽象，不强绑定 ORM 或特定业务框架
- Web 层提供标准响应、上下文、异常、追踪、租户等通用能力，让页面和业务接口更简洁

## 构建

```bash
dotnet build Navyblue.BaseLibrary.sln
```
