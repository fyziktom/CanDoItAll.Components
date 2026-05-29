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

## Adding Assets

Place GLB/GLTF files under `wwwroot/assets`, add logical ids to an app-owned `WebGlAssetCatalog`, and provide primitive fallbacks for model categories that may be missing. Run:

```powershell
npm run webgllib:build-assets
npm run webgllib:verify-assets
```

The runtime loads GLB/GLTF assets asynchronously and renders fallback primitives when a model id or URI cannot be resolved. Asset variants support `primitive`, `model-low`, `model-medium`, and `model-high` quality tiers, with runtime selection by explicit object metadata or scene/runtime profile.

## Runtime Hardening

`WebGlSceneView` now exposes export/import, patch, object transform, and motion methods. The JavaScript runtime supports drag-on-ground-plane for draggable objects, transform-only patches without full scene rebuilds, smooth render-layer motion, `auto`/`continuous`/`on-demand` render modes, and explicit create/runtime diagnostics.

`WebGlLib` remains a render substrate. Simulation clocks, run lifecycle, pathfinding, physics, persistence providers, economy rules, and domain semantics belong in a future `WebGlRunLib` or consuming domain package.

## Related Docs

- Repository overview: `README.md` at this repo root
- Main repo shared component docs: `C:\repositories\CanDoItAll\docs\ui-shared-components\README.md`
