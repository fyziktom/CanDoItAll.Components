# Current State After Codex

## Positive changes observed

- The solution now explicitly contains both generic packages (`WebGlLib`, `WebGlRunLib`) and two samples: a WebGlLib-only viewer and a WebGlRunLib generic sample.
- `WebGlRunActionKinds` exposes an approved-looking generic action vocabulary and now includes `DirectedFlowVisual` rather than an economy-shaped transfer action.
- `IWebGlRunDomainMappingDriver` exists and provides driver id/version, boundary options, action mappings, metadata scrubber, manifest, and driver hash.
- `WebGlRunDocumentValidator` now scans many more run/scene fields for domain leakage, including run id, initial scene document fields, scene metadata, object ids/kinds/families/tags, anchors, symbols, links, layers, and frame/stage metadata.
- CI contains a domain leakage workflow using a Node auditor and a configurable term registry.
- `applyCommandBatchAndWait` can hard-fail when runtime idle proof fails.
- Runtime idle state now distinguishes semantic idle, visual idle, and final-render-drained state.
- `WebGlRunLib` README documents observer proof, domain driver versioning, playback hosting, and boundary responsibilities.

## Remaining architectural gaps

1. There is no explicit public API approval baseline for C# contracts or JS `window.CanDoItAll.webglScene` methods.
2. Global `IsPackable=true` can make package scope ambiguous. Only intended deliverable packages should be packable by default; samples/sandboxes/tests should be explicitly non-packable unless intentionally packaged.
3. `WebGlSceneView.razor` is too large and owns multiple responsibilities: rendering lifecycle, JS interop, import/export, command application, stop/idle, callbacks and external import lifecycle. It needs a freeze-safe facade split.
4. Runtime idle has useful nuanced states but lacks explicit `semanticOnly`, `visualStrict`, and `allowFinalRenderDrain` modes in a stable C# and JS contract.
5. `source.*` provenance remains a special channel. For a frozen generic engine, provenance should support an opaque mode where raw domain ids are stored in consumer-owned trace-map artifacts rather than the generic run document.
6. `WebGlRunPassThroughDomainMappingDriver` can return arbitrary action kinds. A freeze gate should define whether pass-through is only for already-approved generic action kinds.
7. `WebGlRunLibGenericSample` is currently a project-reference sample only. It needs package-mode proof.
8. Domain leakage audit allowlists are useful but broad. Freeze should split hard source/package gates from historical bundle/docs gates.
