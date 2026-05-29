# CanDoItAll.Components.WebGlSandbox

Standalone Blazor Web App for validating generic WebGlLib scene rendering.

## Routes

- `/` links to available proof pages.
- `/tycoon-village` renders a domain-neutral tycoon-style village with buildings, props, agents, status symbols, selection, camera commands, and deterministic proof snapshots.
- `/asset-catalog` lists logical asset ids, repository GLB mappings, primitive fallbacks, tags, and source metadata.

## Run

```powershell
dotnet run --project src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
```

## Adding A Demo

Create a scene factory that returns `WebGlSceneModel`, add a page that hosts `WebGlSceneView`, and keep domain-specific mapping outside `CanDoItAll.Components.WebGlLib`.

## Asset Strategy

`WebGlSandboxAssetCatalogFactory` maps the repository GLB inventory to generic logical ids and supplies primitive fallbacks for categories without exact models. Current GLB inventory:

- `1gears.glb`
- `gears.glb`
- `lowpoly_person_boxing.glb`
- `question_box.glb`

