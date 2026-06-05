# Target architecture: Components freeze + Economy research stack

## Layering target

```text
CanDoItAll.Components.WebGlLib
  - generic scene/document/runtime rendering
  - canvas/webgl runtime JS
  - object/link/symbol/asset abstractions
  - no domain words
  - no run semantics required for simple viewers

CanDoItAll.Components.WebGlRunLib
  - generic run/playback/contracts over WebGlLib
  - generic action kinds only
  - domain mapping driver abstractions
  - public API freeze after this bundle
  - no Economy dependency or example-shaped behavior

Domain driver packages
  - map domain visual actions to generic run actions
  - own forbidden term registry for the domain
  - own source trace-map artifacts
  - own semantic interpretation

CanDoItAll.Economy.Simulation.*
  - Economy simulation abstractions/backends/visualization
  - Economy WebGL bridge driver
  - headless economic truth
  - scenario catalog, oracle corpus, design matrix, readiness
```

## Freeze policy

After SB08 passes:

- Do not add new `WebGlRunActionKinds` unless:
  - a non-economy canary needs it,
  - a public API review approves it,
  - SemVer impact is documented,
  - approval snapshots are updated intentionally.
- Do not add domain-specific metadata keys to generic source provenance.
- Do not add Economy-specific runtime behavior to Components.
- Do not change JS public API without updating JS API approval snapshots and browser proof.
- Economy can keep adding scenarios, drivers, metrics, oracles, and semantic handlers without
  reopening generic Components.

## Domain-driver pattern

A domain driver must define:

- driver id/version/display name
- driver action kind list
- mapping to generic `WebGlRunActionKinds`
- boundary options/forbidden terms
- metadata scrubber behavior
- manifest and deterministic hash
- optional trace-map writer outside generic run document

A driver must not mutate generic runtime behavior. It translates domain intent into generic visual
commands and carries provenance only through approved source metadata or external trace maps.
