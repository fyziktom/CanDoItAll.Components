# Generic Simulation-to-Visualization Bridge Pipeline

## Desired generic pipeline

```text
Experiment input pack JSONs
  -> strict input pack loader
  -> scenario definition + placement + parameters + rules + run plan
  -> backend selection
  -> simulation run frames/deltas
  -> optional pause snapshot
  -> EconomyVisualFrame/EconomyVisualAction
  -> EconomyWebGlBridge
  -> WebGlRunDocument + InitialScene + Timeline Frames + Stages
  -> WebGlSceneView / WebGL runtime
```

## Bridge responsibilities

The bridge must:
- map visual nodes to WebGL scene objects,
- preserve node/object mapping for later actions,
- map visual links to WebGL links,
- map visual symbols to WebGL status symbols,
- map visual actions to generic WebGL actions,
- plan actions into patches/motions,
- emit ordered stages with source provenance,
- keep unresolved mappings as diagnostics rather than silently falling back,
- preserve hashes and source IDs for traceability.

## Bridge must not

- load simulation input packs,
- run the simulation backend,
- know about shared wells, farmers, taxes, or land,
- reference SimpleAccounts or Ledger,
- render UI directly.
