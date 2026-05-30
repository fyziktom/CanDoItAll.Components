# SB14 - Future bridge design only

## Goal

Design the future mapping, but do not implement repository coupling yet.

## Future bridge package

Possible future package:

```text
CanDoItAll.Economy.Simulation.WebGlBridge
```

or in a higher-level app/integration repo.

## Future mapping

```text
EconomyVisualAction
  -> WebGlRunAction
  -> WebGlRunActionPlanner
  -> WebGlSceneCommandBatch
```

## Forbidden now

- No `CanDoItAll.Components.*` reference from `Simulation.Visualization`.
- No Economy reference from `WebGlRunLib`.
- No WebGL-specific fields in Economy DTOs.
