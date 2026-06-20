# Proof Manifest for SB08

Status: complete

## Evidence

- Economy focused tests: `proof/SB14/transcripts/economy-focused-tests.txt`
- Economy full test project: `proof/SB14/transcripts/economy-full-test-project.txt`

## Result

Behavior expansion is now controlled by explicit profiles: `none`, `simple-need`, `trade-policy-v1`, and `all-v1`. Generated events carry `expansionProfileId` and `parentEventId` provenance. The focused suite proves disabled expansion remains single-event while simple need expands deterministically.

## Changed files

- `src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationExperimentPolicies.cs`
- `src/CanDoItAll.Economy.Simulation.Abstractions/Events/SimulationEventStream.cs`
- `src/CanDoItAll.Economy.Simulation.Abstractions/Scenario/SimulationScenarioValidation.cs`
- `tests/CanDoItAll.Economy.Tests/SimulationEconomicTrustHardeningTests.cs`
