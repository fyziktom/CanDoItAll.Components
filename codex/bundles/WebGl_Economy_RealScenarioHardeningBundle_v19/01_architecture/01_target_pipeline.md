# Target pipeline

The target architecture should remain:

```text
Experiment input pack
  -> SimulationExperimentInputPackLoader
  -> IEconomySimulationBackendSelector
  -> IEconomySimulationBackend.Materialize(...)
  -> EconomySimulationBackendResult
  -> IEconomyVisualizationPipeline
  -> EconomyVisualFrame[]
  -> IEconomyWebGlProjectionPipeline
  -> WebGlRunDocument
  -> WebGlRun playback/runtime
  -> IEconomySnapshotPipeline
  -> SimulationRunSnapshot[]
  -> ISimulationSnapshotAnalysisService
```

Repository boundary:

```text
CanDoItAll.Components
  WebGlLib
  WebGlRunLib
  NO Economy references

CanDoItAll.Economy
  Simulation.Abstractions
  Simulation.SimpleAccounts
  Simulation.Ledger
  Simulation.Visualization
  Simulation.WebGlBridge
  SimulationSandbox
```

The joined pipeline belongs in `CanDoItAll.Economy.SimulationSandbox` and later a UI app/component inside Economy, not in Components.
