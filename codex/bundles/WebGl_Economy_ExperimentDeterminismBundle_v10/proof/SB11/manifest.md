# SB11 proof manifest

## Scope

Economy placement and parameter JSON files.

## Changed files

- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationPlacementAndParameters.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Hashing/SimulationDeterministicHash.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/shared-well/placement.json`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/shared-well/parameters.json`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationExperimentInputTests.cs`

## Proof

- Transcript: `bundle://proof/SB08/transcripts/economy-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB11/semantic-invariants.md`

## Failing-first / semantic proof

`PlacementAndParameters_LoadValidateHashAndApplyToScenario` proves placement and parameter JSON can be loaded, validated, hashed, and applied to a canonical scenario.

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| `SimulationPlacementDefinition` and `SimulationParameterSetDefinition` | placement/parameter validator, scenario application, hash | versioned JSON -> validated input -> applied canonical scenario | Placement and parameter changes alter the hash; hidden random state is not used by the run. |
