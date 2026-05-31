# Proof manifest SB15

Status: Completed.

## Changed files

- `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\EconomyPerformanceProbeTests.cs`

## Implementation

- Strengthened the existing Economy performance probe to exercise the SB15 scale targets: 250 actors, 500 stores, 1000 scheduled events, 1000 visual actions, 1000 staged WebGL commands, and 100 snapshot export/import round trips.
- The probe now writes `artifacts/economy/performance/simulation-performance-proof.json`, copied into `bundle://proof/SB15/metrics/simulation-performance-proof.json`.
- Browser proof was not used; SB15 browser frame timing is therefore intentionally not recorded.

## Command transcripts

- Economy performance focused probe: `bundle://proof/SB15/transcripts/economy-performance-probe-tests.txt`
- Full Economy tests: `bundle://proof/SB15/transcripts/economy-tests.txt`
- Economy boundary audit: `bundle://proof/SB15/transcripts/economy-boundary-audit.txt`
- WebGL scene runtime audit: `bundle://proof/SB15/transcripts/webgl-scene-runtime-audit.txt`
- Bundle prepared validator after SB15: `bundle://proof/SB15/transcripts/bundle-validator-prepared-after-sb15.txt`

## Test results

- `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter "FullyQualifiedName~EconomyPerformanceProbeTests"` passed: 1/1.
- `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj` passed: 512/512.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1` passed.
- `npm run webgllib:audit-scene-runtime` passed with 10 warning-threshold split candidates and no failures.
- `python .\scripts\validate_bundle.py --stage prepared` passed after SB15 proof updates.

## Metrics

From `bundle://proof/SB15/metrics/simulation-performance-proof.json` after the full Economy run:

| Operation | Elapsed ms | Key counts |
|---|---:|---|
| Scenario normalization | 16.09 | 250 actors, 500 stores, 1000 events |
| Materialization | 148642.42 | 1001 frames |
| Final frame hash | 29.185 | 500 stores |
| Metric/invariant evaluation | 10.517 | 500 stores |
| Visual mapping | 14.986 | 1000 nodes, 501 links |
| WebGL bridge projection | 24.994 | 1000 objects, 501 links |
| Command batch normalization | 18.999 | 1000 visual actions, 1000 staged commands |
| Snapshot export/import | 41418.496 | 100 snapshots |

## Source assertions

- Hashes: `bundle://proof/SB15/hashes/sha256.txt`
- Source map: `bundle://proof/SB15/source-assertions/performance-source-map.md`
- Anti-stub scan: `bundle://proof/SB15/source-assertions/anti-stub-scan.txt`
- Metrics artifact: `bundle://proof/SB15/metrics/simulation-performance-proof.json`

## Production Behavior Artifact Matrix

No new production signal, state, record, or event type was added in SB15. The test exercises existing production artifacts:

| Artifact | Producer | Consumer | Lifecycle | Proof |
|---|---|---|---|---|
| Large simulation frames and deltas | `SimpleSimulationStateTransitionEngine` | Metrics, visual mapper, snapshot builder | Built from 1000 scheduled events into 1001 frames | `LargeGenericSimulationPerformanceProbe_WritesProofArtifact` |
| Visual frame | `EconomyVisualFrameMapper` | WebGL bridge projector | Mapped from the final large simulation frame | `LargeGenericSimulationPerformanceProbe_WritesProofArtifact` |
| WebGL command batch | `EconomyWebGlRunProjector` and `WebGlRunFrameApplyResult` | Command batch normalizer/runtime | Projected from 1000 visual actions and normalized into staged commands | `LargeGenericSimulationPerformanceProbe_WritesProofArtifact` |
| Simulation snapshot JSON | `SimulationSnapshotBuilder` and serializer | Snapshot import validation | Built, serialized, and hash-validated 100 times | `LargeGenericSimulationPerformanceProbe_WritesProofArtifact` |

## Failures / blockers

- No browser route was exercised, so average/peak WebGL browser frame timing is not applicable in SB15.
- The full-suite materialization timing shows a bottleneck signal around in-memory frame/delta retention for 1000 events; this is recorded as readiness evidence, not optimized in this bundle.
- No implementation blockers remain.
