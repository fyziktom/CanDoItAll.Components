param(
    [string]$PackageProofSuffix,
    [string]$ArtifactsRoot = "artifacts/webgl-engine-rc-v17",
    [switch]$SkipBrowserProof
)

$ErrorActionPreference = "Stop"
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$artifacts = Join-Path $repoRoot $ArtifactsRoot
$packageDir = Join-Path $artifacts "packages"
$nugetPackages = Join-Path $artifacts "nuget-packages"
$stepTranscriptDir = Join-Path $artifacts "step-transcripts"
$nugetConfig = Join-Path $artifacts "package-proof.NuGet.config"
$manifestPath = Join-Path $artifacts "artifact-manifest.json"
$summaryJsonPath = Join-Path $artifacts "validation-summary.json"
$summaryMarkdownPath = Join-Path $artifacts "validation-summary.md"
$transcriptPath = Join-Path $artifacts "validate-release-candidate.transcript.txt"

if ([string]::IsNullOrWhiteSpace($PackageProofSuffix)) {
    $PackageProofSuffix = "-rcv17.$([DateTime]::UtcNow.ToString('yyyyMMddHHmmss'))"
}

New-Item -ItemType Directory -Force -Path $artifacts | Out-Null
New-Item -ItemType Directory -Force -Path $packageDir | Out-Null
New-Item -ItemType Directory -Force -Path $nugetPackages | Out-Null
New-Item -ItemType Directory -Force -Path $stepTranscriptDir | Out-Null

$executed = New-Object System.Collections.ArrayList
$failed = $false
$script:rcStepIndex = 0

function ConvertTo-RcSlug {
    param([string]$Value)

    $slug = [regex]::Replace($Value.ToLowerInvariant(), "[^a-z0-9]+", "-").Trim("-")
    if ([string]::IsNullOrWhiteSpace($slug)) {
        return "step"
    }

    return $slug
}

function ConvertTo-RepoRelativePath {
    param([string]$Path)

    $resolved = if (Test-Path -LiteralPath $Path) {
        (Resolve-Path -LiteralPath $Path).Path
    }
    else {
        [System.IO.Path]::GetFullPath($Path)
    }

    if ($resolved.StartsWith($repoRoot, [StringComparison]::OrdinalIgnoreCase)) {
        return $resolved.Substring($repoRoot.Length).TrimStart('\', '/').Replace('\', '/')
    }

    return $resolved.Replace('\', '/')
}

function New-RcArtifactRecord {
    param(
        [string]$Path,
        [string]$Kind,
        [bool]$AllowEmpty = $false
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "Required artifact '$Path' was not created."
    }

    $item = Get-Item -LiteralPath $Path
    if ($item.Length -le 0 -and -not $AllowEmpty) {
        throw "Required artifact '$Path' is empty."
    }

    $hash = (Get-FileHash -LiteralPath $item.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
    return [ordered]@{
        kind = $Kind
        path = ConvertTo-RepoRelativePath $item.FullName
        bytes = $item.Length
        sha256 = $hash
    }
}

function Add-RcTranscriptLine {
    param(
        [string]$Path,
        [string]$Line
    )

    $Line | Add-Content -LiteralPath $Path
}

function ConvertTo-RcLong {
    param($Value)

    if ($null -eq $Value) {
        return 0
    }

    return [long]$Value
}

function Assert-PackageModeRestore {
    param(
        [string]$ProjectPath,
        [string]$PackageId,
        [string]$PackageVersion
    )

    $projectDirectory = Split-Path -Parent (Resolve-Path -LiteralPath $ProjectPath).Path
    $assetsPath = Join-Path $projectDirectory "obj\project.assets.json"
    if (-not (Test-Path -LiteralPath $assetsPath -PathType Leaf)) {
        throw "Missing package-mode restore assets file: $assetsPath"
    }

    $assets = Get-Content -LiteralPath $assetsPath -Raw | ConvertFrom-Json
    $expectedLibrary = "$PackageId/$PackageVersion"
    $libraries = @($assets.libraries.PSObject.Properties)
    $matchingLibrary = $libraries | Where-Object { $_.Name -eq $expectedLibrary } | Select-Object -First 1
    if ($null -eq $matchingLibrary) {
        $available = ($libraries | Where-Object { $_.Name -like "$PackageId/*" } | ForEach-Object { $_.Name }) -join ", "
        throw "Package-mode restore did not resolve $expectedLibrary. Available matches: $available"
    }

    $projectBacked = $libraries | Where-Object {
        $_.Name -like "$PackageId/*" -and
        $_.Value.type -ne "package"
    }
    if (@($projectBacked).Count -gt 0) {
        $names = ($projectBacked | ForEach-Object { "$($_.Name):$($_.Value.type)" }) -join ", "
        throw "Package-mode restore used non-package libraries for ${PackageId}: $names"
    }

    Write-Output "Package-mode assertion passed for $expectedLibrary"
    Write-Output "Assets: $(ConvertTo-RepoRelativePath $assetsPath)"
}

function Invoke-RcStep {
    param(
        [string]$Name,
        [scriptblock]$Command
    )

    $script:rcStepIndex += 1
    $started = Get-Date
    $slug = ConvertTo-RcSlug $Name
    $stepTranscriptPath = Join-Path $stepTranscriptDir ("{0:D2}-{1}.transcript.txt" -f $script:rcStepIndex, $slug)
    $stepHeader = @(
        "Step: $Name",
        "WorkingDirectory: $repoRoot",
        "StartedUtc: $($started.ToUniversalTime().ToString('o'))",
        "Command: $($Command.ToString().Trim())",
        ""
    )

    $stepHeader | Set-Content -LiteralPath $stepTranscriptPath
    "[$($started.ToString('o'))] START $Name" | Tee-Object -FilePath $transcriptPath -Append
    try {
        $global:LASTEXITCODE = $null
        $output = @(& $Command 2>&1)
        foreach ($line in $output) {
            $text = if ($null -eq $line) { "" } else { $line.ToString() }
            Add-RcTranscriptLine -Path $stepTranscriptPath -Line $text
            Add-RcTranscriptLine -Path $transcriptPath -Line $text
        }

        $exitCode = if ($null -ne $global:LASTEXITCODE) { [int]$global:LASTEXITCODE } else { 0 }
        if ($exitCode -ne 0) {
            throw "$Name failed with exit code $exitCode"
        }

        $outputLineCount = @($output).Count
        if ($outputLineCount -le 0) {
            throw "$Name produced no command output; refusing no-op proof."
        }

        $stepArtifact = New-RcArtifactRecord -Path $stepTranscriptPath -Kind "step-transcript"
        $ended = Get-Date
        "[$($ended.ToString('o'))] PASS $Name" | Tee-Object -FilePath $transcriptPath -Append
        $executed.Add([ordered]@{
            name = $Name
            status = "passed"
            startedUtc = $started.ToUniversalTime().ToString("o")
            endedUtc = $ended.ToUniversalTime().ToString("o")
            transcript = $stepArtifact.path
            transcriptBytes = $stepArtifact.bytes
            transcriptSha256 = $stepArtifact.sha256
            outputLineCount = $outputLineCount
            assertions = [ordered]@{
                exitCodeZero = $true
                transcriptNonEmpty = $stepArtifact.bytes -gt 0
                commandOutputNonEmpty = $outputLineCount -gt 0
            }
        }) | Out-Null
    }
    catch {
        $ended = Get-Date
        "[$($ended.ToString('o'))] FAIL $Name" | Tee-Object -FilePath $transcriptPath -Append
        $_ | Out-String | Tee-Object -FilePath $transcriptPath -Append
        $_ | Out-String | Add-Content -LiteralPath $stepTranscriptPath
        $stepArtifact = New-RcArtifactRecord -Path $stepTranscriptPath -Kind "step-transcript"
        $executed.Add([ordered]@{
            name = $Name
            status = "failed"
            startedUtc = $started.ToUniversalTime().ToString("o")
            endedUtc = $ended.ToUniversalTime().ToString("o")
            transcript = $stepArtifact.path
            transcriptBytes = $stepArtifact.bytes
            transcriptSha256 = $stepArtifact.sha256
            outputLineCount = 0
            error = $_.Exception.Message
        }) | Out-Null
        $script:failed = $true
        throw
    }
}

Set-Location $repoRoot
"WebGL engine RC validation" | Set-Content -LiteralPath $transcriptPath
"RepoRoot: $repoRoot" | Add-Content -LiteralPath $transcriptPath
"PackageProofSuffix: $PackageProofSuffix" | Add-Content -LiteralPath $transcriptPath
"ArtifactsRoot: $ArtifactsRoot" | Add-Content -LiteralPath $transcriptPath
"StartedUtc: $([DateTime]::UtcNow.ToString('o'))" | Add-Content -LiteralPath $transcriptPath

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
"@ | Set-Content -LiteralPath $nugetConfig

    $env:NUGET_PACKAGES = $nugetPackages
    $packageVersion = "0.1.0$PackageProofSuffix"
    Invoke-RcStep "restore WebGlLib package-mode viewer" { dotnet restore samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj --configfile $nugetConfig /p:UseComponentsWebGlLibPackage=true /p:ComponentsWebGlLibPackageVersion=$packageVersion }
    Invoke-RcStep "build WebGlLib package-mode viewer" { dotnet build samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj --no-restore /p:UseComponentsWebGlLibPackage=true /p:ComponentsWebGlLibPackageVersion=$packageVersion /p:UseSharedCompilation=false }
    Invoke-RcStep "assert WebGlLib package-mode viewer uses fresh package" {
        Assert-PackageModeRestore `
            -ProjectPath "samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj" `
            -PackageId "CanDoItAll.Components.WebGlLib" `
            -PackageVersion $packageVersion
    }
    Invoke-RcStep "restore WebGlRunLib package-mode sample" { dotnet restore samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj --configfile $nugetConfig /p:UseComponentsWebGlRunLibPackage=true /p:ComponentsWebGlRunLibPackageVersion=$packageVersion }
    Invoke-RcStep "build WebGlRunLib package-mode sample" { dotnet build samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj --no-restore /p:UseComponentsWebGlRunLibPackage=true /p:ComponentsWebGlRunLibPackageVersion=$packageVersion /p:UseSharedCompilation=false }
    Invoke-RcStep "assert WebGlRunLib package-mode sample uses fresh package" {
        Assert-PackageModeRestore `
            -ProjectPath "samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj" `
            -PackageId "CanDoItAll.Components.WebGlRunLib" `
            -PackageVersion $packageVersion
    }
    Invoke-RcStep "run WebGlRunLib package-mode sample" { dotnet run --project samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj --no-build /p:UseComponentsWebGlRunLibPackage=true /p:ComponentsWebGlRunLibPackageVersion=$packageVersion }

    if (-not $SkipBrowserProof) {
        Invoke-RcStep "browser observer proof" { & (Join-Path $PSScriptRoot "run-browser-observer-proof.ps1") -NoBuild }
    }
}
finally {
    $passedCount = @($executed | Where-Object { $_.status -eq "passed" }).Count
    $failedCount = @($executed | Where-Object { $_.status -eq "failed" }).Count

    $stepRows = foreach ($step in $executed) {
        "| $($step["status"]) | $($step["name"]) | $($step["startedUtc"]) | $($step["endedUtc"]) | $($step["transcript"]) |"
    }
    if ($stepRows.Count -eq 0) {
        $stepRows = @("| not-run | no steps executed |  |  |  |")
    }

    $summary = [ordered]@{
        schemaVersion = "webgl-engine-rc-summary/v2"
        generatedAtUtc = [DateTime]::UtcNow.ToString("o")
        packageVersion = "0.1.0$PackageProofSuffix"
        status = if ($failed) { "failed" } else { "passed" }
        passedStepCount = $passedCount
        failedStepCount = $failedCount
        artifactManifest = $manifestPath
        transcript = $transcriptPath
        stepTranscriptDirectory = $stepTranscriptDir
        steps = $executed
    }
    $summary | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $summaryJsonPath

    $summaryMarkdown = @(
        "# WebGL Engine RC Validation Summary",
        "",
        "- Schema: webgl-engine-rc-summary/v2",
        "- Status: $(if ($failed) { 'failed' } else { 'passed' })",
        "- Package version: $($summary.packageVersion)",
        "- Artifact manifest: $manifestPath",
        "- Transcript: $transcriptPath",
        "",
        "| Status | Step | Started UTC | Ended UTC | Transcript |",
        "|---|---|---|---|---|"
    ) + $stepRows
    $summaryMarkdown | Set-Content -LiteralPath $summaryMarkdownPath

    $artifactRecords = @(Get-ChildItem -LiteralPath $artifacts -Recurse -File |
        Where-Object { $_.FullName -ne $manifestPath } |
        Sort-Object FullName |
        ForEach-Object {
            New-RcArtifactRecord -Path $_.FullName -Kind "rc-artifact" -AllowEmpty $true
        })

    $executedItems = @($executed)
    $emptyTranscriptSteps = @($executedItems | Where-Object { (ConvertTo-RcLong $_["transcriptBytes"]) -le 0 })
    $emptyOutputSteps = @($executedItems | Where-Object { (ConvertTo-RcLong $_["outputLineCount"]) -le 0 })
    $missingHashArtifacts = @($artifactRecords | Where-Object { [string]::IsNullOrWhiteSpace([string]$_["sha256"]) })
    $packageModeAssertionSteps = @($executedItems | Where-Object {
        [string]$_["name"] -like "assert *package-mode*" -and [string]$_["status"] -eq "passed"
    })
    $proofAssertions = [ordered]@{
        noStepsExecuted = [bool]($executedItems.Count -eq 0)
        allStepTranscriptsNonEmpty = [bool]($emptyTranscriptSteps.Count -eq 0)
        allStepOutputsNonEmpty = [bool]($emptyOutputSteps.Count -eq 0)
        artifactManifestHasHashes = [bool](($artifactRecords.Count -gt 0) -and ($missingHashArtifacts.Count -eq 0))
        packageModeAssertionsRan = [bool]($packageModeAssertionSteps.Count -ge 2)
    }

    $manifest = [ordered]@{
        schemaVersion = "webgl-engine-rc-validation/v3"
        generatedAtUtc = [DateTime]::UtcNow.ToString("o")
        packageVersion = "0.1.0$PackageProofSuffix"
        artifactsRoot = (Resolve-Path -LiteralPath $artifacts).Path
        transcript = $transcriptPath
        summaryJson = $summaryJsonPath
        summaryMarkdown = $summaryMarkdownPath
        packageDirectory = $packageDir
        nugetConfig = $nugetConfig
        stepTranscriptDirectory = $stepTranscriptDir
        steps = $executed
        artifactRecords = $artifactRecords
        proofAssertions = $proofAssertions
        passedStepCount = $passedCount
        failedStepCount = $failedCount
        failed = $failed
    }
    $manifest | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $manifestPath
}

if ($failed) {
    exit 1
}

$emptyStep = @($executed | Where-Object { $_.outputLineCount -le 0 -or $_.transcriptBytes -le 0 })
if ($emptyStep.Count -gt 0) {
    Write-Error "RC validation refused empty/no-op proof for: $($emptyStep.name -join ', ')"
    exit 1
}

Write-Host "WebGL engine RC validation passed. Manifest: $manifestPath"
