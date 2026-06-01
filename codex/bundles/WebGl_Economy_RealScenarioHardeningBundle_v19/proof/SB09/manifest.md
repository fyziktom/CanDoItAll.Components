# SB09 Proof Manifest

Status: Completed

## Scope

SB09 proves Economy snapshots carry visual runtime attachment details without losing deterministic data-state hash separation. The runtime attachment includes playback command, active and queued motion ids, barrier policy and blockers, command journal tail, render diagnostics summary, and optional UI selected object/action ids.

## Evidence

| Evidence | Path | Result |
|---|---|---|
| Failing-first committed-baseline runtime attachment audit | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-failing-first.txt` | Failed as expected |
| Snapshot runtime attachment tests | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-tests.txt` | Passed |
| Real artifact runtime field assertions | `bundle://proof/SB09/transcripts/real-artifact-runtime-field-assertions.txt` | Passed |
| Source assertions | `bundle://proof/SB09/transcripts/source-assertions.txt` | Passed |
| Anti-stub audit | `bundle://proof/SB09/transcripts/anti-stub-audit.txt` | Passed |
| Changed file and artifact hashes | `bundle://proof/SB09/transcripts/changed-file-hashes.txt` | Captured |

## Source References

- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationRunSnapshot.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationSnapshotBuilder.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Hashing/SimulationDeterministicHash.Snapshot.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlSnapshotVisualStateBuilder.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationSnapshotTests.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationSnapshotBuilderTests.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyRealProbeArtifactExporterTests.cs`

## Real Scenario Artifact References

- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/shared-well/snapshots/snapshot.run.simple.shared-well-community.state-transition.1.json`
- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/farmer-land/snapshots/snapshot.run.probe.farmer-land.state-transition.1.json`

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `SimulationSnapshotRuntimeAttachment` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationRunSnapshot.cs` | Snapshot serializer, snapshot analysis, readiness probes | Attached under `visualState.runtimeAttachment` and serialized with the snapshot payload | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-failing-first.txt` shows the committed baseline lacked every required field |
| `snapshot.dataState` provenance hash | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationSnapshotBuilder.cs` | Snapshot diff, analysis, readiness, artifact proof | Built from data-only snapshot content with runtime metadata excluded | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-tests.txt` proves the data hash stays stable when runtime diagnostics change |
| `snapshot.visualRuntime` provenance hash | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Hashing/SimulationDeterministicHash.Snapshot.cs` | Snapshot analysis and readiness consumers that need runtime-state drift | Built from the optional visual/runtime attachment | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-tests.txt` proves the visual-runtime hash changes when runtime diagnostics change |
| Real-scenario runtime attachment JSON | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlSnapshotVisualStateBuilder.cs` | SB10/SB12 artifact readers | Derived from the projected WebGL run frame during the snapshot pipeline | `bundle://proof/SB09/transcripts/real-artifact-runtime-field-assertions.txt` proves both required scenarios serialize the fields and hashes |

## Closure

The SB09 gate passed. Deterministic data-state hashing remains stable across runtime diagnostic changes, visual-runtime hashing changes when runtime state changes, and regenerated real-scenario snapshots carry the required runtime attachment fields.
