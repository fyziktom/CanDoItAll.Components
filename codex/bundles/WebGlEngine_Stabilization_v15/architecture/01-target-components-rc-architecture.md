
# Target Components RC architecture

```text
WebGlLib
  - WebGlSceneModel / WebGlSceneDocument
  - WebGlSceneView public component
  - JS runtime window.CanDoItAll.webglScene
  - asset catalog, symbols, patches, command batches, motion, diagnostics
  - no run documents, no simulation semantics, no domain semantics

WebGlRunLib
  - WebGlRunDocument / WebGlRunTimeline / WebGlRunFrame
  - generic actions and stages
  - playback controller and browser apply adapter
  - domain mapping driver contract
  - no Economy/manufacturing semantics

Domain package (outside Components)
  - maps domain events/states to generic WebGlRunDocument
  - owns domain driver, trace map, semantics, metrics, oracles
```

Release candidate requires all three boundaries to be enforceable by tests, package references, API snapshots, JS surface manifests, and domain leakage audits.
