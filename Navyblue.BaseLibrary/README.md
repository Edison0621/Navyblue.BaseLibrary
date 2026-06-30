# Navyblue.BaseLibrary

Base package for common .NET utilities and extensions.

Target frameworks:

```text
net6.0;net7.0;net8.0;net9.0;net10.0
```

Typical usage:

```csharp
using Navyblue.BaseLibrary;
using Navyblue.BaseLibrary.Extensions;

var json = new { Id = 1, Name = "Navyblue" }.ToJson();
var model = json.FromJson<Dictionary<string, object>>();

var slug = "OrderDetailPage".ToKebabCase();
var endOfMonth = DateOnly.FromDateTime(DateTime.Today).EndOfMonth();
var traceBytes = "hello"u8.ToArray().AsSpan().Sha256();
```

For full demos covering Core, DDD, ASP.NET Core and Testing scenarios, see the repository root `README.md`.
