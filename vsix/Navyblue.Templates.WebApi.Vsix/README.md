# Navyblue.Templates.WebApi.Vsix

Visual Studio 2022+ extension that installs the **Navyblue ASP.NET Core Web API** project template
(`dotnet new navyblue-webapi`) when the extension loads.

## Build

Requires **Visual Studio** with the **Visual Studio extension development** workload
(uses the installed VSSDK; NuGet `Microsoft.VSSDK.BuildTools` is not used because restore
can fail with `offreg.dll` AccessDenied).

From the repository root:

```powershell
./scripts/Build-NavyblueWebApiVsix.ps1
```

Output:

```text
artifacts/vsix/Navyblue.Templates.WebApi.Vsix.3.2.0.vsix
```

The script validates that the package contains `manifest.json` and `catalog.json`
(required by Visual Studio Marketplace).
## Install locally

1. Double-click the `.vsix`, **or**
2. Visual Studio → **Extensions** → **Manage Extensions** → **⋯** → **Install from VSIX…**
3. Restart Visual Studio when prompted
4. **File → New → Project** → search **Navyblue ASP.NET Core Web API**

On first load the extension runs `dotnet new install` against the embedded template nupkg.
Check **View → Output → Navyblue Templates** for status.

## Publish to Visual Studio Marketplace

1. Register a publisher: https://marketplace.visualstudio.com/manage  
2. **New extension** → **Visual Studio** → upload `Navyblue.Templates.WebApi.Vsix.3.2.0.vsix`  
3. Fill overview, categories (`Templates`, `Coding`), and support links  
4. Submit for review  

Users then install via **Extensions → Manage Extensions → Online** by searching **Navyblue**.

## Prerequisites for generated projects

Generated projects use **EF Core + Pomelo + MySQL 8.0**, **Redis** cache, and reference `Navyblue.Foundation*` **3.0.0**.
Configure `ConnectionStrings:Default` and `Navyblue:Redis`, and ensure Foundation packages resolve from nuget.org
or a local feed (see `scripts/Install-NavyblueWebApiTemplate.ps1`).

## Uninstall

1. VS → Extensions → Manage Extensions → Installed → remove **Navyblue ASP.NET Core Web API Templates**
2. Optionally: `dotnet new uninstall Navyblue.Templates.WebApi`
