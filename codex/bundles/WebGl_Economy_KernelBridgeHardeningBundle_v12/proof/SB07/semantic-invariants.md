# SB07 Semantic Invariants

## INV-SB07-001 Strict Hashes Are Real

- Expected behavior: strict mode accepts only `sha256:` plus 64 lowercase hex characters.
- Negative proof: placeholder and stale hashes fail in `bundle://proof/SB19/transcripts/economy-tests.txt`.

## INV-SB07-002 Paths Stay Under Pack Root

- Expected behavior: traversal and absolute paths are rejected before file reads.
- Source proof: `bundle://proof/SB19/transcripts/source-assertions.txt`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Safe resolved input path | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationExperimentInputPack.cs` | `SimulationExperimentInputPackLoader` | Resolve, verify, read | `bundle://proof/SB19/transcripts/economy-tests.txt` |
