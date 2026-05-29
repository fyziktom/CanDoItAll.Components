# 01 - Copy-paste Codex master prompt

You are a senior .NET 10 / Blazor / WebGL architect.

Work in repository:

```text
C:\repositories\CanDoItAll.Components
```

Target branch: create a feature branch from current main, for example:

```text
feature/webgl-symbolic-tycoon-sandbox
```

## Goal

Extend the existing `src/CanDoItAll.Components.WebGlLib` with domain-neutral scene/assets/symbols/interaction/interop contracts and add a standalone generic `src/CanDoItAll.Components.WebGlSandbox` project.

The WebGL layer must be sufficient for future "tycoon-like" visualizations: villages, symbolic agents, buildings, objects above heads, color/intensity status indicators, selection, hover, overlays, asset catalog, and deterministic proof snapshots.

## Hard constraints

- Do not add economy-specific concepts.
- Do not add process-specific dependencies.
- Do not reference `CanDoItAll.Modules.Processes`, `CanDoItAll.Economy`, or the main `CanDoItAll` repo.
- Keep `WebGlLib` reusable and domain-neutral.
- Keep existing `WebGlWorkbench` behavior stable.
- Prefer additive generic scene APIs beside the existing workbench APIs.
- All source code comments must be in English.
- All public DTOs must have safe defaults.
- Build must pass with `dotnet build CanDoItAll.Components.slnx`.

## Required implementation

### 1. Inventory and compatibility guard

Inspect current `WebGlLib` and document:

- Existing C# workbench contracts.
- Existing JS runtime files.
- Existing GLB assets under `src/CanDoItAll.Components.WebGlLib/wwwroot`.
- Existing asset generation scripts.
- Whether `WebGlWorkbench` is used by the existing sandbox.

Do not delete or break current workbench files.

### 2. Add generic C# scene contracts

Add files under:

```text
src/CanDoItAll.Components.WebGlLib/WebGl/Scene/
```

Include at least:

```text
WebGlSceneModel.cs
WebGlSceneObject.cs
WebGlSceneLink.cs
WebGlSceneCamera.cs
WebGlSceneLayer.cs
WebGlSceneEnvironment.cs
WebGlSceneSelectionState.cs
WebGlSceneProofSnapshot.cs
```

These must be domain-neutral.

### 3. Add generic asset contracts and services

Add files under:

```text
src/CanDoItAll.Components.WebGlLib/WebGl/Assets/
```

Include at least:

```text
WebGlAssetCatalog.cs
WebGlAssetDefinition.cs
WebGlAssetVariant.cs
WebGlAssetAnimation.cs
WebGlAssetMaterialOverride.cs
IWebGlAssetCatalogProvider.cs
InMemoryWebGlAssetCatalogProvider.cs
WebGlAssetCatalogValidator.cs
```

The asset catalog must support GLB/GLTF models, primitives, materials, thumbnails, tags, license/source metadata, and fallback asset ids.

### 4. Add generic symbol contracts and services

Add files under:

```text
src/CanDoItAll.Components.WebGlLib/WebGl/Symbols/
```

Include at least:

```text
WebGlStatusSymbol.cs
WebGlSymbolAnchor.cs
WebGlSymbolEffect.cs
WebGlSymbolIntensityPolicy.cs
WebGlSymbolPalette.cs
IWebGlSymbolPolicy.cs
DefaultWebGlSymbolPolicy.cs
```

A status symbol must support:

- `SymbolAssetId`
- `SemanticKind`
- `Intensity`
- `Color`
- `Scale`
- `HeightOffset`
- `BillboardToCamera`
- `EffectKey`
- `Tooltip`
- `SortOrder`

This must be generic enough for later economy use but not contain economy terms.

### 5. Add interaction contracts

Add files under:

```text
src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/
```

Include at least:

```text
WebGlHoverState.cs
WebGlSelectionMode.cs
WebGlSelectionChangedEventArgs.cs
WebGlObjectMovedEventArgs.cs
WebGlSceneCommand.cs
WebGlSceneCommandResult.cs
WebGlCameraCommand.cs
WebGlInteractionOptions.cs
```

Do not break existing `WebGlSelectionChangedEventArgs` used by `WebGlWorkbench`; either keep it in place or introduce scene-specific names such as `WebGlSceneSelectionChangedEventArgs`.

### 6. Add interop/runtime contracts

Add files under:

```text
src/CanDoItAll.Components.WebGlLib/WebGl/Interop/
```

Include at least:

```text
WebGlRuntimeOptions.cs
WebGlRuntimeDiagnostics.cs
WebGlRuntimeReadyEventArgs.cs
WebGlRuntimeErrorEventArgs.cs
WebGlRuntimeBridge.cs
```

Add a new generic component:

```text
src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor
```

The component should be additive and should not replace `Components/Workbench/WebGlWorkbench.razor`.

### 7. Add generic JS runtime façade

Add a new generic runtime folder beside the workbench runtime:

```text
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/
```

Required files:

```text
01-webgl-scene.js
02-webgl-scene-core.js
03-webgl-scene-assets.js
04-webgl-scene-symbols.js
05-webgl-scene-interaction.js
06-webgl-scene-camera.js
07-webgl-scene-overlays.js
08-webgl-scene-proof.js
```

Minimum behavior:

- Create/dispose runtime.
- Render scene objects from a `WebGlSceneModel`.
- Load GLB/GLTF via asset catalog.
- Use fallback primitives when a model is missing.
- Render symbols above objects.
- Billboard status symbols to the camera.
- Support color/scale/effect from intensity.
- Support hover and selection.
- Expose diagnostics and deterministic proof snapshot.
- Support fit-view/reset-camera/focus-object.
- Support world-to-screen overlay projection.

The JS global namespace should be separate from the existing workbench runtime:

```js
window.CanDoItAll.webglScene
```

Do not overload or break:

```js
window.CanDoItAll.webglWorkbench
```

### 8. Update WebGlLib assets

Update:

```text
tools/webgllib/asset-manifest.json
tools/webgllib/build-assets.cjs
tools/webgllib/verify-assets.cjs
src/CanDoItAll.Components.WebGlLib/Components/Shared/Assets/WebGlLibBodyAssets.razor
```

so that the new scene runtime script can be loaded safely.

Preferred approach:

- Keep workbench runtime loading unchanged.
- Add parameters to `WebGlLibBodyAssets` such as:
  - `IncludeWorkbenchRuntimeAssets`
  - `IncludeSceneRuntimeAssets`
- Preserve `IncludeRuntimeAssets` as backwards-compatible shorthand if currently used.

### 9. Add standalone WebGL sandbox project

Create:

```text
src/CanDoItAll.Components.WebGlSandbox
```

Use SDK:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
```

Target framework:

```text
net10.0
```

Project references:

```text
../CanDoItAll.Components.BaseLib/CanDoItAll.Components.BaseLib.csproj
../CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj
../CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
```

Do not reference processes/economy/main app modules.

Update:

```text
CanDoItAll.Components.slnx
README.md
```

### 10. Build a generic tycoon village demo

In `CanDoItAll.Components.WebGlSandbox`, add a page such as:

```text
/tycoon-village
```

Create a scene factory:

```text
WebGlSandboxVillageSceneFactory.cs
```

The demo should render a small village using available GLB assets in the repository.

If exact model names differ, Codex must inventory the GLB files first and map the closest available assets to:

- ground/floor/grid
- houses/buildings
- trees/props
- simple people/agents
- status icons/symbols
- marker objects

The scene should include:

- 5-12 buildings/props.
- 5-20 simple agents or placeholder people.
- several status symbols above objects.
- color/intensity variation.
- camera preset suitable for tycoon/isometric view.
- selection and hover.
- overlay inspector showing selected object metadata.
- deterministic proof snapshot panel.

### 11. Validation

Run:

```powershell
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet build CanDoItAll.Components.slnx
```

Then run the sandbox and verify:

```powershell
dotnet run --project src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
```

Browser proof checklist:

- The `/tycoon-village` page loads.
- WebGL canvas appears.
- GLB assets load or fallback primitives render.
- At least one building, one prop, one agent, and one status symbol are visible.
- Hover changes object state.
- Click selects object.
- Inspector updates.
- Fit view/reset camera work.
- Deterministic proof snapshot returns non-empty object/symbol counts.
- No console errors.
- No dependency on process/economy assemblies.

## Deliverables

Commit changes with clear messages and produce an implementation report:

```text
artifacts/webgl-symbolic-tycoon-sandbox/IMPLEMENTATION_REPORT.md
```

The report must include:

- Files added/modified.
- Architecture summary.
- Validation commands and results.
- Known limitations.
- Follow-up recommendations for economy-specific integration.
