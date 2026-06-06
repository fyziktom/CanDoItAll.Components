param(
    [string]$PackageProofSuffix,
    [string]$ArtifactsRoot = "artifacts/webgl-engine-rc-v17",
    [switch]$SkipBrowserProof
)

$ErrorActionPreference = "Stop"
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$innerScript = Join-Path $scriptRoot "webgl-engine\validate-release-candidate.ps1"

$arguments = @{
    ArtifactsRoot = $ArtifactsRoot
}

if (-not [string]::IsNullOrWhiteSpace($PackageProofSuffix)) {
    $arguments.PackageProofSuffix = $PackageProofSuffix
}

if ($SkipBrowserProof) {
    $arguments.SkipBrowserProof = $true
}

& $innerScript @arguments
