# Proof manifest SB08

Status: Completed

## Scope

Reusable production snapshot builder contracts and services: snapshot build request/result, builder interface, provenance builder, deterministic snapshot output, optional visual/runtime inputs, and stable data hash when runtime diagnostics change.

## Changed Files

- `economy://src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationSnapshotBuilder.cs`
- `economy://tests/CanDoItAll.Economy.Tests/SimulationSnapshotBuilderTests.cs`

SHA-256 hashes:

- `bundle://proof/SB08/hashes/sha256.txt`

## Command Transcripts

- Failing-first builder contract scan: `bundle://proof/SB08/transcripts/failing-first-snapshot-builder-contracts.txt`
- Snapshot-focused tests: `bundle://proof/SB08/transcripts/simulation-snapshot-tests.txt`
- Full Economy tests: `bundle://proof/SB08/transcripts/economy-tests.txt`
- Economy simulation boundary audit: `bundle://proof/SB08/transcripts/economy-boundary-audit.txt`
- Bundle prepared validator after SB08: `bundle://proof/SB08/transcripts/bundle-validator-prepared-after-sb08.txt`

## Source Assertions

- Snapshot builder source map: `bundle://proof/SB08/source-assertions/snapshot-builder-source-map.txt`
- Anti-stub scan: `bundle://proof/SB08/source-assertions/anti-stub-scan.txt`
- Production contracts exist: `ISimulationSnapshotBuilder`, `SimulationSnapshotBuildRequest`, `SimulationSnapshotBuildResult`, and `SimulationSnapshotProvenanceBuilder`.
- Builder accepts scenario, run, current frame, last delta, pending/applied events, metrics, invariants, optional visual state, and runtime diagnostics without a WebGL dependency.
- Provenance contains `scenario`, `frame`, optional `lastDelta`, and `snapshot.data` hashes.
- `snapshot.data` ignores runtime diagnostics, while the full snapshot deterministic hash still captures runtime visual state.

## Semantic Adequacy Gate

- Shallow-pass trap: directly constructing snapshots in tests can produce a deterministic hash without a reusable production builder or separated data/runtime hash semantics.
- Adversarial negative proof: `bundle://proof/SB08/transcripts/failing-first-snapshot-builder-contracts.txt` records that required builder contracts were absent before SB08.
- Semantic positive proof: `bundle://proof/SB08/transcripts/simulation-snapshot-tests.txt` proves shared-resource, finite-resource, and runtime-diagnostic hash-stability cases; `bundle://proof/SB08/transcripts/economy-tests.txt` proves the wider suite remains green.
- Boundary proof: `bundle://proof/SB08/transcripts/economy-boundary-audit.txt` records `PASS: Economy simulation boundary audit passed.`
- Anti-stub audit: `bundle://proof/SB08/source-assertions/anti-stub-scan.txt` records no placeholder markers in changed builder/test files.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Snapshot builder contracts | `SimulationSnapshotBuilder.cs` | SB09/SB10/SB12/SB13/SB14 snapshot workflows | Request/result abstractions and `ISimulationSnapshotBuilder` produce a `SimulationRunSnapshot` with warnings/errors. | Failing-first scan and source assertions. |
| Provenance hashes | `SimulationSnapshotProvenanceBuilder` | Snapshot analysis/store/downstream proof | Scenario/frame/last-delta/data hashes are attached before the final deterministic snapshot hash is refreshed. | Shared and finite snapshot builder tests. |
| Runtime-insensitive data hash | `SimulationSnapshotProvenanceBuilder.BuildDataHash` | Snapshot comparisons and analysis services | `snapshot.data` excludes visual state and runtime diagnostics so runtime telemetry can vary without changing data identity. | Runtime diagnostics hash-stability test. |

## Failures / Blockers

- No SB08 blocker.
- Economy test transcripts include existing package warnings (`NU1701`, `NU1510`) and existing nullable warnings in unrelated files.
