<#
.SYNOPSIS
Builds and packs every publishable Components project.

.DESCRIPTION
This is the repository-owned adapter for the CanDoItAll shared NuGet packaging
contract. It builds the generated Tailwind assets, restores unless -NoRestore
is supplied, builds and tests unless -NoBuild is supplied, and packs every
project below src that explicitly sets IsPackable to true.

When -OutputDirectory is omitted, each invocation creates a versioned,
timestamped child below artifacts/packages. When it is supplied, packages are
written directly to that exact directory so cross-repository orchestration can
isolate the output.

.PARAMETER Configuration
Build configuration. The default is Release.

.PARAMETER OutputDirectory
Absolute or repository-relative package destination.

.PARAMETER NoRestore
Skips restore when the caller guarantees it has already completed.

.PARAMETER NoBuild
Skips the build and test gates when the caller guarantees they have already
completed. Packing still uses --no-build.

.PARAMETER Version
Temporarily overrides CanDoItAllPackageBaseVersion without editing the
repository.

.PARAMETER PrereleaseSuffix
Appends a prerelease suffix, including its leading hyphen, to the base version.

.EXAMPLE
.\tools\deployment\nugets\Build-NuGets.ps1

.EXAMPLE
.\tools\deployment\nugets\Build-NuGets.ps1 -Version '0.2.0' -PrereleaseSuffix '-preview.1'

.EXAMPLE
.\tools\deployment\nugets\Build-NuGets.ps1 -OutputDirectory C:\packages\components
#>
[CmdletBinding(SupportsShouldProcess, ConfirmImpact = 'Medium')]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$OutputDirectory,

    [switch]$NoRestore,

    [switch]$NoBuild,

    [string]$Version = '',

    [string]$PrereleaseSuffix = '',

    [switch]$CreateRunDirectory
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = [System.IO.Path]::GetFullPath(
    (Join-Path $PSScriptRoot '..\..\..')
)
$globalJsonPath = Join-Path $repositoryRoot 'global.json'
$sourceRoot = Join-Path $repositoryRoot 'src'
$solutionPath = Join-Path $repositoryRoot 'CanDoItAll.Components.slnx'
$directoryBuildPropsPath = Join-Path $repositoryRoot 'Directory.Build.props'
$nugetConfigPath = Join-Path $repositoryRoot 'NuGet.config'

if (-not (Test-Path -LiteralPath $globalJsonPath -PathType Leaf)) {
    throw "global.json was not found at '$globalJsonPath'."
}

if (-not (Test-Path -LiteralPath $solutionPath -PathType Leaf)) {
    throw "The canonical solution was not found at '$solutionPath'."
}

if (-not (Test-Path -LiteralPath $directoryBuildPropsPath -PathType Leaf)) {
    throw "Directory.Build.props was not found at '$directoryBuildPropsPath'."
}

if (-not (Test-Path -LiteralPath $nugetConfigPath -PathType Leaf)) {
    throw "NuGet.config was not found at '$nugetConfigPath'."
}

function Invoke-DotNet {
    param(
        [Parameter(Mandatory)]
        [string[]]$Arguments,

        [Parameter(Mandatory)]
        [string]$FailureMessage
    )

    Push-Location -LiteralPath $repositoryRoot
    try {
        & dotnet @Arguments
        if ($LASTEXITCODE -ne 0) {
            throw "$FailureMessage Exit code: $LASTEXITCODE."
        }
    }
    finally {
        Pop-Location
    }
}

[xml]$directoryBuildProps = Get-Content -LiteralPath $directoryBuildPropsPath -Raw
$committedVersionNode = $directoryBuildProps.SelectSingleNode(
    '/Project/PropertyGroup/CanDoItAllPackageBaseVersion'
)
if ($null -eq $committedVersionNode) {
    throw "CanDoItAllPackageBaseVersion must be defined in '$directoryBuildPropsPath'."
}

$committedBaseVersion = $committedVersionNode.InnerText.Trim()
if ([string]::IsNullOrWhiteSpace($committedBaseVersion)) {
    throw "CanDoItAllPackageBaseVersion must not be empty in '$directoryBuildPropsPath'."
}

if (
    -not [string]::IsNullOrWhiteSpace($PrereleaseSuffix) -and
    -not $PrereleaseSuffix.StartsWith('-', [StringComparison]::Ordinal)
) {
    throw "PrereleaseSuffix must start with '-', for example '-preview.1'."
}

$effectiveBaseVersion = if ([string]::IsNullOrWhiteSpace($Version)) {
    $committedBaseVersion
}
else {
    $Version.Trim()
}
$effectiveVersion = "$effectiveBaseVersion$PrereleaseSuffix"
$versionSource = if ([string]::IsNullOrWhiteSpace($Version)) {
    'Directory.Build.props (CanDoItAllPackageBaseVersion)'
}
else {
    'the -Version command-line override'
}

$outputWasSpecified = -not [string]::IsNullOrWhiteSpace($OutputDirectory)
$outputRoot = if ($outputWasSpecified) {
    if ([System.IO.Path]::IsPathRooted($OutputDirectory)) {
        [System.IO.Path]::GetFullPath($OutputDirectory)
    }
    else {
        [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot $OutputDirectory))
    }
}
else {
    Join-Path $repositoryRoot 'artifacts\packages'
}

if (-not $outputWasSpecified -or $CreateRunDirectory) {
    $runTimestamp = Get-Date -Format 'yyyyMMdd-HHmmssfff'
    $OutputDirectory = Join-Path $outputRoot "${effectiveVersion}_$runTimestamp"
}
else {
    $OutputDirectory = $outputRoot
}
$OutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)

$normalizedRepositoryRoot = $repositoryRoot.TrimEnd(
    [System.IO.Path]::DirectorySeparatorChar,
    [System.IO.Path]::AltDirectorySeparatorChar
)
$normalizedOutputDirectory = $OutputDirectory.TrimEnd(
    [System.IO.Path]::DirectorySeparatorChar,
    [System.IO.Path]::AltDirectorySeparatorChar
)
if ($normalizedOutputDirectory -eq $normalizedRepositoryRoot) {
    throw 'The package output directory cannot be the repository root.'
}

$projectFiles = @(
    Get-ChildItem -LiteralPath $sourceRoot -Filter '*.csproj' -File -Recurse |
        Sort-Object FullName
)
$packableProjects = @(
    foreach ($projectFile in $projectFiles) {
        [xml]$projectXml = Get-Content -LiteralPath $projectFile.FullName -Raw
        $isPackableNodes = @(
            $projectXml.SelectNodes('/Project/PropertyGroup/IsPackable')
        )
        $isPackable = $isPackableNodes |
            Where-Object {
                $_.InnerText.Equals('true', [StringComparison]::OrdinalIgnoreCase)
            } |
            Select-Object -First 1

        if ($null -eq $isPackable) {
            continue
        }

        $descriptionNode = $projectXml.SelectSingleNode(
            '/Project/PropertyGroup/Description'
        )
        if (
            $null -eq $descriptionNode -or
            [string]::IsNullOrWhiteSpace($descriptionNode.InnerText)
        ) {
            throw "$($projectFile.Name) must define a package Description."
        }

        $readmePath = Join-Path $projectFile.DirectoryName 'README.md'
        if (-not (Test-Path -LiteralPath $readmePath -PathType Leaf)) {
            throw "$($projectFile.Name) must have a README.md beside the project file."
        }

        $packageIdNode = $projectXml.SelectSingleNode(
            '/Project/PropertyGroup/PackageId'
        )
        $packageId = if (
            $null -eq $packageIdNode -or
            [string]::IsNullOrWhiteSpace($packageIdNode.InnerText)
        ) {
            $projectFile.BaseName
        }
        else {
            $packageIdNode.InnerText.Trim()
        }

        [pscustomobject]@{
            ProjectFile = $projectFile
            PackageId = $packageId
        }
    }
)

if ($packableProjects.Count -eq 0) {
    throw "No packable projects were found under '$sourceRoot'."
}

$operationParts = [System.Collections.Generic.List[string]]::new()
$operationParts.Add('build Tailwind assets')
if (-not $NoRestore) {
    $operationParts.Add('restore the solution')
}
if (-not $NoBuild) {
    $operationParts.Add('build and test the solution')
}
$operationParts.Add("pack $($packableProjects.Count) projects")
$operation = $operationParts -join ', '

if (-not $PSCmdlet.ShouldProcess($OutputDirectory, $operation)) {
    [pscustomobject]@{
        Repository = Split-Path $repositoryRoot -Leaf
        Solution = Split-Path $solutionPath -Leaf
        Configuration = $Configuration
        PackageVersion = $effectiveVersion
        OutputDirectory = $OutputDirectory
        ProjectCount = $packableProjects.Count
        Status = 'Preview'
    }
    return
}

Write-Host "Package version: $effectiveVersion"
Write-Host "Version source: $versionSource"
Write-Host "Package output: $OutputDirectory"
Write-Host ''

Write-Host 'Building Tailwind assets...'
Push-Location $repositoryRoot
try {
    & npm run build:tailwind
    if ($LASTEXITCODE -ne 0) {
        throw "npm run build:tailwind failed with exit code $LASTEXITCODE."
    }
}
finally {
    Pop-Location
}

$msbuildProperties = @()
if (-not [string]::IsNullOrWhiteSpace($Version)) {
    $msbuildProperties += "-p:CanDoItAllPackageBaseVersion=$effectiveBaseVersion"
}
if (-not [string]::IsNullOrWhiteSpace($PrereleaseSuffix)) {
    $msbuildProperties += "-p:CanDoItAllPackageProofSuffix=$PrereleaseSuffix"
}

if (-not $NoRestore) {
    Write-Host ''
    Write-Host 'Restoring solution...'
    $restoreArguments = @(
        'restore',
        $solutionPath,
        '--configfile',
        $nugetConfigPath
    ) + $msbuildProperties
    Invoke-DotNet `
        -Arguments $restoreArguments `
        -FailureMessage 'dotnet restore failed.'
}

if (-not $NoBuild) {
    Write-Host ''
    Write-Host 'Building solution...'
    $buildArguments = @(
        'build',
        $solutionPath,
        '--configuration',
        $Configuration,
        '--no-restore'
    ) + $msbuildProperties
    Invoke-DotNet `
        -Arguments $buildArguments `
        -FailureMessage 'dotnet build failed.'

    Write-Host ''
    Write-Host 'Testing solution...'
    $testArguments = @(
        'test',
        $solutionPath,
        '--configuration',
        $Configuration,
        '--no-build',
        '--no-restore'
    ) + $msbuildProperties
    Invoke-DotNet `
        -Arguments $testArguments `
        -FailureMessage 'dotnet test failed.'
}

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

foreach ($project in $packableProjects) {
    Write-Host ''
    Write-Host "Packing $($project.PackageId)..."
    $packArguments = @(
        'pack',
        $project.ProjectFile.FullName,
        '--configuration',
        $Configuration,
        '--no-build',
        '--no-restore',
        '--output',
        $OutputDirectory
    ) + $msbuildProperties

    Invoke-DotNet `
        -Arguments $packArguments `
        -FailureMessage "dotnet pack failed for '$($project.ProjectFile.Name)'."
}

$packagePaths = @(
    foreach ($project in $packableProjects) {
        $packagePath = Join-Path $OutputDirectory (
            "$($project.PackageId).$effectiveVersion.nupkg"
        )
        if (-not (Test-Path -LiteralPath $packagePath -PathType Leaf)) {
            throw "Expected package was not produced: '$packagePath'."
        }
        $packagePath
    }
)
$symbolPackagePaths = @(
    foreach ($project in $packableProjects) {
        $packagePath = Join-Path $OutputDirectory (
            "$($project.PackageId).$effectiveVersion.snupkg"
        )
        if (-not (Test-Path -LiteralPath $packagePath -PathType Leaf)) {
            throw "Expected symbol package was not produced: '$packagePath'."
        }
        $packagePath
    }
)

Write-Host ''
Write-Host "Packed $($packagePaths.Count) libraries and $($symbolPackagePaths.Count) symbol packages."

[pscustomobject]@{
    Repository = Split-Path $repositoryRoot -Leaf
    Solution = Split-Path $solutionPath -Leaf
    Configuration = $Configuration
    PackageVersion = $effectiveVersion
    OutputDirectory = $OutputDirectory
    Packages = $packagePaths
    SymbolPackages = $symbolPackagePaths
    Status = 'Succeeded'
}
