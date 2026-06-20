# Target Architecture: Components Freeze

```text
CanDoItAll.Components.WebGlLib
  - Generic scene model/document contracts
  - Generic asset catalog and primitive/model fallback contracts
  - WebGlSceneView public component boundary
  - JS runtime: window.CanDoItAll.webglScene
  - Patch/motion/command batch runtime
  - Runtime idle/stop/proof diagnostics
  - No run lifecycle, no domain semantics

CanDoItAll.Components.WebGlRunLib
  - Generic run document/timeline/frame/action/stage contracts
  - Action compiler/planner and browser apply adapter
  - Domain driver interface and driver manifest validation
  - Observer proof contracts
  - No Economy/market/ledger/production-line semantics

Domain packages, e.g. Economy
  - Map domain events/visual frames to generic action kinds
  - Own raw domain provenance and trace maps
  - Own scenario/oracle/metric semantics
  - Own domain-specific visualization choices
```

After this bundle, Components should be treated as release-candidate infrastructure. Economy may consume it, but should not require Components changes unless a true generic bug or approved generic feature gap is found.
