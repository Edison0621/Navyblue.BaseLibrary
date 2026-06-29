# Masuit.Tools to Navyblue.BaseLibrary Migration Plan

Goal: migrate selected high-value Masuit.Tools capabilities into Navyblue.BaseLibrary vNext while preserving all existing Navyblue.BaseLibrary source code unchanged.

## Rules

1. Do not modify, delete, rename, or move existing Navyblue.BaseLibrary source files.
2. Add new vNext projects under `src/`.
3. Keep the existing `Navyblue.BaseLibrary` package available as-is during migration.
4. Prefer BCL implementations over third-party packages.
5. Put heavy or platform-specific features in optional packages.
6. Re-implement Masuit features in Navyblue style instead of copying legacy structure wholesale.

## Initial vNext Projects

- `Navyblue.BaseLibrary.Primitives`: Guard, Result, Error, PagedResult, SequentialGuid.
- `Navyblue.BaseLibrary.Security`: Hashing, HMAC, PBKDF2 password hashing, Hex, Base64Url.

## Candidate Masuit Features

### Phase 1: Zero-dependency foundation

- Guard / Ensure improvements.
- Result / Error / Maybe.
- PagedResult / pagination helpers.
- Sequential GUID.
- Hashing, HMAC, PBKDF2.
- CRC32 / CRC64.
- Common string and enumerable helpers.

### Phase 2: Low-dependency domain modules

- Validation attributes and validation helpers.
- MIME mapping.
- File signature detection.
- INI file parsing.
- Text diff and SimHash.
- Weighted random selector.
- Snowflake IDs.
- Tree helpers.

### Phase 3: Optional integration modules

- ASP.NET Core resume file results.
- Excel import/export.
- Image processing.
- HTML parsing/sanitization.
- Windows-only hardware and Win32 helpers.
- MongoDB helpers.

## Dependency Boundary

Core vNext packages must stay zero-dependency. Optional package names should expose their external dependency or platform clearly, for example:

- `Navyblue.BaseLibrary.Html.AngleSharp`
- `Navyblue.BaseLibrary.Media.ImageSharp`
- `Navyblue.BaseLibrary.Excel.EPPlus`
- `Navyblue.BaseLibrary.Windows`

## First Implementation Slice

1. Add `Navyblue.BaseLibrary.Primitives`.
2. Add `Navyblue.BaseLibrary.Security`.
3. Build both projects independently.
4. Do not modify the existing `Navyblue.BaseLibrary` project.
5. Add tests in a later slice once the vNext API shape is accepted.