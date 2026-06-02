# SB11 Scenario Inventory

## Built-In SimpleAccounts Examples

| Scenario | Generic contract surface | Scenario-specific surface | SB11 conclusion |
| --- | --- | --- | --- |
| `simple.shared-well-community` | `SimulationScenarioDefinition` actors, locations, resources, stores, relationships, rules, scheduled events, aliases, deterministic metadata. | The factory seeds named actors/resources and hand-authored frame states for the current example. | Remains an example fixture. Generic scheduled events and visual action mapping are not hard-coded into WebGlBridge. |
| `simple.small-entrepreneur-community` | Same generic scenario definition, manifest, frame, delta, flow, issue, and event contracts as shared-well. | The factory seeds local-market actors/resources and hand-authored frame states for the current example. | Remains an example fixture. Generic materialization remains available through `SimpleStateTransitionMaterializerHandler`. |

Evidence:

- `src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationScenarioFactory.cs` lists the current built-in definitions.
- `src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleScenarioDefinitionMaterializer.cs` keeps named factories in `SimpleScenarioFactoryMaterializerHandler` and falls through to `SimpleStateTransitionMaterializerHandler` for generic scheduled-event definitions.
- `tests/CanDoItAll.Economy.Tests/SimulationPreparationTests.cs` round-trips both definitions and maps both through generic visual frames.

## Experiment Input Fixtures

| Fixture | Experiment id | Generic visual mapping evidence | Browser/readiness evidence |
| --- | --- | --- | --- |
| `shared-well` | `experiment.shared-well.distance-trade-tax.v1` | `visual.mapping.json` maps event kinds such as `actor.resource.use`, `actor.trade.sell`, `rule.enforcement.apply`, and `relationship.trust.change` to generic visual action kinds. | `RealScenarioReadinessReporter_AnswersRequiredQuestionsWithArtifactCitations` confirms strict headless run/projection readiness. |
| `farmer-land` | `experiment.farmer-land.anti-oligarchy.v1` | `visual.mapping.json` maps land/market/rule/risk categories and action kinds through the same generic mapping schema. | Same readiness probe confirms strict headless run/projection readiness. |

SB11 conclusion:

- Vernon-Smith-style examples are carried as data packs plus `visual-mapping/v1`, not bridge code branches.
- The bridge consumes `EconomyVisualFrame` and `EconomyVisualMappingDefinition`; it does not switch on `shared-well`, `farmer-land`, or built-in scenario names.
- Browser playback is ready for smoke planning at the generated artifact level, but the Economy browser host route/actions remain explicitly missing and are not claimed as passed in SB11.

## Large Generic Scale Proof

The SB11 large proof builds a synthetic shared-resource scenario through generic scenario definitions and scheduled events, then proves:

- 101 actors, 1 shared resource, 50 scheduled steps.
- 5,000 scheduled events, 15,000 compiled events, 15,000 visual actions.
- 51 WebGlRun frames, 15,000 WebGlRun stages, 10,000 motions.
- Strict WebGlRun validation succeeds.
- Deterministic replay fingerprint matches across projection runs.

Artifact: `proof/SB11/artifacts/large-generic-webglrun-proof.json`.
