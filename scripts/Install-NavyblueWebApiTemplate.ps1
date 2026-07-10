#Requires -Version 5.1
<#
.SYNOPSIS
  Packs Navyblue foundation packages + Web API template, registers a local NuGet feed, and installs the template for VS / dotnet new.
#>
param(
    [string]$Configuration = "Release",
    [switch]$SkipInstall,
    [switch]$FromContent
)

$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $PSScriptRoot
if (-not (Test-Path (Join-Path $Root "Navyblue.BaseLibrary.sln"))) {
    throw "Cannot locate Navyblue.BaseLibrary.sln from $PSScriptRoot"
}
$Packages = Join-Path $Root "artifacts\packages"
$Content = Join-Path $Root "templates\Navyblue.WebApi\content"
New-Item -ItemType Directory -Force -Path $Packages | Out-Null

Write-Host "==> Packing Navyblue foundation packages to $Packages" -ForegroundColor Cyan
$projects = @(
    "Navyblue.BaseLibrary\Navyblue.BaseLibrary.csproj",
    "src\Navyblue.Foundation\Navyblue.Foundation.csproj",
    "src\Navyblue.Foundation.Cqrs\Navyblue.Foundation.Cqrs.csproj",
    "src\Navyblue.Foundation.AspNetCore\Navyblue.Foundation.AspNetCore.csproj",
    "src\Navyblue.Foundation.Testing\Navyblue.Foundation.Testing.csproj"
)
foreach ($p in $projects) {
    $path = Join-Path $Root $p
    if (-not (Test-Path $path)) { Write-Warning "Skip missing $p"; continue }
    dotnet pack $path -c $Configuration -o $Packages --nologo
    if ($LASTEXITCODE -ne 0) { throw "dotnet pack failed: $p" }
}

Write-Host "==> Packing Web API template nupkg" -ForegroundColor Cyan
$templateProj = Join-Path $Root "templates\Navyblue.WebApi\Navyblue.Templates.WebApi.csproj"
dotnet pack $templateProj -c $Configuration -o $Packages --nologo
if ($LASTEXITCODE -ne 0) { throw "dotnet pack failed: template" }

$sourceName = "navyblue-local"
$sources = dotnet nuget list source 2>$null | Out-String
if ($sources -match $sourceName) {
    Write-Host "==> Updating NuGet source $sourceName" -ForegroundColor Cyan
    dotnet nuget remove source $sourceName 2>$null | Out-Null
}
dotnet nuget add source $Packages -n $sourceName
if ($LASTEXITCODE -ne 0) { throw "Failed to add NuGet source" }

if (-not $SkipInstall) {
    Write-Host "==> Installing template (navyblue-webapi)" -ForegroundColor Cyan
    dotnet new uninstall Navyblue.Templates.WebApi 2>$null | Out-Null
    dotnet new uninstall Navyblue.Templates 2>$null | Out-Null
    dotnet new uninstall $Content 2>$null | Out-Null

    if ($FromContent) {
        dotnet new install $Content --force
    } else {
        $nupkg = Get-ChildItem $Packages -Filter "Navyblue.Templates.WebApi.*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
        if (-not $nupkg) { throw "Template nupkg not found in $Packages" }
        dotnet new install $nupkg.FullName --force
    }
    if ($LASTEXITCODE -ne 0) { throw "dotnet new install failed" }
}

Write-Host ""
Write-Host "Done." -ForegroundColor Green
Write-Host "  CLI:  dotnet new navyblue-webapi -n Contoso.Catalog -o Contoso.Catalog"
Write-Host "  VS:   File > New > Project > search 'Navyblue ASP.NET Core Web API'"
Write-Host "  Feed: $Packages  (source name: navyblue-local)"
