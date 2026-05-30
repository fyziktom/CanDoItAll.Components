# Target generic pipeline

The intended architecture must remain generic:

```text
ScenarioDefinition
  -> ScenarioLoader
  -> ScenarioValidator
  -> EventCompiler
  -> Backend Materializer
  -> SimulationFrame / SimulationFrameDelta / SimulationEventStream
  -> EconomyVisualFrame / EconomyVisualAction
  -> future bridge: EconomyVisualAction -> WebGlRunAction
  -> WebGlRunDocument / WebGlRunFrame
  -> WebGlSceneCommandBatch
  -> WebGlLib runtime
```

## Important separation

### Components repo

Components may know:
- generic scenes;
- objects;
- anchors;
- run frames;
- generic visual actions;
- command batches;
- motion/patch commands.

Components must not know:
- water;
- wells;
- citizens;
- tax;
- compliance;
- ledger;
- account balances;
- economy semantics.

### Economy repo

Economy may know:
- actor/resource/rule/market semantics;
- simple accounts;
- ledger projections;
- scenario definitions;
- visual intentions.

Economy must not reference:
- `CanDoItAll.Components.*`;
- `WebGlSceneModel`;
- `WebGlRunAction`;
- JS runtime names.
