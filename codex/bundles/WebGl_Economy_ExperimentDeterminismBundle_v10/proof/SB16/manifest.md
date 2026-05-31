# SB16 proof manifest

## Scope

Economy visual action ordering and policy.

## Changed files

- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Visualization/EconomyVisualAction.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Visualization/EconomyVisualActionMapper.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Visualization/EconomyVisualActionNormalizer.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationExperimentInputTests.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationPreparationTests.cs`

## Proof

- Transcript: `bundle://proof/SB08/transcripts/economy-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB16/semantic-invariants.md`

## Failing-first / semantic proof

`VisualActionNormalizer_RemovesNestedChildrenUnlessMarkedStandalone` proves nested sequence children are not duplicated as top-level actions unless explicitly marked standalone.

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| `EconomyVisualActionNormalizer` | `EconomyVisualActionMapper` and future bridge | mapped visual intentions -> stable ordered/deduplicated action stream | Nested sequence children are not executed twice by default. |
