# SB09 proof manifest

## Scope

Economy canonical scenario model and alias cleanup.

## Changed files

- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Hashing/SimulationDeterministicHash.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Scenario/SimulationScenarioValidation.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationPreparationTests.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationExperimentInputTests.cs`

## Proof

- Transcript: `bundle://proof/SB08/transcripts/economy-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB09/semantic-invariants.md`

## Failing-first / semantic proof

`ScenarioNormalizer_UsesCanonicalCollectionsAndReportsAliasConflicts` proves aliases remain compatibility input only and canonical collections drive validation/hash behavior.

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| `SimulationScenarioDefinitionNormalizer` canonical scenario | validators, policies, event compiler, deterministic hash | alias-compatible JSON -> normalized canonical scenario -> validation/hash/event stream | Alias conflicts emit diagnostics and alias order cannot silently change the hash. |
