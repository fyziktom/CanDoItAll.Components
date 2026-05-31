# SB12 proof manifest

## Scope

Economy random input generation policy.

## Changed files

- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationInputGeneration.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationPlacementAndParameters.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Hashing/SimulationDeterministicHash.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationExperimentInputTests.cs`

## Proof

- Transcript: `bundle://proof/SB08/transcripts/economy-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB12/semantic-invariants.md`

## Failing-first / semantic proof

`RandomInputGenerator_WritesReplayablePlacementJson` proves seeded random placement generation is an input-preparation step and the saved JSON replays deterministically.

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| `DeterministicSimulationPlacementGenerator` | saved placement JSON and hash validator | generator request + seed -> generated placement JSON -> simulation input | Runtime simulation consumes saved JSON, not a hidden random generator. |
