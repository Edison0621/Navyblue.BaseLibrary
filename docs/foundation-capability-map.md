# Navyblue.BaseLibrary vNext Capability Map

The vNext foundation is organized around common software development workflows while preserving the existing `Navyblue.BaseLibrary` project unchanged.

## Workflow Coverage

| Workflow stage | vNext package | Scope |
| --- | --- | --- |
| API input and domain modeling | `Navyblue.BaseLibrary.Primitives` | Guard, Result, Error, PagedResult, SequentialGuid |
| Everyday coding | `Navyblue.BaseLibrary.Extensions` | String, enumerable, number, date/time, GUID helpers |
| Data exchange | `Navyblue.BaseLibrary.Serialization` | System.Text.Json helpers with AOT-friendly overloads |
| Security baseline | `Navyblue.BaseLibrary.Security` | Hashing, HMAC, PBKDF2, Hex, Base64Url |
| Input validation | `Navyblue.BaseLibrary.Validation` | Validation helpers and DataAnnotations attributes |
| Collection modeling | `Navyblue.BaseLibrary.Collections` | Weighted selection and collection result helpers |
| Observability context | `Navyblue.BaseLibrary.Diagnostics` | Correlation context and operation timing |
| Resilience | `Navyblue.BaseLibrary.Resilience` | Lightweight retry helpers |

## Migration Strategy

- Existing source code stays in place and unchanged.
- New capabilities are added under `src/`.
- Modules stay zero-dependency until a capability explicitly requires an integration package.
- Heavy integrations such as ASP.NET Core, Excel, ImageSharp, MongoDB, Windows APIs, or HTML parsers should be added as separate optional packages.