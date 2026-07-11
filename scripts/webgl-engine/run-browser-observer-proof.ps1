param(
    [string]$BaseUrl = "http://127.0.0.1:5206",
    [string]$ProofDir = "codex/bundles/WebGlEngine_Stabilization_v17/proof/SB16/browser",
    [int]$TimeoutMs = 120000,
    [switch]$NoBuild
)

$ErrorActionPreference = "Stop"
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$sandboxProject = Join-Path $repoRoot "samples\CanDoItAll.Components.WebGlSandbox\CanDoItAll.Components.WebGlSandbox.csproj"
$proofRoot = Join-Path $repoRoot $ProofDir
$serverLogDir = Join-Path $proofRoot "server"
$stdout = Join-Path $serverLogDir "sandbox.stdout.log"
$stderr = Join-Path $serverLogDir "sandbox.stderr.log"
$report = Join-Path $proofRoot "browser-observer-proof.json"
$screenshot = Join-Path $proofRoot "run-playback.png"
$route = "$BaseUrl/run-playback"

New-Item -ItemType Directory -Force -Path $proofRoot | Out-Null
New-Item -ItemType Directory -Force -Path $serverLogDir | Out-Null

$arguments = @(
    "run",
    "--project", $sandboxProject,
    "--no-launch-profile",
    "--urls", $BaseUrl
)
if ($NoBuild) {
    $arguments += "--no-build"
}

$server = $null
try {
    $server = Start-Process -FilePath "dotnet" `
        -ArgumentList $arguments `
        -WorkingDirectory $repoRoot `
        -RedirectStandardOutput $stdout `
        -RedirectStandardError $stderr `
        -WindowStyle Hidden `
        -PassThru

    $deadline = (Get-Date).AddSeconds(90)
    do {
        if ($server.HasExited) {
            throw "Sandbox exited before the browser proof could start. See $stderr"
        }

        try {
            $response = Invoke-WebRequest -Uri $route -UseBasicParsing -TimeoutSec 2
            if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 500) {
                break
            }
        }
        catch {
            Start-Sleep -Milliseconds 500
        }
    } while ((Get-Date) -lt $deadline)

    if ((Get-Date) -ge $deadline) {
        throw "Timed out waiting for sandbox route $route"
    }

    node (Join-Path $repoRoot "tools\webgllib\run-browser-observer-proof.cjs") `
        --url $route `
        --output $report `
        --screenshot $screenshot `
        --timeoutMs $TimeoutMs
    if ($LASTEXITCODE -ne 0) {
        throw "Browser observer proof node runner failed with exit code $LASTEXITCODE"
    }
}
finally {
    if ($server -ne $null -and -not $server.HasExited) {
        Stop-Process -Id $server.Id -Force
        $server.WaitForExit(10000) | Out-Null
    }
}
