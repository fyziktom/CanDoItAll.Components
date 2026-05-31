# SB14 proof manifest

## Scope

Economy shared-well readiness without final demo UI.

## Changed files

- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/shared-well/experiment.json`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/shared-well/scenario.definition.json`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/shared-well/placement.json`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/shared-well/parameters.json`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationExperimentInputTests.cs`

## Proof

- Transcript: `bundle://proof/SB08/transcripts/economy-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB14/semantic-invariants.md`

## Failing-first / semantic proof

`SharedWellInputPack_CompilesRunsAndMapsReadinessFlowWithoutDemoUi` loads the pack, validates inputs, compiles events, runs transitions, maps visual actions, and checks stable hashes without adding a demo UI.

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| Shared-well fixture input pack and transition result | future demo, visualization mapper, proof hashes | versioned fixtures -> compiled event stream -> frames/deltas/visual actions | No final WebGL demo or direct bridge was implemented. |
