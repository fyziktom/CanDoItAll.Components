# SB03 — Resource Ownership and Disposal Hardening

## Goal

Prevent WebGL resource leaks when scenes are updated, models are replaced, symbols are rebuilt, or the component is disposed.

## Current risk

GLB template objects and cloned instances need clear ownership. Shared template geometry should not be disposed per instance, but cloned materials should be disposed. Primitive geometries/materials must be disposed. Decorations must also be disposed.

## Implementation tasks

Add helper module:

```text
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js
```

It should expose helpers such as:

```js
markSharedTemplateResource(object)
markInstanceResource(object, { ownsGeometry, ownsMaterial })
disposeSceneObjectTree(object)
disposeOwnedMaterial(material)
disposeOwnedGeometry(geometry)
```

Refactor current disposal to use explicit ownership flags instead of broad `skipDispose` where possible.

Specific requirements:

- primitive fallback geometry/materials are owned and disposed;
- cloned materials from model instances are owned and disposed;
- shared template geometry/materials are not disposed per instance;
- decorations (`ground`, `grid`) are disposed during runtime dispose;
- link geometries/materials are disposed;
- symbol groups are disposed;
- create/dispose repeated cycles do not leave duplicate canvases or event handlers.

## Acceptance criteria

- Add a browser proof that creates and disposes the scene multiple times.
- After repeated create/dispose, there is at most one canvas in the active scene host.
- Diagnostics include a create/dispose counter or proof evidence.
- No existing workbench runtime is changed except shared asset includes if necessary.

