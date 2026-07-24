[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$OutputPath = "artifacts/packages",
    [string]$Version = "",
    [string]$PrereleaseSuffix = "",
    [switch]$NoBuild,
    [switch]$NoRestore
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$sourceRoot = Join-Path $repositoryRoot "src"
$outputDirectory = if ([System.IO.Path]::IsPathRooted($OutputPath))
{
    [System.IO.Path]::GetFullPath($OutputPath)
}
else
{
    [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot $OutputPath))
}

if ($outputDirectory.TrimEnd("\", "/") -eq $repositoryRoot.TrimEnd("\", "/"))
{
    throw "The package output directory cannot be the repository root."
}

New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null

# Keep the upload folder deterministic without touching unrelated artifacts.
Get-ChildItem -LiteralPath $outputDirectory -File |
    Where-Object { $_.Name -match '^CanDoItAll\.Components\..+\.(?:nupkg|snupkg)$' } |
    Remove-Item -Force

$packableProjects = @(
    Get-ChildItem -LiteralPath $sourceRoot -Filter "*.csproj" -Recurse |
        Sort-Object FullName |
        Where-Object {
            [xml]$project = Get-Content -LiteralPath $_.FullName -Raw
            $isPackable = @($project.Project.PropertyGroup.IsPackable) |
                Where-Object { $_ -eq "true" } |
                Select-Object -First 1
            $isPackable -eq "true"
        }
)

if ($packableProjects.Count -eq 0)
{
    throw "No packable projects were found under $sourceRoot."
}

foreach ($projectFile in $packableProjects)
{
    [xml]$project = Get-Content -LiteralPath $projectFile.FullName -Raw
    $description = @($project.Project.PropertyGroup.Description) |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
        Select-Object -First 1
    $readmePath = Join-Path $projectFile.DirectoryName "README.md"

    if ([string]::IsNullOrWhiteSpace($description))
    {
        throw "$($projectFile.Name) must define a package Description."
    }

    if (-not (Test-Path -LiteralPath $readmePath -PathType Leaf))
    {
        throw "$($projectFile.Name) must have a README.md beside the project file."
    }

    $packArguments = @(
        "pack",
        $projectFile.FullName,
        "--configuration",
        $Configuration,
        "--output",
        $outputDirectory
    )

    if ($NoBuild)
    {
        $packArguments += "--no-build"
    }

    if ($NoRestore)
    {
        $packArguments += "--no-restore"
    }

    if (-not [string]::IsNullOrWhiteSpace($Version))
    {
        $packArguments += "-p:CanDoItAllPackageBaseVersion=$Version"
    }

    if (-not [string]::IsNullOrWhiteSpace($PrereleaseSuffix))
    {
        $packArguments += "-p:CanDoItAllPackageProofSuffix=$PrereleaseSuffix"
    }

    Write-Host "Packing $($projectFile.BaseName)..."
    & dotnet @packArguments
    if ($LASTEXITCODE -ne 0)
    {
        exit $LASTEXITCODE
    }
}

$packages = @(Get-ChildItem -LiteralPath $outputDirectory -Filter "*.nupkg" -File)
$symbolPackages = @(Get-ChildItem -LiteralPath $outputDirectory -Filter "*.snupkg" -File)

if ($packages.Count -ne $packableProjects.Count)
{
    throw "Expected $($packableProjects.Count) NuGet packages, but found $($packages.Count) in $outputDirectory."
}

if ($symbolPackages.Count -ne $packableProjects.Count)
{
    throw "Expected $($packableProjects.Count) symbol packages, but found $($symbolPackages.Count) in $outputDirectory."
}

Write-Host ""
Write-Host "Packed $($packages.Count) libraries and $($symbolPackages.Count) symbol packages into:"
Write-Host $outputDirectory
