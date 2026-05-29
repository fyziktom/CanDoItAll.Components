# 05 - Runtime JS design

## Existing workbench runtime

The current runtime is:

```text
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/workbench/
```

It exposes:

```js
window.CanDoItAll.webglWorkbench
```

Keep it stable.

## New generic scene runtime

Add:

```text
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/
```

Expose:

```js
window.CanDoItAll.webglScene
```

## Minimum runtime state

```js
const state = {
    host,
    dotNetRef,
    scene,
    renderer,
    camera,
    controls,
    viewport,
    raycaster,
    sourceScene,
    renderScene,
    assetCatalog,
    assetCache: new Map(),
    objectGroups: new Map(),
    objectHitMeshes: [],
    symbolGroups: new Map(),
    selectedObjectIds: new Set(),
    hoveredObjectId: null,
    diagnostics,
    handlers,
    scheduleRender
};
```

## Asset loading rules

- Cache loaded GLB templates by URI.
- Clone templates for instances.
- Mark cloned template materials safely.
- Tint materials only when `supportsTint` is true.
- Use primitive fallback if:
  - asset id missing,
  - URI missing,
  - GLB load fails,
  - GLB scene missing.

## Symbol placement

For each object:

```text
symbol position = object position + object height + height offset
```

For multiple symbols:

```text
x offset = (index - centerIndex) * symbolSpacing
```

Billboard logic:

```js
if (symbol.billboardToCamera) {
    symbolGroup.quaternion.copy(state.camera.quaternion);
}
```

## Effects

Implement lightweight effects in render loop or symbol sync:

- `pulse`: scale oscillates.
- `float`: y offset oscillates.
- `spin`: rotate around y.
- `blink`: opacity oscillates.
- `glow`: emissive intensity oscillates.
- `scale-by-intensity`: static scale based on intensity.

Keep deterministic mode stable:
- Use elapsed frame count or deterministic timestamp if `options.deterministicMode`.
- Proof snapshot should round values.

## Proof snapshot

Return:

```json
{
  "sceneId": "generic-tycoon-village",
  "objectCount": 12,
  "symbolCount": 5,
  "loadedAssetCount": 8,
  "missingAssetCount": 0,
  "fallbackObjectCount": 1,
  "selectedObjectIds": [],
  "hoveredObjectId": "",
  "viewportWidth": 1200,
  "viewportHeight": 800
}
```

## Do not overbuild

Do not implement physics, pathfinding, animation blending, particle systems, or economy mapping in this bundle.
