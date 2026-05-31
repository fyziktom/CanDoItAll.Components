# SB13 proof manifest

## Scope

Economy generic simple state transition engine.

## Changed files

- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationStateTransitionEngine.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Scenario/SimulationScenarioValidation.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationPreparationTests.cs`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationExperimentInputTests.cs`

## Proof

- Transcript: `bundle://proof/SB08/transcripts/economy-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB13/semantic-invariants.md`

## Failing-first / semantic proof

`SimpleStateTransitionEngine_AppliesCollectTradeFeeAndRuleEvents` and `SharedWellInputPack_CompilesRunsAndMapsReadinessFlowWithoutDemoUi` prove the generic transition path consumes events and emits frames/deltas for shared-well-style flows.

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| `SimpleSimulationStateTransitionEngine` | frame/delta materialization and visualization mapping | initial state + ordered event stream -> generic state update -> frame/delta | Shared-well fixture no longer requires a final UI/demo to prove transition readiness. |
