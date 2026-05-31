# SB08 Semantic Invariants

## INV-SB08-001 One Loader Owns The Pipeline

- Expected behavior: shared-well and farmer-land packs both move through the same loader path.
- Positive proof: `bundle://proof/SB19/transcripts/economy-tests.txt`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Loaded experiment pipeline | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationExperimentInputPackLoader.cs` | Readiness and bridge contracts | Pack JSON to compiled event stream | `bundle://proof/SB19/transcripts/economy-tests.txt` |
