# Navyblue 企业级基础库架构设计（合并版）

## 1. 合并后的包结构

为了降低使用成本，vNext 新增项目收敛为 3 个包，原有 `Navyblue.BaseLibrary` 继续作为兼容包保留不改：

| 项目 | 职责 | 典型引用方 |
| --- | --- | --- |
| Navyblue.BaseLibrary | 原有兼容包，保留历史 API | 已有项目 |
| Navyblue.BaseLibrary.Core | 企业基础能力核心包：DDD 实体、审计、软删、多租户、Result、ApiResult、异常、仓储抽象、缓存抽象、分布式锁抽象、幂等、事件、Outbox、Options、诊断、扩展方法、安全、JSON、校验、HTTP、模块化注册 | 绝大多数业务项目 |
| Navyblue.BaseLibrary.AspNetCore | ASP.NET Core 接入层：全局异常、TraceId、请求日志、当前用户/租户解析、`AddNavyblueFramework()`、`UseNavyblueFramework()` | Web API 项目 |
| Navyblue.BaseLibrary.Testing | 测试辅助：测试用户、ClaimsPrincipal、TestClock | 单元测试/集成测试项目 |

这个结构比“每个能力一个项目”更适合企业内部基础库的第一阶段：开发者心智负担更低，业务项目引用更简单，同时仍然通过命名空间和接口保持模块边界。

## 2. 设计原则

- 不修改原有 `Navyblue.BaseLibrary` 源码，避免破坏兼容性。
- 新能力集中在 `Core`，但不设计万能 `BaseEntity`。
- Domain、Application、Data、Caching、Events 等能力通过命名空间隔离。
- EF Core、FreeSql、Redis、RabbitMQ、Kafka、OpenTelemetry、Excel 等重依赖暂不进入核心实现，只保留抽象或后续做适配包。
- Web 能力放入 `AspNetCore`，避免非 Web 项目被迫引用 ASP.NET Core。
- 测试辅助独立成 `Testing`，避免生产包携带测试语义。

## 3. Core 内部能力

`Navyblue.BaseLibrary.Core` 当前包含：

- `Navyblue.BaseLibrary.Primitives`：`Result`、`PagedResult`、`Guard`、`SequentialGuid`
- `Navyblue.BaseLibrary.Domain`：`IEntity`、`AggregateRoot`、审计、软删、多租户、领域事件、企业异常
- `Navyblue.BaseLibrary.Application`：`ApiResult`、`ApiResult<T>`、`PageResult<T>`、`BusinessCode`、`ICurrentUser`、`ICurrentTenant`
- `Navyblue.BaseLibrary.Data`：`IRepository`、`IUnitOfWork`、`IDataFilter`
- `Navyblue.BaseLibrary.Caching`：`ICacheProvider`、`IDistributedCacheProvider`、`CacheKeyBuilder`
- `Navyblue.BaseLibrary.Locking`：`IDistributedLock`、`IDistributedLockProvider`
- `Navyblue.BaseLibrary.Idempotency`：`IdempotencyKey`、`IIdempotencyStore`
- `Navyblue.BaseLibrary.Events`：集成事件、事件总线、消息发布消费、Outbox 抽象
- `Navyblue.BaseLibrary.Configuration`：框架、Redis、消息、审计、多租户、ID 生成配置
- `Navyblue.BaseLibrary.Diagnostics`：`CorrelationContext`、`OperationTimer`
- `Navyblue.BaseLibrary.Extensions`：字符串、集合、日期、Guid、枚举扩展
- `Navyblue.BaseLibrary.Security`：Hash、HMAC、Base64Url、密码哈希
- `Navyblue.BaseLibrary.Serialization`：JSON 扩展
- `Navyblue.BaseLibrary.Validation`：常用校验规则
- `Navyblue.BaseLibrary.Modularity`：模块化注册
- `Navyblue.BaseLibrary.Http`：`IHttpClientService` 与轻量重试

## 4. 业务接入示例

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNavyblueFramework(options =>
{
    options.EnableExceptionHandling = true;
    options.EnableTraceId = true;
    options.EnableRequestLogging = true;
});

builder.Services.AddNavyblueHttp(options =>
{
    options.Timeout = TimeSpan.FromSeconds(10);
    options.RetryCount = 2;
});

var app = builder.Build();
app.UseNavyblueFramework();
app.MapControllers();
app.Run();
```

普通类库或控制台项目只引用 `Navyblue.BaseLibrary.Core`。Web API 项目引用 `Navyblue.BaseLibrary.AspNetCore`，它会传递引用 Core。测试项目按需引用 `Navyblue.BaseLibrary.Testing`。

## 5. 后续演进

先保持少项目结构，等某类能力确实引入重依赖或出现替换实现时再拆适配包：

- `Navyblue.BaseLibrary.EntityFrameworkCore`
- `Navyblue.BaseLibrary.FreeSql`
- `Navyblue.BaseLibrary.Redis`
- `Navyblue.BaseLibrary.RabbitMQ`
- `Navyblue.BaseLibrary.Kafka`
- `Navyblue.BaseLibrary.OpenTelemetry`
- `Navyblue.BaseLibrary.Excel`

拆分判断标准：只要会引入重依赖、外部服务 SDK、运行时中间件或明显增加包体积，就不要塞进 Core。

## 6. 本轮已增强能力

本轮在保持 3 个新增包结构不变的前提下，继续丰富了基础库能力：

### Core 新增

- `Maybe<T>`：表达可选值，避免用 `null` 表达业务缺失。
- `IIdGenerator<long>` / `SnowflakeIdGenerator`：轻量雪花 ID 生成器。
- `ValueObject`：DDD 值对象基类。
- `DateRange` / `Money`：常见业务值对象。
- `IClock` / `SystemClock`：统一时间来源，便于测试替换。
- `IAuditor` / `ITenantResolver` / `IAuditPropertySetter`：审计和租户抽象。
- `ISpecification<T>` / `Specification<T>` / `SpecificationEvaluator`：查询规约抽象，不绑定 EF Core。
- `CachePolicy` / `ICacheSerializer` / `GetOrCreateNullableAsync`：缓存 Aside 和 Null 值缓存策略。
- `IdempotencyResult` / `IIdempotencyKeyProvider`：幂等处理结果和 Key 来源抽象。
- `LocalDomainEventDispatcher` / `InMemoryEventBus`：本地事件调度和内存集成事件总线。
- `AddNavyblueCore()`：核心服务统一注册入口。
- `FileNameSanitizer` / `FileSizeFormatter` / `StreamUtilities` / `MimeTypeMap`：文件和流处理工具。

### AspNetCore 新增

- `AddNavyblueApiBehavior()`：统一模型验证失败响应，输出 `ApiResult`。
- `HttpContextExtensions`：TraceId 和 TenantId 读取辅助。

### Testing 新增

- `FakeCurrentUser`
- `FakeCurrentTenant`

这些新增能力仍然遵循低耦合原则：核心包不绑定 EF Core、Redis、RabbitMQ、Kafka 等重依赖，后续需要时再做适配包。

## 7. DDD 能力增强

针对 DDD 过于单薄的问题，当前 `Navyblue.BaseLibrary.Core` 已补充以下能力：

- 实体身份相等性：`Entity<TKey>` 基于类型和非临时 ID 做相等性判断，避免引用相等导致领域对象比较失真。
- 聚合根版本：`AggregateRoot<TKey>` 实现 `IHasVersion`，为乐观并发、事件溯源和 Outbox 事件版本预留基础。
- 审计聚合根：`FullAuditedAggregateRoot<TKey>` 支持完整审计、软删和并发戳。
- 领域事件封装：`DomainEventEnvelope` 包含聚合类型、聚合 ID、聚合版本、CorrelationId、CausationId。
- 聚合事件收集：`AggregateEventCollector` 可从聚合或实体集合中收集并清理领域事件。
- 业务规则：`IBusinessRule`、`BusinessRule`、`CheckRule`、`DelegateBusinessRule`，用于在领域对象内部表达不变量。
- 领域规则异常：`DomainRuleViolationException`，区分普通业务异常和领域不变量破坏。
- 强类型 ID：`StronglyTypedId<TValue>`、`GuidStronglyTypedId`、`LongStronglyTypedId`，减少不同聚合 ID 混用。
- 领域服务：`IDomainService`、`DomainService`，用于承载跨聚合但仍属于领域层的行为。
- 领域策略：`IDomainPolicy<TContext>`，适合表达可替换的业务策略。
- 聚合仓储语义：`IAggregateRepository<TAggregate, TKey>`，比通用 Repository 更明确地面向聚合根。
- 领域事件 UnitOfWork：`IDomainEventUnitOfWork`，为 ORM 适配包统一提交后分发事件预留契约。
- DI 扫描：`AddDomainServicesFrom()`、`AddDomainEventHandlersFrom()`，减少业务项目手工注册。
- 领域模型扩展：软删标记、恢复、领域事件读取等辅助方法。

这些增强仍然不绑定 EF Core、FreeSql、Redis 或消息队列；基础库只提供 DDD 建模和扩展点，基础设施实现后续通过适配包接入。

## 8. 扩展方法增强

当前 `Navyblue.BaseLibrary.Core` 已增强扩展方法体系，重点覆盖现代 .NET 常用类型：

### ReadOnlySpan / ReadOnlyMemory

- `ReadOnlySpan<char>.IsWhiteSpace()` / `IsNullOrWhiteSpace()`
- `EqualsOrdinal()` / `EqualsOrdinalIgnoreCase()`
- `TryParseInt32()` / `TryParseInt64()` / `TryParseDecimal()` / `TryParseGuid()`
- `Split(char, StringSplitOptions)`：返回 `SpanSplitEnumerator`，支持 `foreach` 且避免字符串分配
- `EnumerateLines()`：返回 `SpanLineEnumerator`，支持按行读取 span
- `ReadOnlySpan<byte>.ToUtf8String()`
- `ToHexStringLower()` / `ToHexStringUpper()` / `ToBase64String()`
- `FixedTimeEquals()`：常量时间比较，适合签名、Token、Hash 比较
- `Sha256()` / `HmacSha256()`
- `ReadOnlyMemory<byte>.ToUtf8String()` / `ToArraySafe()` / `ToReadOnlyStream()`

### 字符串

- `IsNotNullOrWhiteSpace()`
- `AsReadOnlySpan()`
- `ToPascalCase()` / `ToCamelCase()`
- `ToSnakeCase()` / `ToKebabCase()`
- `NormalizeLineEndings()`
- `RemoveWhiteSpace()`
- `EnsureStartsWith()` / `EnsureEndsWith()`
- `Truncate(maxLength, suffix)`

### 集合与字典

- `WhereNotNull()`
- `ChunkBy()`
- `ToHashSet(comparer)`
- `Dictionary.GetOrAdd()`
- `GetValueOrDefault()`
- `TryRemove()`
- `Merge()`

### Task / Type / Claims

- `Task.WithTimeout()` / `Task<T>.WaitAsync(timeout)`
- `IgnoreExceptionAsync()`
- `WhenAll()`
- `Type.IsNullableType()` / `UnwrapNullableType()`
- `IsAssignableToGenericType()`
- `GetFriendlyName()`
- `Assembly.GetConcreteTypesAssignableTo<T>()`
- `ClaimsPrincipal.GetUserId()` / `GetUserName()` / `GetTenantId()` / `GetRoles()` / `HasClaimValue()`

这些扩展方法都保持零第三方依赖，优先使用 .NET 8/10 BCL 能力，并避免把 ASP.NET Core 专属 API 下沉到 Core。
