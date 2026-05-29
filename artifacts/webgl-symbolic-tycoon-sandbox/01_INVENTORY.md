# WebGL Symbolic Tycoon Sandbox Inventory

## Existing WebGlLib File Map

### C# DTOs and Events

- `src/CanDoItAll.Components.WebGlLib/WebGl/WebGlWorkbenchSurface.cs`
- `src/CanDoItAll.Components.WebGlLib/WebGl/WebGlWorkbenchUiState.cs`
- `src/CanDoItAll.Components.WebGlLib/WebGl/WebGlWorkbenchEvents.cs`

### Razor Components

- `src/CanDoItAll.Components.WebGlLib/Components/Workbench/WebGlWorkbench.razor`
- `src/CanDoItAll.Components.WebGlLib/Components/Shared/Assets/WebGlLibHeadAssets.razor`
- `src/CanDoItAll.Components.WebGlLib/Components/Shared/Assets/WebGlLibBodyAssets.razor`

### Runtime JS

- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/workbench/01-webgl-workbench.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/workbench/02-webgl-workbench-core.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/workbench/03-webgl-workbench-overlays.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/workbench/04-webgl-workbench-chrome.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/workbench/05-webgl-workbench-interaction.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/workbench/06-webgl-workbench-camera.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/workbench/07-webgl-workbench-scene-graph.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/workbench/08-webgl-workbench-hit-testing.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/workbench/09-webgl-workbench-actions.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/workbench/10-webgl-workbench-drag.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/workbench/11-webgl-workbench-anchor-flow.js`

### CSS

- `src/CanDoItAll.Components.WebGlLib/wwwroot/css/workbench/webgl-workbench.css`

### Vendored Runtime Assets

- `src/CanDoItAll.Components.WebGlLib/wwwroot/vendor/three.module.min.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/vendor/three.core.min.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/vendor/OrbitControls.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/vendor/GLTFLoader.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/vendor/utils/SkeletonUtils.js`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/vendor/utils/BufferGeometryUtils.js`

### Existing GLB Assets

- `src/CanDoItAll.Components.WebGlLib/wwwroot/assets/model/1gears.glb`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/assets/model/gears.glb`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/assets/model/lowpoly_person_boxing.glb`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/assets/model/question_box.glb`

### Asset Scripts

- `tools/webgllib/asset-manifest.json`
- `tools/webgllib/build-assets.cjs`
- `tools/webgllib/verify-assets.cjs`
- `tools/convert_fbx_to_glb.py`

## Current Public APIs That Must Not Break

- `WebGlWorkbench`
- `WebGlWorkbenchSurface`
- `WebGlWorkbenchNode`
- `WebGlWorkbenchEdge`
- `WebGlWorkbenchAnchor`
- `WebGlWorkbenchUiState`
- `WebGlSelectionChangedEventArgs`
- `WebGlNodeMovedEventArgs`
- `WebGlLibHeadAssets`
- `WebGlLibBodyAssets`
- `window.CanDoItAll.webglWorkbench`

## Current Usage Notes

- `WebGlWorkbench.razor` invokes `CanDoItAll.webglWorkbench.create`, `update`, `fitView`, `focusNode`, `exportImageData`, `resetView`, and related workbench functions.
- The existing workbench runtime stores state on `host.__webglWorkbenchState` and exposes `window.CanDoItAll.webglWorkbench`.
- The existing generated body asset component loads only `js/runtime/workbench/01-webgl-workbench.js`.
- `CanDoItAll.Components.WebGlSandbox` did not exist before this bundle execution.

## Proposed New File List

- `src/CanDoItAll.Components.WebGlLib/WebGl/Scene/*`
- `src/CanDoItAll.Components.WebGlLib/WebGl/Assets/*`
- `src/CanDoItAll.Components.WebGlLib/WebGl/Symbols/*`
- `src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/*`
- `src/CanDoItAll.Components.WebGlLib/WebGl/Interop/*`
- `src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/*`
- `src/CanDoItAll.Components.WebGlLib/wwwroot/css/scene/webgl-scene.css`
- `src/CanDoItAll.Components.WebGlSandbox/*`

## Architecture Guardrails

- The existing workbench runtime remains stable and keeps the `window.CanDoItAll.webglWorkbench` namespace.
- The new scene runtime is additive and uses `window.CanDoItAll.webglScene`.
- Future domain-specific visualization should consume the generic scene contracts from outside WebGlLib.
- Production code in this bundle must not add economy/process-specific terms or project references.

