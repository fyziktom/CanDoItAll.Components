# Proof manifest SB14

Status: Completed.

## Changed files

- `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\FiniteResourceMarketProbeTests.cs`

## Implementation

- Added a focused finite-resource market probe test that loads the farmer/land fixture through the generic experiment input loader instead of adding domain-specific production code.
- The probe proves finite capacity, actor expansion, external demand, concentration metrics, anti-concentration invariant failure, fee event emission, and production snapshot diff behavior.
- The before/after snapshot diff is built with `SimulationSnapshotBuilder` and compared with `SimulationSnapshotDiff`, then asserted for resource store, metric, invariant, applied-event, and pending-event changes.

## Command transcripts

- Finite-resource focused probe test: `bundle://proof/SB14/transcripts/finite-resource-probe-tests.txt`
- Full Economy tests: `bundle://proof/SB14/transcripts/economy-tests.txt`
- Economy boundary audit: `bundle://proof/SB14/transcripts/economy-boundary-audit.txt`
- Bundle prepared validator after SB14: `bundle://proof/SB14/transcripts/bundle-validator-prepared-after-sb14.txt`

## Test results

- `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter "FullyQualifiedName~FiniteResourceMarketProbeTests"` passed: 1/1.
- `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj` passed: 512/512.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1` passed.
- `python .\scripts\validate_bundle.py --stage prepared` passed after SB14 proof updates.

## Source assertions

- Hashes: `bundle://proof/SB14/hashes/sha256.txt`
- Source map: `bundle://proof/SB14/source-assertions/finite-resource-source-map.md`
- Forbidden production term scan: `bundle://proof/SB14/source-assertions/generic-production-forbidden-term-scan.txt`
- Anti-stub scan: `bundle://proof/SB14/source-assertions/anti-stub-scan.txt`

## Production Behavior Artifact Matrix

No new production signal, state, record, or event type was added in SB14. The test exercises existing production artifacts:

| Artifact | Producer | Consumer | Lifecycle | Proof |
|---|---|---|---|---|
| `SimulationMetricValue` concentration metrics | `SimulationFrameMetricEvaluator` | SB14 probe and snapshot builder | Computed from a materialized frame and included in snapshots | `FiniteResourceMarketProbe_UsesGenericMetricsRulesFeesAndSnapshotDiffs` |
| `SimulationInvariantEvaluationResult` anti-concentration result | `SimulationInvariantEvaluator` | SB14 probe and snapshot builder | Computed from the same materialized frame and diffed before/after | `FiniteResourceMarketProbe_UsesGenericMetricsRulesFeesAndSnapshotDiffs` |
| `SimulationRunSnapshot` | `SimulationSnapshotBuilder` | `SimulationSnapshotDiff` | Built before and after the policy checkpoint, then hash-refreshed by the builder | `FiniteResourceMarketProbe_UsesGenericMetricsRulesFeesAndSnapshotDiffs` |

## Failures / blockers

- The first local focused run failed because the new helper looked for fixtures under the test output directory. The helper now uses the repository fixture path pattern used by existing experiment-input tests.
- A broader forbidden-term scan including `Simulation.SimpleAccounts` found pre-existing sample factory vocabulary. The recorded passing scan matches the SB13 genericity boundary: simulation abstractions, visualization, WebGL bridge, and sandbox production code.
- No implementation blockers remain.
