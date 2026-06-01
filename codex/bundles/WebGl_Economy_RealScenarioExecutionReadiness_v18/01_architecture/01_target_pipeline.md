# Target Generic Pipeline

The target pipeline must stay generic and scenario-independent.

```text
Experiment input pack
  -> canonical input documents
  -> SimulationSandbox session
  -> backend selection
  -> simulation backend materialization
  -> backend-neutral frames/deltas
  -> Economy visual frames/actions
  -> Economy WebGL bridge
  -> WebGlRunDocument
  -> WebGL runtime apply/run
  -> snapshots + analysis artifacts
```

## Correct dependency direction

```text
CanDoItAll.Components.WebGlLib
  <- CanDoItAll.Components.WebGlRunLib
      <- CanDoItAll.Economy.Simulation.WebGlBridge
          <- CanDoItAll.Economy.SimulationSandbox
```

`Components` must never reference Economy.

`Simulation.WebGlBridge` may reference `Simulation.Abstractions`, `Simulation.Visualization`, and `Components.WebGlRunLib` only.

`SimulationSandbox` may compose concrete simulation backends, visualization, bridge and snapshot services.

## Genericity rule

The examples are only probes. Generic source code must not be shaped around words such as:

```text
water, well, farmer, land, parcel, oligarchy, near-household, far-household
```

These words are allowed in fixture JSON, test fixture names, probe tests and human documentation, but not in generic source files.
