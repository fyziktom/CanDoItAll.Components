# Generic scenario/event/visual/run pipeline

## Layer boundaries

```text
Economy Simulation.Abstractions
  Defines scenario, event stream, frames, deltas and validation.

Economy Simulation.SimpleAccounts
  Materializes generic scenario definitions using simple account/resource stores.

Economy Simulation.Ledger
  Projects ledger snapshots/events into the shared simulation frame model.

Economy Simulation.Visualization
  Maps frames/events into economy visual DTOs. No Components/WebGL reference.

Future bridge project, not in this wave
  Maps Economy visual DTOs to Components WebGlRunLib DTOs.

Components WebGlRunLib
  Knows only generic run actions, target anchors, poses, symbols, patches and motions.

Components WebGlLib
  Knows only rendering, scene patches, motion commands, picking and diagnostics.
```

## Canonical event-action pipeline

```text
SimulationScenarioDefinition
  -> SimulationScenarioDefinitionNormalizer
  -> SimulationScenarioEventCompiler
  -> ISimulationStateTransitionEngine
  -> SimulationFrame / SimulationFrameDelta
  -> EconomyVisualFrame / EconomyVisualActionTimeline
  -> future bridge
  -> WebGlRunDocument / WebGlRunActionTimeline
  -> WebGlSceneCommandBatch
  -> WebGl scene runtime
```

## Stage safety

Never flatten ordered actions into a single unordered batch. A sequence must carry stage metadata:

```text
sequenceId
stageIndex
startsAtSeconds
durationSeconds
allowDuplicateMotionsPerObject
batchingPolicy = preserve-order
```

The batch normalizer may coalesce commands only within the same stage when order does not matter.
