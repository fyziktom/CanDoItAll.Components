# SB09 Semantic Invariants

Status: Completed

## Invariants

| Invariant ID | Expected behavior | Shallow-pass trap | Adversarial negative proof | Semantic positive proof | Anti-stub audit |
|---|---|---|---|---|---|
| SB09-I1 | Snapshots serialize a structured visual runtime attachment with playback command, motion queue, barrier state, command journal tail, render diagnostics, and selected ids. | A shallow implementation could add only a generic diagnostics dictionary or test-only JSON fields. | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-failing-first.txt` shows the committed baseline lacked the required contract fields. | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-tests.txt` validates round-trip serialization, and `bundle://proof/SB09/transcripts/real-artifact-runtime-field-assertions.txt` validates real scenario artifacts. | `bundle://proof/SB09/transcripts/anti-stub-audit.txt` |
| SB09-I2 | Data-state hash remains stable when runtime diagnostics change. | A shallow implementation could include runtime diagnostics in the data hash and still emit two hash strings. | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-failing-first.txt` shows the baseline lacked `snapshot.dataState` naming and visual-runtime hash separation. | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-tests.txt` asserts equal `snapshot.dataState` values across changed runtime diagnostics. | `bundle://proof/SB09/transcripts/anti-stub-audit.txt` |
| SB09-I3 | Visual-runtime hash changes when runtime diagnostics or runtime attachment details change. | A shallow implementation could hard-code the visual hash or compute it from visual frame ids only. | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-failing-first.txt` shows the baseline lacked `HashVisualRuntimeState`. | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-tests.txt` asserts unequal `snapshot.visualRuntime` values after runtime diagnostics change. | `bundle://proof/SB09/transcripts/anti-stub-audit.txt` |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| `SimulationSnapshotRuntimeAttachment` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationRunSnapshot.cs` | Snapshot serializer, runtime analysis, readiness checks | Attached to `SimulationSnapshotVisualState` and cloned by `SimulationSnapshotBuilder` | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-failing-first.txt` |
| `snapshot.dataState` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationSnapshotBuilder.cs` | Snapshot diff and analysis consumers | Built from data-state only, excluding runtime visual state | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-tests.txt` |
| `snapshot.visualRuntime` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Hashing/SimulationDeterministicHash.Snapshot.cs` | Snapshot analysis and readiness consumers | Built from optional visual/runtime state and stored separately from data-state hash | `bundle://proof/SB09/transcripts/snapshot-runtime-attachment-tests.txt` |
| Real artifact `visualState.runtimeAttachment` | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlSnapshotVisualStateBuilder.cs` | SB10 snapshot analyzers and SB12 readiness report | Derived from WebGL run frames during Economy snapshot pipeline export | `bundle://proof/SB09/transcripts/real-artifact-runtime-field-assertions.txt` |

## Contract

SB09 is closed only if the snapshot model exposes every required runtime field, real scenario snapshot artifacts serialize those fields, `snapshot.dataState` stays stable across runtime-only changes, `snapshot.visualRuntime` changes across runtime-only changes, and the source audit contains no fixture-specific or stubbed runtime attachment path.
