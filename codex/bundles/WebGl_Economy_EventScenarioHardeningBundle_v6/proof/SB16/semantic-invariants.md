# SB16 Semantic Invariants

Status: Completed

## Components Invariants

| Invariant | Evidence |
| --- | --- |
| `SB16-COMP-ACTION-001`: WebGL run actions express generic operations only: move, return, pose, symbol, sequence, parallel, and patch. | `WebGlRunActions.cs`, `WebGlRunActionPlanning.cs`, `source-assertions.txt` |
| `SB16-COMP-TARGET-002`: Action targets resolve through explicit coordinates, object IDs, object anchors, metadata anchors, or built-in geometric anchors without Economy concepts. | `WebGlSceneObject.cs`, `WebGlRunActionPlannerTests.cs` |
| `SB16-COMP-BATCH-003`: Scene command batches coalesce object patches and dedupe duplicate motions unless explicitly allowed. | `WebGlSceneCommandBatch.cs`, `WebGlSceneCommandBatchTests.cs`, `26-webgl-scene-command-batch.js` |
| `SB16-COMP-PLAYBACK-004`: Run playback is controller-owned and applies compiled frames through a frame applier, not page-local per-action loops. | `WebGlRunPlaybackController.cs`, `RunPlayback.razor.cs` |
| `SB16-COMP-RENDER-005`: WebGL scene renders actual scene content in desktop and mobile browser viewports. | Desktop/mobile screenshots and pixel audit under `artifacts/scenario-followup` |

## Economy Invariants

| Invariant | Evidence |
| --- | --- |
| `SB16-ECON-SCENARIO-001`: Simulation scenario definitions expose backend-neutral entities, places, stores, behaviors, and event templates. | `SimulationScenarioDefinition.cs`, `SimulationPreparationTests.cs` |
| `SB16-ECON-MATERIALIZER-002`: SimpleAccounts can materialize generic scenario definitions and frame deltas without Components references. | `SimpleScenarioDefinitionMaterializer.cs`, Economy tests |
| `SB16-ECON-EVENT-003`: Simulation events expose generic actors, places, resources, quantities, durations, and step indices; deterministic hashes include those fields. | `SimulationEvent.cs`, `SimulationDeterministicHash.cs`, Economy tests |
| `SB16-ECON-VISUAL-004`: Economy visual actions are intentions with generic targets, pose hints, symbol hints, and nested sequence/parallel steps. | `EconomyVisualAction.cs`, `EconomyVisualActionMapper.cs`, Economy tests |
| `SB16-ECON-LEDGER-005`: Ledger projections can emit generic simulation events without depending on SimpleAccounts. | `LedgerSimulationEventProjector.cs`, boundary scan |

## Boundary Invariants

| Invariant | Evidence |
| --- | --- |
| `SB16-BOUNDARY-001`: Components source and tests contain no `CanDoItAll.Economy` references. | `cross-repo-boundary-scan.txt` |
| `SB16-BOUNDARY-002`: Economy Simulation source contains no `CanDoItAll.Components`, `WebGl`, `WebGL`, or `Three` references. | `cross-repo-boundary-scan.txt`, `economy-boundary-audit.txt` |
| `SB16-BOUNDARY-003`: SimpleAccounts does not reference Ledger, BusinessObjects, or Sdk; Ledger does not reference Simulation.SimpleAccounts. | `cross-repo-boundary-scan.txt` |

## Browser Rendering Invariants

| Viewport | Expected | Observed |
| --- | --- | --- |
| Desktop `1440x1000` | Nonblank WebGL grid and scenario objects | Grid, blue runner marker, green goal marker, connection/path, and overlay rendered |
| Mobile `390x844` | Same core model content visible without layout collapse | Same scene content visible and responsive |

The screenshots were visually inspected, and the pixel audit confirms varied nonblank rendering in both captures.
