# Source references used for this bundle

## Components

- `CanDoItAll.Components.slnx` — WebGlRunLib is included in the solution.
- `src/CanDoItAll.Components.WebGlRunLib/WebGlRunContracts.cs` — run document/timeline/frame/playback contracts.
- `src/CanDoItAll.Components.WebGlRunLib/WebGlRunActions.cs` — generic event/action/target/pose/symbol contracts.
- `src/CanDoItAll.Components.WebGlRunLib/WebGlRunPlaybackController.cs` — reusable playback controller.
- `src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlSceneCommandBatch.cs` — C# command batch model/normalizer.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/26-webgl-scene-command-batch.js` — JS command batch application.
- `tools/webgllib/audit-scene-runtime.cjs` — runtime audit, domain-neutrality and large-screen guard.
- `docs/webgl/large-screen-only-policy.md` — desktop/large-screen-only rule.

## Economy

- `src/CanDoItAll.Economy.Simulation.Abstractions/Scenario/SimulationScenarioDefinition.cs` — current scenario definition model.
- `src/CanDoItAll.Economy.Simulation.Abstractions/Scenario/SimulationScenarioValidation.cs` — serializer/loader/store/validator.
- `src/CanDoItAll.Economy.Simulation.Abstractions/Events/SimulationEvent.cs` — event model/taxonomy.
- `src/CanDoItAll.Economy.Simulation.Abstractions/Events/SimulationEventStream.cs` — event compiler.
- `src/CanDoItAll.Economy.Simulation.Abstractions/Hashing/SimulationDeterministicHash.cs` — deterministic hashes.
- `src/CanDoItAll.Economy.Simulation.SimpleAccounts/SharedWellCommunityScenarioFactory.cs` — shared-well readiness seed.
- `src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleScenarioDefinitionMaterializer.cs` — current scenario-id materializer.
- `src/CanDoItAll.Economy.Simulation.Visualization/EconomyVisualAction.cs` — visual action DTOs.
- `src/CanDoItAll.Economy.Simulation.Visualization/EconomyVisualActionMapper.cs` — event-to-action mapping.

## CanDoItAll bundle workflow inspiration

- `codex/skills/bundles/candoitall-bundle-workflow/SKILL.md`
- `codex/skills/bundles/candoitall-bundle-workflow/references/workflow-decision-tree.md`
