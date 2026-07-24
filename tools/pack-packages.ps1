<#
.SYNOPSIS
Packs all publishable component projects into one folder.

.DESCRIPTION
By default, the package version comes from CanDoItAllPackageBaseVersion in
Directory.Build.props. Use -Version to override that base version for this
invocation without modifying the repository. Use -PrereleaseSuffix to append
a suffix such as "-preview.1". The script rebuilds the Tailwind assets before
packing, then creates a versioned and timestamped folder for the run.

.PARAMETER OutputPath
Parent directory for package runs. Each invocation creates a child directory
named from the effective package version and the local date and time.

.PARAMETER Version
Temporarily overrides the shared base package version from
Directory.Build.props for every package.

.PARAMETER PrereleaseSuffix
Appends a prerelease suffix to the base version. The value must start with "-".

.EXAMPLE
.\tools\pack-packages.ps1

Packs every library using the committed version in Directory.Build.props.

.EXAMPLE
.\tools\pack-packages.ps1 -Version "0.2.0" -PrereleaseSuffix "-preview.1"

Packs every library as version 0.2.0-preview.1 without editing
Directory.Build.props.
#>
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
$directoryBuildPropsPath = Join-Path $repositoryRoot "Directory.Build.props"
$outputRootDirectory = if ([System.IO.Path]::IsPathRooted($OutputPath))
{
    [System.IO.Path]::GetFullPath($OutputPath)
}
else
{
    [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot $OutputPath))
}

if ($outputRootDirectory.TrimEnd("\", "/") -eq $repositoryRoot.TrimEnd("\", "/"))
{
    throw "The package output root cannot be the repository root."
}

[xml]$directoryBuildProps = Get-Content -LiteralPath $directoryBuildPropsPath -Raw
$committedBaseVersionElement = @($directoryBuildProps.Project.PropertyGroup.CanDoItAllPackageBaseVersion) |
    Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
    Select-Object -First 1
$committedBaseVersion = if ($null -eq $committedBaseVersionElement)
{
    ""
}
else
{
    $committedBaseVersionElement.InnerText.Trim()
}

if ([string]::IsNullOrWhiteSpace($committedBaseVersion))
{
    throw "CanDoItAllPackageBaseVersion must be defined in $directoryBuildPropsPath."
}

if (-not [string]::IsNullOrWhiteSpace($PrereleaseSuffix) -and
    -not $PrereleaseSuffix.StartsWith("-", [StringComparison]::Ordinal))
{
    throw "PrereleaseSuffix must start with '-', for example '-preview.1'."
}

$effectiveBaseVersion = if ([string]::IsNullOrWhiteSpace($Version))
{
    $committedBaseVersion
}
else
{
    $Version
}
$effectiveVersion = "$effectiveBaseVersion$PrereleaseSuffix"
$versionSource = if ([string]::IsNullOrWhiteSpace($Version))
{
    "Directory.Build.props (CanDoItAllPackageBaseVersion)"
}
else
{
    "the -Version command-line override"
}

Write-Host "Package version: $effectiveVersion"
Write-Host "Version source: $versionSource"
Write-Host ""

Write-Host "Building Tailwind assets..."
$tailwindExitCode = 0
Push-Location $repositoryRoot
try
{
    & npm run tailwind:build
    $tailwindExitCode = $LASTEXITCODE
}
finally
{
    Pop-Location
}

if ($tailwindExitCode -ne 0)
{
    exit $tailwindExitCode
}

$runTimestamp = Get-Date -Format "yyyyMMdd-HHmmssfff"
$runDirectoryName = "${effectiveVersion}_$runTimestamp"
$outputDirectory = Join-Path $outputRootDirectory $runDirectoryName

New-Item -ItemType Directory -Force -Path $outputRootDirectory | Out-Null
New-Item -ItemType Directory -Path $outputDirectory | Out-Null

Write-Host ""
Write-Host "Package run folder: $outputDirectory"
Write-Host ""

$projectFiles = @(
    Get-ChildItem -LiteralPath $sourceRoot -Filter "*.csproj" -Recurse |
        Sort-Object FullName
)
$packableProjects = @(
    foreach ($candidateProjectFile in $projectFiles)
    {
        [xml]$candidateProjectXml = Get-Content -LiteralPath $candidateProjectFile.FullName -Raw
        $isPackable = @($candidateProjectXml.Project.PropertyGroup.IsPackable) |
            Where-Object { $_ -eq "true" } |
            Select-Object -First 1

        if ($isPackable -eq "true")
        {
            $candidateProjectFile
        }
    }
)

if ($packableProjects.Count -eq 0)
{
    throw "No packable projects were found under $sourceRoot."
}

foreach ($projectFile in $packableProjects)
{
    [xml]$projectXml = Get-Content -LiteralPath $projectFile.FullName -Raw
    $description = @($projectXml.Project.PropertyGroup.Description) |
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
