# SB17 — Cross-repo bridge design only

## Goal
Design but do not yet implement the direct bridge.

Future bridge:

```text
EconomyVisualAction
  -> WebGlRunAction
  -> WebGlRunFrame
  -> WebGlSceneCommandBatch
  -> WebGlSceneView
```

## Current wave
- Document mapping rules.
- Add DTO compatibility notes.
- Do not add `CanDoItAll.Components.*` references to Economy.
- Do not add Economy references to Components.
