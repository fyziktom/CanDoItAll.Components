# Target bridge pipeline

The bridge must live in Economy, not Components.

```text
Experiment input pack JSON files
  -> SimulationExperimentInputPackLoader
  -> SimulationScenarioDefinition + placement + parameters + rules + run plan + visual mapping
  -> ISimulationBackend / transition engine
  -> SimulationFrame + SimulationFrameDelta + SimulationEvent
  -> EconomyVisualFrameMapper / EconomyVisualActionMapper
  -> EconomyVisualFrame + EconomyVisualAction
  -> Economy.Simulation.WebGlBridge
  -> WebGlRunDocument with InitialScene + Timeline + WebGlRunFrames
  -> WebGlRunLib playback/stage/action planning
  -> WebGlLib scene runtime
```

## Required bridge outputs

The bridge output must contain:

- `WebGlRunDocument.RunId`
- `WebGlRunDocument.InitialScene`
- initial scene objects for actors, resources, places, institutions, stores, and links
- `WebGlRunTimeline.Frames`
- frame stages with actual actions or command batches, not metadata-only stages
- stage metadata with source `SimulationEventId`, `EconomyVisualActionId`, `SimulationFrameId`, input pack hash
- visual-state catalog: poses, symbols, bindings
- deterministic source references and content hashes

## Prohibited directions

- Do not make `Components` reference Economy.
- Do not make `Simulation.Abstractions`, `Simulation.SimpleAccounts`, or `Simulation.Visualization` reference Components/WebGL.
- Do not make bridge reference `Simulation.SimpleAccounts` or `Simulation.Ledger`.
- Do not make bridge depend on specific example ids.
