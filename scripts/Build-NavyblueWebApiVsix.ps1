#Requires -Version 5.1
<#
.SYNOPSIS
  Packs the template nupkg and builds a Marketplace-valid VSIX using Visual Studio MSBuild + VSSDK.
#>
param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $PSScriptRoot
$Packages = Join-Path $Root "artifacts\packages"
$VsixDir = Join-Path $Root "vsix\Navyblue.Templates.WebApi.Vsix"
$VsixProj = Join-Path $VsixDir "Navyblue.Templates.WebApi.Vsix.csproj"
$VsixOut = Join-Path $Root "artifacts\vsix"
New-Item -ItemType Directory -Force -Path $Packages, $VsixOut | Out-Null

function Get-VSMSBuild {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"
    if (-not (Test-Path $vswhere)) { return $null }
    $install = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
    if (-not $install) { return $null }
    $msbuild = Join-Path $install "MSBuild\Current\Bin\MSBuild.exe"
    if (Test-Path $msbuild) { return $msbuild }
    return $null
}

Write-Host "==> Packing template nupkg" -ForegroundColor Cyan
dotnet pack (Join-Path $Root "templates\Navyblue.WebApi\Navyblue.Templates.WebApi.csproj") -c $Configuration --nologo
if ($LASTEXITCODE -ne 0) { throw "Template pack failed" }

$nupkg = Get-ChildItem $Packages -Filter "Navyblue.Templates.WebApi.*.nupkg" |
    Where-Object { $_.Name -notlike "*.symbols*" -and $_.Name -notlike "*.snupkg" } |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1
if (-not $nupkg) { throw "Template nupkg missing in $Packages" }

$embedded = Join-Path $VsixDir "Packages\Navyblue.Templates.WebApi.nupkg"
New-Item -ItemType Directory -Force -Path (Split-Path $embedded) | Out-Null
Copy-Item $nupkg.FullName $embedded -Force
Write-Host "==> Embedded $($nupkg.Name)" -ForegroundColor Cyan

$msbuild = Get-VSMSBuild
if (-not $msbuild) {
    throw "Visual Studio MSBuild not found. Install VS with 'Visual Studio extension development' workload, then retry."
}

Write-Host "==> Building VSIX with VS MSBuild + installed VSSDK:" -ForegroundColor Cyan
Write-Host "    $msbuild"
Write-Host "    (NuGet Microsoft.VSSDK.BuildTools is skipped: offreg.dll AccessDenied on this machine)"

& $msbuild $VsixProj /t:Restore,Rebuild /p:Configuration=$Configuration /p:DeployExtension=false /v:m /nologo
if ($LASTEXITCODE -ne 0) { throw "VSIX MSBuild failed" }

$built = Join-Path $VsixDir "bin\$Configuration\Navyblue.Templates.WebApi.Vsix.vsix"
if (-not (Test-Path $built)) {
    $built = Get-ChildItem $VsixDir -Filter "*.vsix" -Recurse |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1 -ExpandProperty FullName
}
if (-not $built -or -not (Test-Path $built)) { throw "No .vsix produced under $VsixDir" }

$dest = Join-Path $VsixOut "Navyblue.Templates.WebApi.Vsix.3.3.0.vsix"
Copy-Item $built $dest -Force

Write-Host ""
Write-Host "VSIX ready (Marketplace-compatible):" -ForegroundColor Green
Write-Host "  $dest"
Write-Host ""
Write-Host "Validate contents (must include manifest.json + catalog.json, forward-slash paths):"
Add-Type -AssemblyName System.IO.Compression.FileSystem
$z = [System.IO.Compression.ZipFile]::OpenRead($dest)
$names = @($z.Entries | ForEach-Object { $_.FullName })
foreach ($n in $names) { Write-Host ("  - " + $n) }
$z.Dispose()
if ($names -notcontains "manifest.json" -or $names -notcontains "catalog.json") {
    throw "VSIX missing manifest.json/catalog.json — Marketplace will reject this file."
}
if ($names | Where-Object { $_ -like "*\*" }) {
    throw "VSIX contains backslash entry paths — Marketplace will reject this file."
}
Write-Host ""
Write-Host "Upload to: https://marketplace.visualstudio.com/manage"
