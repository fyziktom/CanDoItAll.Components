# Can We Run Real Tests Now?

## Yes: headless scenario tests

The current foundation appears ready for real **headless** scenario tests of the existing fixture scenarios:

- shared-resource / shared-well style scenario
- finite-resource / farmer-land style scenario

A real headless test should perform this pipeline:

```text
experiment.json
  -> load strict input pack
  -> apply placement and parameter documents
  -> select simulation backend
  -> materialize frames/deltas
  -> build visual frames
  -> project WebGlRunDocument
  -> validate stage commands
  -> build snapshots
  -> export artifacts
  -> analyze snapshot states
```

This is a real test of the simulation/visualization join even without a browser.

## Not yet: full interactive visual demo

A polished interactive browser demo should wait until:

- WebGlRunDocument can be applied by a reusable runtime runner/controller with clear frame/stage semantics.
- Stage barriers are proven with a runtime or browser-like harness.
- Snapshot capture can attach actual runtime diagnostics, active stages, pending stages, active motions, and pending motions.
- Economy-side SimulationSandbox has stable session APIs for load, seek, step, play, pause, snapshot, analyze, export.

## Practical next milestone

The next milestone should be a reproducible headless artifact run:

```text
artifacts/economy/real-probe/shared-resource/
  input-pack.validation.json
  simulation.frames.json
  visual.frames.json
  webgl.run-document.json
  snapshots/*.json
  analysis/*.json
  readiness-report.json

artifacts/economy/real-probe/finite-resource/
  ...same structure...
```

Only after this should a large-screen UI demo be built.
