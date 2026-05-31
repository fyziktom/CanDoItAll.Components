# SB16 - Refactor Large Files Gate

## Goal

Prevent the new foundation from becoming unmaintainable.

## Target files to split or monitor

Economy:

- `SimulationScenarioDefinitionNormalizer.cs` (currently broad)
- `SimulationScenarioPolicies.cs` (currently too broad)
- `SimulationDeterministicHash.cs` (many hash projections)
- `SimpleSimulationStateTransitionEngine.cs` (large and likely to grow)
- `SimulationExperimentInputTests.cs` and `SimulationPreparationTests.cs` (very large tests)

Components:

- `WebGlSceneCommandBatch.cs` (large but acceptable if split into normalizer, models, metrics)
- `28-webgl-scene-command-batch-normalizer.js` (near warning threshold)
- `14-webgl-scene-motion.js` (watch if queue support is added)
- `WebGlRunActionPlanner.cs`

## Required gate

Create or update audits to fail if:

- JS runtime file > 320 lines
- public facade > 180 lines
- C# production file > 350 lines unless explicitly allowed
- test file > 500 lines unless split is planned
