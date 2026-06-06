param(
    [string]$PackageProofSuffix,
    [string]$ArtifactsRoot = "artifacts/webgl-engine-rc-v16",
    [switch]$SkipBrowserProof
)

$ErrorActionPreference = "Stop"
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$artifacts = Join-Path $repoRoot $ArtifactsRoot
$packageDir = Join-Path $artifacts "packages"
$nugetPackages = Join-Path $artifacts "nuget-packages"
$nugetConfig = Join-Path $artifacts "package-proof.NuGet.config"
$manifestPath = Join-Path $artifacts "artifact-manifest.json"
$summaryJsonPath = Join-Path $artifacts "validation-summary.json"
$summaryMarkdownPath = Join-Path $artifacts "validation-summary.md"
$transcriptPath = Join-Path $artifacts "validate-release-candidate.transcript.txt"

if ([string]::IsNullOrWhiteSpace($PackageProofSuffix)) {
    $PackageProofSuffix = "-rcv16.$([DateTime]::UtcNow.ToString('yyyyMMddHHmmss'))"
}

New-Item -ItemType Directory -Force -Path $artifacts | Out-Null
New-Item -ItemType Directory -Force -Path $packageDir | Out-Null
New-Item -ItemType Directory -Force -Path $nugetPackages | Out-Null

$executed = New-Object System.Collections.Generic.List[object]
$failed = $false

function Invoke-RcStep {
    param(
        [string]$Name,
        [scriptblock]$Command
    )

    $started = Get-Date
    "[$($started.ToString('o'))] START $Name" | Tee-Object -FilePath $transcriptPath -Append
    try {
        & $Command 2>&1 | Tee-Object -FilePath $transcriptPath -Append
        if ($LASTEXITCODE -ne $null -and $LASTEXITCODE -ne 0) {
            throw "$Name failed with exit code $LASTEXITCODE"
        }

        $ended = Get-Date
        "[$($ended.ToString('o'))] PASS $Name" | Tee-Object -FilePath $transcriptPath -Append
        $executed.Add([ordered]@{
            name = $Name
            status = "passed"
            startedUtc = $started.ToUniversalTime().ToString("o")
            endedUtc = $ended.ToUniversalTime().ToString("o")
        }) | Out-Null
    }
    catch {
        $ended = Get-Date
        "[$($ended.ToString('o'))] FAIL $Name" | Tee-Object -FilePath $transcriptPath -Append
        $_ | Out-String | Tee-Object -FilePath $transcriptPath -Append
        $executed.Add([ordered]@{
            name = $Name
            status = "failed"
            startedUtc = $started.ToUniversalTime().ToString("o")
            endedUtc = $ended.ToUniversalTime().ToString("o")
            error = $_.Exception.Message
        }) | Out-Null
        $script:failed = $true
        throw
    }
}

Set-Location $repoRoot
"WebGL engine RC validation" | Set-Content -Path $transcriptPath
"RepoRoot: $repoRoot" | Add-Content -Path $transcriptPath
"PackageProofSuffix: $PackageProofSuffix" | Add-Content -Path $transcriptPath
"StartedUtc: $([DateTime]::UtcNow.ToString('o'))" | Add-Content -Path $transcriptPath

try {
    Invoke-RcStep "npm verify WebGlLib static assets" { npm run webgllib:verify-assets }
    Invoke-RcStep "npm audit WebGlLib scene runtime" { npm run webgllib:audit-scene-runtime }
    Invoke-RcStep "npm audit WebGlLib scene runtime imports" { npm run webgllib:audit-scene-runtime-imports }
    Invoke-RcStep "npm test runtime idle policy" { npm run webgllib:test-runtime-idle-policy }
    Invoke-RcStep "npm test command-batch runtime idle policy" { npm run webgllib:test-command-batch-runtime-idle-policy }
    Invoke-RcStep "npm test resource ownership" { npm run webgllib:test-resource-ownership }
    Invoke-RcStep "npm audit command batch parity" { npm run webgllib:audit-command-batch-parity }
    Invoke-RcStep "npm audit motion queue" { npm run webgllib:audit-motion-queue }
    Invoke-RcStep "npm audit stage runner" { npm run webgllib:audit-stage-runner }
    Invoke-RcStep "npm audit large scene performance" { npm run webgllib:audit-large-scene-performance }
    Invoke-RcStep "domain audit generic source hard gate" { node tools/webgllib/domain-boundary-auditor.cjs --profile generic-source-hard-gate }
    Invoke-RcStep "domain audit public API hard gate" { node tools/webgllib/domain-boundary-auditor.cjs --profile generic-public-api-hard-gate }
    Invoke-RcStep "domain audit package content hard gate" { node tools/webgllib/domain-boundary-auditor.cjs --profile package-content-hard-gate }
    Invoke-RcStep "dotnet build solution" { dotnet build CanDoItAll.Components.slnx /p:UseSharedCompilation=false }
    Invoke-RcStep "dotnet test WebGlLib" { dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore /p:UseSharedCompilation=false }
    Invoke-RcStep "dotnet test WebGlRunLib" { dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore /p:UseSharedCompilation=false }
    Invoke-RcStep "dotnet pack release packages" { dotnet pack CanDoItAll.Components.slnx --configuration Release --output $packageDir /p:CanDoItAllPackageProofSuffix=$PackageProofSuffix /p:UseSharedCompilation=false }

    @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="fresh-components-proof" value="$packageDir" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"@ | Set-Content -Path $nugetConfig

    $env:NUGET_PACKAGES = $nugetPackages
    $packageVersion = "0.1.0$PackageProofSuffix"
    Invoke-RcStep "restore WebGlLib package-mode viewer" { dotnet restore samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj --configfile $nugetConfig /p:UseComponentsWebGlLibPackage=true /p:ComponentsWebGlLibPackageVersion=$packageVersion }
    Invoke-RcStep "build WebGlLib package-mode viewer" { dotnet build samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj --no-restore /p:UseComponentsWebGlLibPackage=true /p:ComponentsWebGlLibPackageVersion=$packageVersion /p:UseSharedCompilation=false }
    Invoke-RcStep "restore WebGlRunLib package-mode sample" { dotnet restore samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj --configfile $nugetConfig /p:UseComponentsWebGlRunLibPackage=true /p:ComponentsWebGlRunLibPackageVersion=$packageVersion }
    Invoke-RcStep "build WebGlRunLib package-mode sample" { dotnet build samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj --no-restore /p:UseComponentsWebGlRunLibPackage=true /p:ComponentsWebGlRunLibPackageVersion=$packageVersion /p:UseSharedCompilation=false }
    Invoke-RcStep "run WebGlRunLib package-mode sample" { dotnet run --project samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj --no-build /p:UseComponentsWebGlRunLibPackage=true /p:ComponentsWebGlRunLibPackageVersion=$packageVersion }

    if (-not $SkipBrowserProof) {
        Invoke-RcStep "browser observer proof" { & (Join-Path $PSScriptRoot "run-browser-observer-proof.ps1") -NoBuild }
    }
}
finally {
    $passedCount = @($executed | Where-Object { $_.status -eq "passed" }).Count
    $failedCount = @($executed | Where-Object { $_.status -eq "failed" }).Count
    $manifest = [ordered]@{
        schemaVersion = "webgl-engine-rc-validation/v2"
        generatedAtUtc = [DateTime]::UtcNow.ToString("o")
        packageVersion = "0.1.0$PackageProofSuffix"
        artifactsRoot = (Resolve-Path $artifacts).Path
        transcript = $transcriptPath
        summaryJson = $summaryJsonPath
        summaryMarkdown = $summaryMarkdownPath
        packageDirectory = $packageDir
        nugetConfig = $nugetConfig
        steps = $executed
        passedStepCount = $passedCount
        failedStepCount = $failedCount
        failed = $failed
    }
    $manifest | ConvertTo-Json -Depth 8 | Set-Content -Path $manifestPath

    $summary = [ordered]@{
        schemaVersion = "webgl-engine-rc-summary/v1"
        generatedAtUtc = $manifest.generatedAtUtc
        packageVersion = $manifest.packageVersion
        status = if ($failed) { "failed" } else { "passed" }
        passedStepCount = $passedCount
        failedStepCount = $failedCount
        artifactManifest = $manifestPath
        transcript = $transcriptPath
        steps = $executed
    }
    $summary | ConvertTo-Json -Depth 8 | Set-Content -Path $summaryJsonPath

    $stepRows = foreach ($step in $executed) {
        "| $($step.status) | $($step.name) | $($step.startedUtc) | $($step.endedUtc) |"
    }
    if ($stepRows.Count -eq 0) {
        $stepRows = @("| not-run | no steps executed |  |  |")
    }

    $summaryMarkdown = @(
        "# WebGL Engine RC Validation Summary",
        "",
        "- Schema: webgl-engine-rc-summary/v1",
        "- Status: $(if ($failed) { 'failed' } else { 'passed' })",
        "- Package version: $($manifest.packageVersion)",
        "- Artifact manifest: $manifestPath",
        "- Transcript: $transcriptPath",
        "",
        "| Status | Step | Started UTC | Ended UTC |",
        "|---|---|---|---|"
    ) + $stepRows
    $summaryMarkdown | Set-Content -Path $summaryMarkdownPath
}

if ($failed) {
    exit 1
}

Write-Host "WebGL engine RC validation passed. Manifest: $manifestPath"
