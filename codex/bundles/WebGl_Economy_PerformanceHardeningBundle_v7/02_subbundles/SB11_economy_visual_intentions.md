# SB11 - Economy visual intentions, no WebGL dependency

## Goal

Add temporal visual intention DTOs in `Simulation.Visualization`.

## Required contracts

```text
EconomyVisualAction
EconomyVisualActionKind
EconomyVisualTargetRef
EconomyVisualPoseIntent
EconomyVisualSymbolIntent
EconomyVisualActionFrame
IEconomyVisualActionMapper
```

## Generic visual action kinds

- `move-to-target`
- `return-to-anchor`
- `perform-at-target`
- `change-pose`
- `show-symbol`
- `hide-symbol`
- `pulse-link`
- `resource-transfer-visual`
- `wait`
- `apply-scene-patch`

## Important

These are not WebGL types. They are economy visualization intentions. The future bridge can map them to `WebGlRunAction`, but not in this wave.
