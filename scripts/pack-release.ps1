param(
    [string]$OutputPath = "artifacts/packages",
    [switch]$NoBuild
)

$ErrorActionPreference = "Stop"

Write-Warning "scripts/pack-release.ps1 is retained for compatibility. Prefer tools/pack-packages.ps1."

$packScript = Join-Path $PSScriptRoot "..\tools\pack-packages.ps1"
& $packScript -OutputPath $OutputPath -NoBuild:$NoBuild
exit $LASTEXITCODE
