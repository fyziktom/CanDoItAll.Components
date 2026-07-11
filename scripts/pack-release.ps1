param(
    [string]$OutputPath = "artifacts/packages",
    [switch]$NoBuild
)

$ErrorActionPreference = "Stop"

$projects = @(
    "src/CanDoItAll.Components.Common/CanDoItAll.Components.Common.csproj",
    "src/CanDoItAll.Components.BaseLib/CanDoItAll.Components.BaseLib.csproj",
    "src/CanDoItAll.Components.CanvasLib/CanDoItAll.Components.CanvasLib.csproj",
    "src/CanDoItAll.Components.Charts/CanDoItAll.Components.Charts.csproj",
    "src/CanDoItAll.Components.FileBrowser.Core/CanDoItAll.Components.FileBrowser.Core.csproj",
    "src/CanDoItAll.Components.FileBrowser.Providers.FileSystem/CanDoItAll.Components.FileBrowser.Providers.FileSystem.csproj",
    "src/CanDoItAll.Components.FileBrowser.BaseLib/CanDoItAll.Components.FileBrowser.BaseLib.csproj",
    "src/CanDoItAll.Components.Mermaid/CanDoItAll.Components.Mermaid.csproj",
    "src/CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj",
    "src/CanDoItAll.Components.QRCode/CanDoItAll.Components.QRCode.csproj",
    "src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj",
    "src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj"
)

$arguments = @("--configuration", "Release", "--output", $OutputPath)
if ($NoBuild)
{
    $arguments += "--no-build"
}

foreach ($project in $projects)
{
    & dotnet pack $project @arguments
    if ($LASTEXITCODE -ne 0)
    {
        exit $LASTEXITCODE
    }
}

$packageVersion = (& dotnet msbuild $projects[4] -getProperty:PackageVersion -nologo).Trim()
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($packageVersion))
{
    throw "Could not resolve the File Browser package version for archive validation."
}

& "$PSScriptRoot/validate-file-browser-packages.ps1" `
    -PackagePath $OutputPath `
    -PackageVersion $packageVersion
