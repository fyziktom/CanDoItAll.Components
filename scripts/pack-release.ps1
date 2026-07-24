[CmdletBinding(SupportsShouldProcess, ConfirmImpact = 'Medium')]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$OutputPath = "artifacts/packages",

    [string]$Version = '',

    [string]$PrereleaseSuffix = '',

    [switch]$NoBuild,

    [switch]$NoRestore
)

$ErrorActionPreference = "Stop"

Write-Warning (
    "scripts/pack-release.ps1 is retained for compatibility. " +
    "Prefer tools/deployment/nugets/Build-NuGets.ps1."
)

$packScript = Join-Path $PSScriptRoot "..\tools\pack-packages.ps1"
$arguments = @{
    Configuration = $Configuration
    OutputPath = $OutputPath
    Version = $Version
    PrereleaseSuffix = $PrereleaseSuffix
    NoBuild = $NoBuild
    NoRestore = $NoRestore
}

& $packScript @arguments -WhatIf:$WhatIfPreference
