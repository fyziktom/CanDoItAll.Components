You are a senior .NET 10 / Blazor / WebGL / Three.js architect.

Repository:

```text
C:\repositories\CanDoItAll.Components
```

Start from:

```text
codex/webgl-symbolic-tycoon-sandbox
```

Create follow-up branch:

```text
codex/webgl-scene-hardening-run-readiness
```

Harden the generic WebGL scene wrapper and sandbox. Do not add economy/process/game-specific domain concepts.

Required work:

1. Inventory all current WebGL scene files and all GLB/GLTF assets.
2. Add user-provided GLB models as optional asset variants / high-detail profiles, while keeping primitive fallback defaults.
3. Make `WebGlAssetVariant` actually used by runtime.
4. Implement `AllowDragOnGroundPlane` and emit `ObjectsMoved`.
5. Add generic `WebGlScenePatch` and `applyPatch(host, patch)`.
6. Add generic `WebGlObjectMotionCommand` and `enqueueMotion(host, command)`.
7. Add `ExportSceneAsync`, `ApplyPatchAsync`, and `EnqueueMotionAsync` to `WebGlSceneView`.
8. Add render loop modes: `auto`, `continuous`, `on-demand`.
9. Fix create failure reporting so `RuntimeError` is raised if JS create returns false.
10. Remove empty partial `WebGlSceneSelectionState` smell.
11. Refactor large JS/Razor/CSS files before adding more logic.
12. Add unit tests where practical and browser proof for primitive + GLB variant profiles.
13. Produce reports under `artifacts/webgl-scene-hardening`.

Boundary rule:
- `WebGlLib` owns rendering contracts, picking, drag, camera, overlays, asset variants, patching, and transform interpolation primitives.
- Future `WebGlRunLib` owns run clock, scenario playback, event scheduling, persistence adapters, physics/pathfinding, and simulation semantics.
- Domain repos own economy/process/game-specific rules.

Validation:
```powershell
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet build CanDoItAll.Components.slnx
```

All source code comments must be in English.
