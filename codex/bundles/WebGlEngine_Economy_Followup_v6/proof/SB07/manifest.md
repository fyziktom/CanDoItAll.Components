# Proof Manifest for SB07

Status: complete

## Evidence

- Economy focused tests: `proof/SB14/transcripts/economy-focused-tests.txt`
- Economy full test project: `proof/SB14/transcripts/economy-full-test-project.txt`

## Result

Metric and invariant evaluation now validates known kinds in strict mode and reports diagnostics for missing metric ids, unknown metric kinds, unknown invariant kinds, and missing invariant metric references.

## Changed files

- `src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationExperimentPolicies.cs`
- `src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationMetricAndInvariantEvaluation.cs`
- `src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationInvariantEvaluation.cs`
- `src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationExperimentInputPackLoader.cs`
- `tests/CanDoItAll.Economy.Tests/SimulationEconomicTrustHardeningTests.cs`
