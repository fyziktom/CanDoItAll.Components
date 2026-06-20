# Current-state analysis after v15

## Positive findings

### Package scope

`Directory.Build.props` now defaults `IsPackable=false`, while `WebGlLib` and `WebGlRunLib` opt in with `IsPackable=true`. This is a good NuGet boundary because sandbox and sample projects are not accidentally published.

### Generic action vocabulary

`WebGlRunActionKinds` now contains `DirectedFlowVisual` and exposes an `All` set. This is an appropriate generic vocabulary because it supports visual flows without implying economics, resources, inventory, buyer/seller, work orders, or a specific simulator.

### Domain-driver contract

`IWebGlRunDomainMappingDriver` exists with:

- driver id
- driver version
- display name
- boundary options
- driver action kinds
- driver hash
- manifest
- mapping function
- metadata scrubber
- validation

This is the right extensibility point for Economy and future production-line simulators.

### Runtime idle

The runtime idle system distinguishes:

- semantic idle
- visual idle
- final render drained

This can support deterministic proof, fast UI responsiveness, and visual proof modes.

### Approval tests

There are freeze approval tests for public API, package content, JS surface, JS API manifest, action vocabulary, and domain-driver manifest.

## Remaining risks

### RC validation is fragmented

There is no single `rc-validate` command that proves the whole WebGL engine can be frozen.

### `WebGlSceneView.razor` is still too broad

It remains the main boundary for lifecycle, JS interop, event callbacks, import/export, command execution, runtime stop/idle, and external import keying. Public API should not be broken, but internals should be split.

### JS API contract still needs stronger semantics

A method list is not enough. Each JS method needs result shape, missing-runtime behavior, lifecycle state behavior, failure semantics, and cancellation/idle semantics.

### Production-line canary is still missing

Economy is no longer enough to prove genericity. A future manufacturing simulator will stress:

- repeated stations
- conveyors
- buffers
- WIP tokens
- machine state overlays
- alarms
- cycle timing
- operator intervention
- route/path visualization
- small interactive controls

The generic engine should support these via neutral objects, links, status symbols, layers, directed flows, patches, motions, and domain-driver mappings.

### Domain-boundary audits may drift

Allowlisted historical docs and bundles are acceptable as soft audit, but production source/package hard gates must remain strict.

### Instancing and LOD are not yet first-class

The current model can render primitives and GLB assets, but large manufacturing layouts will expose the need for repeated-object strategy, pooled symbols, batching, and eventually instancing/LOD diagnostics.
