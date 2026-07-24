<#
.SYNOPSIS
Compatibility wrapper for the canonical NuGet build adapter.

.DESCRIPTION
New automation should call tools/deployment/nugets/Build-NuGets.ps1. This
wrapper preserves the former OutputPath parameter and versioned-run-folder
behavior for existing callers.
#>
[CmdletBinding(SupportsShouldProcess, ConfirmImpact = 'Medium')]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$OutputPath = 'artifacts/packages',

    [string]$Version = '',

    [string]$PrereleaseSuffix = '',

    [switch]$NoBuild,

    [switch]$NoRestore
)

$ErrorActionPreference = 'Stop'

Write-Warning (
    'tools/pack-packages.ps1 is retained for compatibility. ' +
    'Prefer tools/deployment/nugets/Build-NuGets.ps1.'
)

$packScript = Join-Path $PSScriptRoot 'deployment\nugets\Build-NuGets.ps1'
$arguments = @{
    Configuration = $Configuration
    OutputDirectory = $OutputPath
    CreateRunDirectory = $true
    Version = $Version
    PrereleaseSuffix = $PrereleaseSuffix
    NoBuild = $NoBuild
    NoRestore = $NoRestore
}

& $packScript @arguments -WhatIf:$WhatIfPreference
