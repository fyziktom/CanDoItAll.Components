# Performance risk register

| Risk | Area | Impact | Proposed mitigation |
| --- | --- | --- | --- |
| Ordered sequence collapsed by batch coalescing | Components/WebGlRunLib | Incorrect visual behavior | Add stage-aware batching and tests. |
| Duplicate motion dropped incorrectly | Components/WebGlLib JS | Actor does not complete route | Deduplicate only within a stage unless explicitly requested. |
| Full object replacement for symbol-only patch | Components/WebGlLib JS | CPU/GPU churn | Add symbol-only update path. |
| Link update scans too many links | Components/WebGlLib JS | Poor performance with many links | Use object-to-link index and metric counts. |
| Continuous render due animated symbols | Components/WebGlLib JS | Idle GPU use | Add symbol animation budget and visible-only animation. |
| Scenario hardcoded frames | Economy/SimpleAccounts | Poor maintainability | Use event stream materialization. |
| Event kind alias drift | Economy/Abstractions | Mapping bugs | Add canonical event kind registry. |
| Visual actions duplicate sequence children | Economy/Visualization | Double execution in bridge | Keep sequence children nested unless debug-expanded. |
| Large file growth | Both repos | Maintainability | Enforce refactoring gates and line thresholds. |
