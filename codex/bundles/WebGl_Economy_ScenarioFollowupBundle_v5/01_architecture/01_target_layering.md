# Target layering

## Components repo

```text
CanDoItAll.Components.WebGlLib
  Pure renderer / scene engine.
  Knows: scene objects, assets, symbols, layers, patches, motions, command results.
  Does not know: simulation, economy, ledger, citizens, wells, markets.

CanDoItAll.Components.WebGlRunLib
  Generic run/playback layer above WebGlLib.
  Knows: run documents, timelines, frames, generic visual events/actions, action mappings, playback controller.
  Does not know: economy, ledger, accounts, water, well, citizens, markets.

CanDoItAll.Components.WebGlSandbox
  Generic proofs and demos.
  May contain sample demo names, but no reusable library code should depend on demo semantics.
```

## Economy repo

```text
CanDoItAll.Economy.Simulation.Abstractions
  Backend-neutral simulation scenario/frame/event contracts.
  No references to Ledger, BusinessObjects, SDK, Components, WebGL.

CanDoItAll.Economy.Simulation.SimpleAccounts
  Lightweight proof backend for generic community/resource/account scenarios.
  References only Simulation.Abstractions.

CanDoItAll.Economy.Simulation.Ledger
  Ledger-backed adapter.
  References Simulation.Abstractions + ledger stack.

CanDoItAll.Economy.Simulation.Visualization
  Economy visual-intent DTOs and mappers.
  References only Simulation.Abstractions.
  Does not emit WebGL DTOs yet.

Future integration app/package
  Maps EconomyVisualFrame/EconomyVisualAction to WebGlRunDocument/WebGlRunAction.
  This bridge must be separate and explicit.
```

## Integration direction

The eventual runtime flow should be:

```text
SimulationScenarioDefinition
  -> ISimulationBackend
  -> SimulationFrame + SimulationEvent stream
  -> IEconomyVisualFrameMapper
  -> EconomyVisualFrame + EconomyVisualAction stream
  -> future bridge
  -> WebGlRunDocument / WebGlRunAction plan
  -> WebGlSceneView
```
