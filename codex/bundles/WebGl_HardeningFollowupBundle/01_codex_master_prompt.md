# 01 - Copy-paste Codex master prompt

You are a senior .NET 10 / Blazor / WebGL / Three.js architect.

Work in repository:

```text
C:\repositories\CanDoItAll.Components
```

Start from branch:

```text
codex/webgl-symbolic-tycoon-sandbox
```

Create a new follow-up branch, for example:

```text
codex/webgl-scene-hardening-run-readiness
```

## Mission

Harden the generic WebGL scene wrapper and sandbox implemented in `codex/webgl-symbolic-tycoon-sandbox`.

Do **not** add economy-specific or process-specific concepts.

Prepare the generic wrapper for future tycoon-like visualizations and for a later "run" / light-game layer, where a scene can be loaded, saved, patched, and advanced over time by object motion commands.

## Non-negotiable constraints

- Keep `CanDoItAll.Components.WebGlLib` domain-neutral.
- Do not reference `CanDoItAll`, `CanDoItAll.Economy`, `CanDoItAll.Modules.Processes`, or any process/economy projects.
- Keep existing `WebGlWorkbench` behavior stable.
- Keep the new `WebGlSceneView` additive and compatible.
- All source code comments must be in English.
- Avoid giant files. Split code before files become hard to maintain.
- Build must pass:
  - `npm run webgllib:build-assets`
  - `npm run webgllib:verify-assets`
  - `dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj`
  - `dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj`
  - `dotnet build CanDoItAll.Components.slnx`

## Required hardening

### 1. Inventory current branch

Inspect and report:

- Current `WebGlLib` scene contracts.
- Current JS scene runtime file sizes.
- Current sandbox pages and factories.
- All GLB/GLTF assets under the repo, especially user-provided models not currently referenced by the sandbox.
- The current asset catalog entries and whether they are primitive/model/variant.
- Current proof snapshots and browser proof route.

Create:

```text
artifacts/webgl-scene-hardening/01_INVENTORY.md
```

Include a table:

```text
asset file | size | logical proposed id | recommended use | fallback id | status
```

### 2. Add user-provided GLB models as alternatives, not replacements

Do not remove the primitive fallback village.

Add optional model-based alternatives:

- Keep current primitive houses/trees as low-cost default fallback.
- Add discovered GLB models as asset variants or separate assets.
- Use a quality profile:
  - `primitive`
  - `model-low`
  - `model-high`
- The sandbox must allow switching asset strategy at runtime.

Add DTO support if missing:

```csharp
public sealed class WebGlAssetPerformanceHint
{
    public int TriangleCountHint { get; set; }
    public int VertexCountHint { get; set; }
    public long ByteSizeHint { get; set; }
    public string QualityTier { get; set; } = "unknown";
    public double RecommendedMaxInstanceCount { get; set; }
}
```

Extend `WebGlAssetVariant` with:

```csharp
public string QualityTier { get; set; } = "default";
public string Format { get; set; } = WebGlAssetFormats.Glb;
public string PrimitiveKind { get; set; } = WebGlPrimitiveKinds.Box;
public string FallbackAssetId { get; set; } = string.Empty;
public WebGlAssetPerformanceHint PerformanceHint { get; set; } = new();
```

Add a runtime variant resolver. It must support:
- explicit variant id from object metadata,
- scene/runtime quality profile,
- fallback to primitive.

### 3. Implement drag-on-ground-plane correctly

Current contracts include `AllowDragOnGroundPlane` and `ObjectsMoved`, but runtime does not implement object move.

Implement:

- pointer down on selectable/draggable object,
- ray/ground-plane intersection,
- drag threshold,
- update transform visually during drag,
- commit event `OnObjectsMoved`,
- cancel on Escape or lost pointer,
- no drag when camera orbit is active,
- respect `sceneObject.IsDraggable` and `scene.Interaction.AllowDragOnGroundPlane`.

Keep this generic.

### 4. Add scene patch / command API

Do not implement a full game engine inside `WebGlLib`.

Add generic render-layer commands:

```csharp
public sealed class WebGlScenePatch
{
    public string SceneId { get; set; } = string.Empty;
    public int BaseRevision { get; set; }
    public int NextRevision { get; set; }
    public List<WebGlSceneObjectPatch> ObjectPatches { get; set; } = [];
    public List<WebGlSceneObject> AddObjects { get; set; } = [];
    public List<string> RemoveObjectIds { get; set; } = [];
    public List<WebGlSceneLink> AddLinks { get; set; } = [];
    public List<string> RemoveLinkIds { get; set; } = [];
}
```

Add runtime JS method:

```text
CanDoItAll.webglScene.applyPatch(host, patch)
```

Do not rebuild the whole scene for simple transform changes.

### 5. Add motion/tween primitive, but not full simulation semantics

Add generic object motion command:

```csharp
public sealed class WebGlObjectMotionCommand
{
    public string ObjectId { get; set; } = string.Empty;
    public WebGlVector3 TargetPosition { get; set; } = WebGlVector3.Zero;
    public double SpeedUnitsPerSecond { get; set; }
    public double DurationSeconds { get; set; }
    public string Easing { get; set; } = "linear";
    public bool SnapAtEnd { get; set; } = true;
}
```

Add runtime JS method:

```text
CanDoItAll.webglScene.enqueueMotion(host, command)
```

This belongs in `WebGlLib` because it is a render-layer transform interpolation primitive.

Do **not** add:
- pathfinding,
- physics,
- collision,
- economy semantics,
- game rules.

Those belong to a future `WebGlRunLib` or consuming domain module.

### 6. Add export/import scene state

Add methods on `WebGlSceneView`:

```csharp
Task<WebGlSceneModel?> ExportSceneAsync();
Task ApplyPatchAsync(WebGlScenePatch patch);
Task EnqueueMotionAsync(WebGlObjectMotionCommand command);
```

Runtime should return the current scene model including:
- object positions,
- camera state,
- selection,
- UI state revision,
- active asset strategy / quality profile.

### 7. Refactor JS runtime into maintainable modules

Target max file size guideline:

```text
Ordinary JS module: <= 250 lines
Complex orchestrator: <= 320 lines
Razor page: <= 180 lines
CSS file: split above ~250 lines
```

Refactor at least:

```text
01-webgl-scene.js
03-webgl-scene-assets.js
src/CanDoItAll.Components.WebGlSandbox/Components/Pages/TycoonVillage.razor
src/CanDoItAll.Components.WebGlSandbox/wwwroot/sandbox-webgl.css
```

Suggested split:

```text
scene/01-webgl-scene-api.js
scene/02-webgl-scene-core.js
scene/03-webgl-scene-lifecycle.js
scene/04-webgl-scene-graph.js
scene/05-webgl-scene-assets.js
scene/06-webgl-scene-primitives.js
scene/07-webgl-scene-symbols.js
scene/08-webgl-scene-interaction.js
scene/09-webgl-scene-drag.js
scene/10-webgl-scene-camera.js
scene/11-webgl-scene-overlays.js
scene/12-webgl-scene-patching.js
scene/13-webgl-scene-motion.js
scene/14-webgl-scene-proof.js
```

Avoid breaking asset-manifest generation.

### 8. Improve render-loop policy

Add runtime option:

```csharp
public string RenderMode { get; set; } = WebGlRenderModes.Auto;
```

Supported modes:

```text
auto
continuous
on-demand
```

Behavior:
- `continuous`: render every frame.
- `on-demand`: render only after invalidation.
- `auto`: continuous while active symbol effects, active motion commands, or camera damping are active; otherwise on-demand.

### 9. Fix create failure reporting

If JS create fails:
- JS should call `OnRuntimeError` when possible.
- `WebGlSceneView` should invoke `RuntimeError` if JS returns `false`.
- Include failure detail in diagnostics.

### 10. Remove empty partial class smell

There is an empty partial `WebGlSceneSelectionState` under `WebGl/Interaction`. Remove or replace it with a meaningful type organization. Prefer a single source file unless there is a clear reason for partial classes.

### 11. Add tests and browser proof

Add unit tests if there is a test project; otherwise create a minimal test project if repo conventions allow it.

Required test targets:
- `WebGlAssetCatalogValidator`
- `DefaultWebGlSymbolPolicy`
- scene patch reducer / patch model validation
- asset variant resolver

Browser proof:
- `/tycoon-village` default primitive profile.
- `/tycoon-village` model/high profile using discovered GLB alternatives.
- select object from canvas.
- drag a draggable object and verify `ObjectsMoved`.
- enqueue motion command and verify smooth position change/proof snapshot.
- export scene, reload/import, verify positions persist.
- verify `window.CanDoItAll.webglWorkbench` still exists.
- verify `window.CanDoItAll.webglScene` exists.

### 12. Documentation and report

Create:

```text
artifacts/webgl-scene-hardening/IMPLEMENTATION_REPORT.md
artifacts/webgl-scene-hardening/RUN_LAYER_BOUNDARY.md
artifacts/webgl-scene-hardening/ASSET_VARIANT_INVENTORY.md
artifacts/webgl-scene-hardening/VALIDATION.md
```

The report must explicitly answer:

1. Which features remain in `WebGlLib`?
2. Which features should move to future `WebGlRunLib`?
3. Which external/user GLB models were detected?
4. Which models are enabled by default?
5. Which models are optional/high-detail alternatives?
6. What is the fallback strategy if models are too heavy?
7. What validation was executed?
8. Did existing `WebGlWorkbench` still work?
