# SB04 — Economy bridge initial scene projector

## Problem
`EconomyWebGlRunProjector` currently creates frames/stages without building a real initial scene or node/object mapping.

## Tasks
- Add `EconomyWebGlInitialSceneProjector`.
- Map `EconomyVisualFrame.Nodes` to `WebGlSceneObject`.
- Map `EconomyVisualFrame.Links` to `WebGlSceneLink`.
- Populate `EconomyWebGlMappingContext.NodeObjectIds`.
- Use `visual.mapping.json` for asset ids, pose keys, symbol keys, anchors, and object categories.
- Add a diagnostic object only when explicitly configured; do not silently route unresolved nodes to empty fallback.

## Tests
- shared-well visual frame creates actor, place/resource, institution objects without using well-specific code.
- farmer-land visual frame creates actors/resources/finite-spatial-resource objects with the same projector.
