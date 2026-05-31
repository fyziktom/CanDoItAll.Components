# SB05 Proof Manifest

Status: Completed

## Scope

Economy WebGL bridge projection hardening for executable documents, diagnostics, and shared/finite probe coverage.

## Changed Files

| File | SHA-256 proof |
|---|---|
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlInitialSceneProjector.cs` | `bundle://proof/SB05/hashes/changed-file-sha256.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs` | `bundle://proof/SB05/hashes/changed-file-sha256.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs` | `bundle://proof/SB05/hashes/changed-file-sha256.txt` |
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeTests.cs` | `bundle://proof/SB05/hashes/changed-file-sha256.txt` |

## Command Transcripts

| Command | Result | Path |
|---|---|---|
| `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter EconomyWebGlBridgeTests` | Pass, 9 tests | `bundle://proof/SB05/transcripts/economy-webglbridge-tests.txt` |
| `powershell -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1` | Pass | `bundle://proof/SB05/transcripts/economy-boundary-audit.txt` |

## Source Assertions

| Assertion | Path |
|---|---|
| Bridge emits diagnostics for unresolved subject, unresolved target, missing asset mapping, wait fallback, and invalid WebGL plan. | `bundle://proof/SB05/source-assertions/bridge-diagnostics-source-assertions.txt` |
| Bridge reference scan shows Components references are isolated to the bridge project and backend references are not introduced into bridge code. | `bundle://proof/SB05/source-assertions/bridge-reference-scan.txt` |
| Anti-stub scan covers production bridge files and bridge tests. | `bundle://proof/SB05/source-assertions/anti-stub-scan.txt` |

## Behavior Artifacts

| Artifact | Path |
|---|---|
| Bridge test transcript proving diagnostics, non-duplicated global actions, initial scene projection, and shared/finite executable stages | `bundle://proof/SB05/transcripts/economy-webglbridge-tests.txt` |

## Semantic Gate

See `bundle://proof/SB05/semantic-invariants.md`.
