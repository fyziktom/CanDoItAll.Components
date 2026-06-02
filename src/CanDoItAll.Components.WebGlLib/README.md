# CanDoItAll.Components.WebGlLib

Package version: `0.1.0`.

## Purpose

WebGL workbench concept runtime plus additive generic scene/component contracts for reusable 3D visualization.

## Project Type

- SDK: `Microsoft.NET.Sdk.Razor`
- Target framework(s): `net10.0`
- Validation command:

```powershell
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
```

## References

Project references:

- `../CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj`

Framework references:

- None

Direct package references:

- `Microsoft.AspNetCore.Components.Web (10.0.4)`

## Architecture Notes

Keep shared UI reusable and typed. Use BaseLib for ordinary product UI, CanvasLib for graph/canvas surfaces, OverlayLib for floating windows, WebGlLib for WebGL concepts, and sandbox projects only for demos or proof.

The existing workbench surface remains node/edge oriented and continues to use:

- `WebGlWorkbench`
- `WebGlWorkbenchSurface`
- `window.CanDoItAll.webglWorkbench`

The generic scene layer is additive and uses:

- `WebGlSceneModel`, `WebGlSceneObject`, `WebGlSceneLink`, and `WebGlSceneProofSnapshot`
- `WebGlAssetCatalog`, `WebGlAssetVariant`, `WebGlAssetPerformanceHint`, and `IWebGlAssetCatalogProvider`
- `WebGlStatusSymbol` and `IWebGlSymbolPolicy`
- `WebGlScenePatch` and `WebGlObjectMotionCommand`
- `WebGlSceneView`
- `window.CanDoItAll.webglScene`

To include runtime assets:

```razor
<WebGlLibHeadAssets />
<WebGlLibBodyAssets IncludeRuntimeAssets="true" IncludeSceneRuntimeAssets="true" />
```

`IncludeRuntimeAssets` remains the backwards-compatible shorthand for the workbench runtime. Use `IncludeSceneRuntimeAssets` when a page hosts `WebGlSceneView`.

Minimal WebGlLib-only consumption is covered by `samples/CanDoItAll.Components.WebGlLibOnlyViewer`, which references this package and renders a primitive `WebGlSceneModel` without any `WebGlRunLib` dependency:

```powershell
dotnet build samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj
```

## Adding Assets

Place GLB/GLTF files under `wwwroot/assets`, add logical ids to an app-owned `WebGlAssetCatalog`, and provide primitive fallbacks for model categories that may be missing. Run:

```powershell
npm run webgllib:build-assets
npm run webgllib:verify-assets
```

The runtime loads GLB/GLTF assets asynchronously and renders fallback primitives when a model id or URI cannot be resolved. Asset variants support `primitive`, `model-low`, `model-medium`, and `model-high` quality tiers, with runtime selection by explicit object metadata or scene/runtime profile.

Use `WebGlModelImportOptions` on assets or variants to tune generic model import behavior: unit scale, fit mode, center mode, rotation/position offsets, double-sided material normalization, debug bounds, material visibility normalization, and tint disabling. Runtime diagnostics report empty scenes, mesh/material counts, zero or extreme bounds, invisible meshes, transparent materials, invalid transforms, and camera clipping risk.

Asset cache entries are state-local by default. Cached GLB/GLTF templates own their geometry, material, and texture disposal when the scene state is disposed; cloned/tinted model instances own cloned material objects but retain shared template texture maps unless a texture was explicitly cloned and marked owned. Use `assetCacheMode`, `assetCacheEntryCount`, `assetCacheHitCount`, `assetCacheMissCount`, `disposedTemplateCount`, `disposedTextureCount`, and `retainedSharedTextureCount` to verify repeated import/profile/dispose behavior.

## Runtime Hardening

`WebGlSceneView` now exposes export/import, patch, object transform, detailed patch results, and detailed motion methods. The JavaScript runtime supports drag-on-ground-plane for draggable objects, transform-only patches without full scene rebuilds, smooth render-layer motion, `auto`/`continuous`/`on-demand` render modes, idle render-loop sleeping, explicit resource ownership/disposal, model diagnostics, and create/runtime diagnostics.

`WebGlSceneDocument` is the generic save/load contract for scene layouts. It preserves scene data, runtime options, saved timestamp, source, metadata, and a deterministic content hash without adding storage providers or run semantics.

Use `WebGlSceneDocumentSerializer.Validate` for persisted documents and `WebGlSceneModelValidator` for live scenes. Scene objects are the canonical source for layer membership; layer `ObjectIds` are validated as view/grouping references and stale or duplicate layer entries are reported as warnings.

`WebGlSceneModel.Revision` is the canonical scene content and patch revision. `WebGlScenePatch` reducers resolve legacy payloads from `UiState.Revision` only when the top-level revision is zero, and successful mutating patches mirror the committed revision back to `UiState.Revision` for runtime compatibility. Scene content hashing uses `Scene.Revision` and ignores UI-only revision, hover, and selection state; document hashing still covers included UI state.

Patch diagnostics classify incremental updates as `transform-only`, `symbol-only`, `link-only`, `visual-replace`, `mixed-incremental`, `graph-structure`, or `scene-rebuild`. Use `fullSceneRebuildCount`, `transformOnlyPatchCount`, `symbolOnlyPatchCount`, `linkOnlyPatchCount`, `linkGeometryUpdateCount`, and `lastPatchClassification` to prove hot patch paths avoided full scene rebuilds.

Run the scene runtime audit before widening JavaScript changes:

```powershell
npm run webgllib:audit-scene-runtime-imports
npm run webgllib:audit-scene-runtime
npm run webgllib:test-resource-ownership
```

`WebGlSceneCommandBatch` and its command stages are render-command transport for the scene runtime. They may batch patches, motions, waits, and render-idle barriers, but they must not define run documents, replay lifecycle, scenario lifecycle, domain events, persistence providers, or domain action semantics.

`WebGlLib` remains a render substrate. Simulation clocks, run lifecycle, pathfinding, physics, persistence providers, economy rules, and domain semantics belong in `WebGlRunLib` or a consuming domain package.

## Related Docs

- Repository overview: `README.md` at this repo root
- Run-layer boundary: `docs/webgl/run-layer-boundary.md`
- Main repo shared component docs: `C:\repositories\CanDoItAll\docs\ui-shared-components\README.md`
