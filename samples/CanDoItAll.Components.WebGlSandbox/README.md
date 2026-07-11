# CanDoItAll.Components.WebGlSandbox

The WebGl Sandbox is a standalone Blazor app for exploring and validating generic WebGlLib scenes. Use it to inspect runtime behavior, asset fallbacks, interaction, export/import, and visual proof without adding 3D concerns to the standard component Sandbox.

## Routes

- `/` links to available proof pages.
- `/tycoon-village` renders a domain-neutral tycoon-style village with primitive, mixed GLB, and high-detail GLB asset profiles, drag/move, motion, export/import, missing-asset fallback, and deterministic proof snapshots.
- `/asset-catalog` lists logical asset ids, variants, repository GLB mappings, primitive fallbacks, tags, and source metadata.

## Run

```powershell
dotnet run --project samples/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
```

## Adding A Demo

Create a scene factory that returns `WebGlSceneModel`, add a page that hosts `WebGlSceneView`, and keep domain-specific mapping outside `CanDoItAll.Components.WebGlLib`.

## Asset Strategy

This sandbox is intentionally tuned and validated for large-screen proof work only. Do not spend bundle time on small-screen layout tuning until that becomes an explicit requirement.

`WebGlSandboxAssetCatalogFactory` maps the repository GLB inventory plus `3DModels/glb` into generic logical ids and supplies primitive fallbacks for every model category. Use `npm run webgllib:inventory-glb` to refresh `artifacts/webgl-scene-hardening/glb-inventory.json`.
