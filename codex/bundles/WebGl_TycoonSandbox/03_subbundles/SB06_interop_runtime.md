# SB06 - Generic interop runtime

## Status

Completed. Proof: `bundle://proof/SB06/manifest.md`.

## Goal

Add a separate generic WebGL scene runtime, keeping workbench runtime intact.

## Required C# component

```text
src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor
```

Parameters:

- `Scene`
- `Options`
- `SelectionChanged`
- `HoverChanged`
- `ObjectsMoved`
- `StateChanged`
- `RuntimeReady`
- `RuntimeError`

Public methods:

- `FitViewAsync()`
- `FocusObjectAsync(string objectId)`
- `ResetCameraAsync()`
- `CaptureImageAsync()`
- `GetProofSnapshotAsync()`

## Required JS files

```text
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/01-webgl-scene.js
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/03-webgl-scene-assets.js
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/04-webgl-scene-symbols.js
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/05-webgl-scene-interaction.js
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/06-webgl-scene-camera.js
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/07-webgl-scene-overlays.js
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js
```

JS namespace:

```js
window.CanDoItAll.webglScene
```

Required JS methods:

```js
create(host, dotNetRef, scene, options)
update(host, scene, options)
dispose(host)
fitView(host)
focusObject(host, objectId)
resetCamera(host)
getState(host)
getDiagnostics(host)
getProofSnapshot(host)
exportImageData(host)
```

## Asset loading

Use existing vendored Three.js and GLTFLoader.

Do not introduce CDN dependencies.

## Acceptance criteria

- Scene runtime works even with no GLB assets by using primitive fallbacks.
- Runtime disposes renderer, controls, geometries, materials, and DOM listeners.
- No global state collision with `webglWorkbench`.
