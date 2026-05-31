# SB13 Proof Manifest

Status: Completed

## Scope

Performance and scalability proof for economy normalization/materialization, visual frame mapping, WebGL bridge projection, command batch generation, and runtime link/motion batching.

## Changed Files

| File | SHA-256 proof |
|---|---|
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyPerformanceProbeTests.cs` | `bundle://proof/SB13/hashes/performance-file-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs` | `bundle://proof/SB13/hashes/performance-file-hashes.txt` |
| `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs` | `bundle://proof/SB13/hashes/performance-file-hashes.txt` |

## Command Transcripts

| Command | Result | Path |
|---|---|---|
| `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter EconomyPerformanceProbeTests --logger "console;verbosity=normal"` | Pass, 1 test | `bundle://proof/SB13/transcripts/economy-performance-probe-tests.txt` |
| `npm run webgllib:audit-sharedwell-performance` | Pass | `bundle://proof/SB13/transcripts/components-sharedwell-performance-audit.txt` |
| `powershell -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1` | Pass | `bundle://proof/SB13/transcripts/simulation-boundary-audit.txt` |

## JSON Artifacts

| Artifact | Path |
|---|---|
| Economy performance proof with normalization/materialization/hash/metrics/mapping/projection/batch/frame-smoke operations | `bundle://proof/SB13/artifacts/economy-simulation-performance-proof.json` |
| Components WebGlLib performance proof for batched motions, ordered stages, and indexed link updates | `bundle://proof/SB13/artifacts/components-performance-proof.json` |

## Source Assertions

| Assertion | Path |
|---|---|
| Source scan proves required probe stages, 100/1000/300/1200 scale inputs, dictionary action merge, node-object dictionary lookup, and cached object bindings. | `bundle://proof/SB13/source-assertions/performance-source-assertions.txt` |
| Anti-stub scan covers the performance probe and bridge lookup-cache edits. | `bundle://proof/SB13/source-assertions/anti-stub-scan.txt` |

## Semantic Gate

See `bundle://proof/SB13/semantic-invariants.md`.

## Downstream Gate

SB14 and SB15 may proceed because performance proof exists before richer demo closure, and bridge node/action resolution avoids obvious repeated linear scans by using `NodeObjectIds`, action dictionaries, and cached object bindings.
