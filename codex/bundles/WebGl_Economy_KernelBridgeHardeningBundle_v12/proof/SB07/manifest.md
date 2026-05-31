# SB07 Proof Manifest

Status: Completed

## Scope

Economy experiment input packs now support strict SHA-256 validation while retaining loose example-pack compatibility.

## Evidence

| Artifact | Purpose |
|---|---|
| `bundle://proof/SB19/transcripts/economy-tests.txt` | Includes strict-mode tests for placeholder rejection, real hashes, stale hashes, traversal paths, and absolute paths. |
| `bundle://proof/SB19/transcripts/changed-file-hashes.txt` | Hashes for strict validator source and tests. |
| `bundle://proof/SB19/transcripts/source-assertions.txt` | Source assertions for `StrictHashValidation` and safe path resolution. |

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Strict input hash validation | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationExperimentInputPack.cs` | Experiment loader and validator callers | Load/validate experiment pack before simulation | `bundle://proof/SB19/transcripts/economy-tests.txt` |
