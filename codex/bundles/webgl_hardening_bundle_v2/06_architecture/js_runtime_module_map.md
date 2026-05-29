# Recommended JS Runtime Module Map

Target directory:

```text
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene
```

## Public API

```text
01-webgl-scene.js
```

Thin facade only. No rendering logic, no DOM creation, no asset loading internals.

## Core helpers

```text
02-webgl-scene-core.js
```

Keep small. If it grows, split into:

```text
02a-webgl-scene-vectors.js
02b-webgl-scene-materials.js
02c-webgl-scene-normalization.js
02d-webgl-scene-diagnostics.js
```

## Existing responsibility modules

```text
03-webgl-scene-assets.js
04-webgl-scene-symbols.js
05-webgl-scene-interaction.js
06-webgl-scene-camera.js
07-webgl-scene-overlays.js
08-webgl-scene-proof.js
09-webgl-scene-primitives.js
10-webgl-scene-lifecycle.js
11-webgl-scene-graph.js
12-webgl-scene-drag.js
13-webgl-scene-patching.js
14-webgl-scene-motion.js
15-webgl-scene-render-loop.js
16-webgl-scene-models.js
```

## Proposed new extraction modules

```text
17-webgl-scene-resources.js
18-webgl-scene-model-diagnostics.js
19-webgl-scene-shell.js
20-webgl-scene-state.js
21-webgl-scene-notifications.js
22-webgl-scene-disposal.js
23-webgl-scene-command-results.js
24-webgl-scene-render-scheduler.js
```

Do not create all files blindly. Extract where it reduces complexity and improves safety. Avoid micro-modules with only one trivial function unless the function is a stable boundary.

