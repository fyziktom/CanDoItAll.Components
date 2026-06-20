# Target architecture

## Layering

```text
CanDoItAll.Components.WebGlLib
  - Renderer-only scene/document/patch/runtime APIs
  - No run semantics
  - No domain words

CanDoItAll.Components.WebGlRunLib
  - Generic run/playback/driver contracts
  - Generic action kinds only
  - Domain driver manifest and opaque provenance support
  - No Economy/production-line/example semantics

CanDoItAll.Economy.Simulation.*
  - Domain simulation semantics
  - Scenario packs, metrics, invariants, oracle corpus
  - Economy domain driver maps Economy visual actions to generic WebGlRun actions

Future domain drivers
  - Production line, logistics, process/workflow, robotics, etc.
  - Use the same generic WebGlRun driver contract
```

## Research truth boundary

Headless simulation artifacts are economic truth. WebGL/browser proof is observer proof only. Browser proof may validate replay and visualization, but must never mutate headless economic validity.
