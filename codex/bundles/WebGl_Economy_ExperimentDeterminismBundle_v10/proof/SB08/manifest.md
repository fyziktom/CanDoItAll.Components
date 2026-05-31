# SB08 proof manifest

## Scope

Economy experiment input pack contracts.

## Changed files

- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationExperimentInputPack.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Hashing/SimulationDeterministicHash.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationExperimentInputTests.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/shared-well/experiment.json`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/farmer-land/experiment.json`

## Proof

- Transcript: `bundle://proof/SB08/transcripts/economy-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB08/semantic-invariants.md`

## Failing-first / semantic proof

`ExperimentInputPack_HashesInputDocumentsButIgnoresUiPlaybackSettings` proves deterministic input hashes include referenced experiment documents while excluding playback speed/UI settings.

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| `SimulationExperimentInputPack` and `SimulationInputHashManifest` | deterministic hash and validators | versioned JSON input refs -> canonical hash -> run/proof metadata | UI playback setting changes do not affect the input-pack hash. |
