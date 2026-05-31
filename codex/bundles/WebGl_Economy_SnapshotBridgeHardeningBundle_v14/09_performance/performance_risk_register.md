# Performance Risk Register

| Risk | Area | Mitigation |
|---|---|---|
| O(n^2) visual action mapping | Economy WebGlBridge | Use node/object dictionaries and cache action mapping lookups |
| Oversized JS modules | WebGL runtime | Keep audit thresholds; split warning-sized files before adding behavior |
| Motion queue runaway | WebGL runtime | Max queue length diagnostics and cancellation/clear tests |
| Stage runner infinite render | WebGL scheduler | Explicit stage runner detection and idle proof |
| Snapshot export too large | Economy snapshots | Provide compact snapshot options and full snapshot option |
| Generic model overfitting | Economy abstractions | Shared-resource + finite-resource probes in tests |
| Direct sibling Components path breaks CI | Economy bridge | Conditional ProjectReference/package strategy |
