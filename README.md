# Navyblue.BaseLibrary

Navyblue.BaseLibrary is a modern .NET foundation library for enterprise applications. It provides common extensions, modern BCL helpers, DDD primitives, application result models, ASP.NET Core infrastructure, and testing helpers while keeping external dependencies low.

Navyblue.BaseLibrary 是一个面向 .NET 企业应用的现代化基础库，覆盖常用扩展方法、现代 BCL 类型、DDD 基础建模、应用层返回模型、ASP.NET Core 基础设施和测试辅助，同时尽量减少外部依赖。

## Documentation / 文档

- [中文文档](docs/Navyblue.BaseLibrary.zh-CN.md)
- [English Documentation](docs/Navyblue.BaseLibrary.en-US.md)

## Packages

| Package | Target frameworks | Purpose |
| --- | --- | --- |
| `Navyblue.BaseLibrary` | `net6.0;net7.0;net8.0;net9.0;net10.0` | Common extensions, JSON, Guid, hash, Span/Memory, stream, URI, HTTP, modern BCL helpers |
| `Navyblue.BaseLibrary.Core` | `net8.0;net10.0` | DDD, Result, paging, events, caching, idempotency, locks, diagnostics, DI |
| `Navyblue.BaseLibrary.AspNetCore` | `net8.0;net10.0` | Standard Web API responses, exception mapping, TraceId, tenant context, audit, security headers, Minimal API helpers |
| `Navyblue.BaseLibrary.Testing` | `net8.0;net10.0` | Fake current user, fake tenant, test clock, claims principal, in-memory domain events |

## Quick Start / 快速开始

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

## Build / 构建

```bash
dotnet build Navyblue.BaseLibrary.sln
```