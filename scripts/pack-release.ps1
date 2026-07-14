param(
    [string]$OutputPath = "artifacts/packages",
    [switch]$NoBuild
)

$ErrorActionPreference = "Stop"

$projects = @(
    "src/CanDoItAll.Components.Common/CanDoItAll.Components.Common.csproj",
    "src/CanDoItAll.Components.BaseLib/CanDoItAll.Components.BaseLib.csproj",
    "src/CanDoItAll.Components.CanvasLib/CanDoItAll.Components.CanvasLib.csproj",
    "src/CanDoItAll.Components.Gantt/CanDoItAll.Components.Gantt.csproj",
    "src/CanDoItAll.Components.Charts/CanDoItAll.Components.Charts.csproj",
    "src/CanDoItAll.Components.Mermaid/CanDoItAll.Components.Mermaid.csproj",
    "src/CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj",
    "src/CanDoItAll.Components.QRCode/CanDoItAll.Components.QRCode.csproj",
    "samples/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj",
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
    $projectArguments = @($arguments)
    if ($project -eq "samples/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj")
    {
        $projectArguments += "-p:IsPackable=true"
    }

    & dotnet pack $project @projectArguments
    if ($LASTEXITCODE -ne 0)
    {
        exit $LASTEXITCODE
    }
}
