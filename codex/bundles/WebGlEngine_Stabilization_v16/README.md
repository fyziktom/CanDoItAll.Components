# CanDoItAll Components WebGL Engine Stabilization Follow-up Bundle v16

## Purpose

This bundle is a Components-only stabilization wave for the `CanDoItAll.Components` repository, branch `webgl-engine`.

The strategic goal is to finish hardening and freezing the generic WebGL/Run engine so later work can move primarily into domain repositories such as `CanDoItAll.Economy` or a future production-line simulator. Codex must not use this bundle to expand generic Components for a single domain use case.

## Scope guard

Allowed repository:

- `fyziktom/CanDoItAll.Components`

Forbidden repositories for implementation in this bundle:

- `fyziktom/CanDoItAll.Economy`
- main `CanDoItAll`
- any future manufacturing/production-line repository

This bundle may discuss future domain consumers, but implementation changes must stay in Components.

## Current-state summary

Current review indicates that the previous stabilization work made important progress:

- `WebGlLib`, `WebGlRunLib`, `WebGlSandbox`, `WebGlLibOnlyViewer`, and `WebGlRunLibGenericSample` are in the solution.
- `IsPackable=false` is now the repository default, with WebGL package projects opt-in.
- `WebGlRunActionKinds` contains generic `DirectedFlowVisual` instead of domain-shaped `ResourceTransferVisual`.
- `IWebGlRunDomainMappingDriver` exists and provides a domain-driver contract with manifest/hash/scrubber/validator.
- Domain-boundary CI exists and has source, public API, package content, and docs/bundle audit profiles.
- Runtime idle policy modes exist and distinguish semantic/visual/final-render-drain semantics.
- Freeze approval tests exist for public API, JS API, package content, generic action kinds, and domain-driver manifest.

## Why another Components-only bundle is still needed

The previous work moved the engine close to release candidate, but several release-candidate risks remain:

1. Freeze tests exist but are still text-scan based and must be hardened into a single RC validation command.
2. `WebGlSceneView.razor` is still a broad boundary component. Public API should remain stable, but implementation should be split internally.
3. JS API approval must validate result semantics, not only method names.
4. Runtime idle policies must be explicitly documented and tested against production-line-style step/cycle visualization.
5. Domain-boundary audit allowlists must not become a way to hide generic/domain leakage.
6. Production-line canary is needed to ensure the engine is not accidentally tuned to Economy examples.
7. Performance/large-scene proof needs to address repeated stations, WIP tokens, overlays, LOD/instancing readiness, and asset lifecycle.
8. Package-mode proof must cover both WebGlLib-only and WebGlRunLib-generic usage.

## Execution rule

Run subbundles in order. After every checkpoint, stop and perform the required review/refactor pass before continuing.

Checkpoint subbundles are gates, not optional documentation.
