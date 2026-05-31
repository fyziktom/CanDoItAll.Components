# SB12 Semantic Invariants

## INV-SB12-001 Experiment Interpretation Is Explicit

- Expected behavior: metrics and invariants are data-defined and generic across examples.
- Positive proof: `bundle://proof/SB19/transcripts/economy-tests.txt`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Concentration and semantic invariant results | `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationInvariantEvaluation.cs` | Readiness artifact | Evaluate frame, emit pass/fail | `bundle://artifacts/economy/readiness/shared-well-and-farmer-land-readiness.json` |
