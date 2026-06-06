# External WebGL engine benchmark and lessons

## Three.js

Three.js exposes a broad generic rendering surface: cameras, geometries, materials, loaders, controls, objects such as `InstancedMesh` and `LOD`, WebGL/WebGPU renderers, and many primitives. The relevant lesson is that generic rendering engines stay generic by exposing reusable building blocks, not domain concepts.

Implication for Components:
- Keep WebGlLib as renderer/scene substrate.
- Keep WebGlRunLib as generic run/playback/action layer.
- Add repeated-object/instancing readiness as a generic concept.
- Do not add Economy or production-line semantics to core engine.

Source: https://threejs.org/docs/

## PlayCanvas

PlayCanvas describes its Engine as a JavaScript runtime framework, MIT-licensed, published to npm and shipped with TypeScript declarations and examples. It also emphasizes optimization goals: load time, stable frame rate, CPU/GPU load and memory usage.

Implication for Components:
- Package-mode proof matters.
- Samples must be credible.
- Runtime diagnostics must include load/runtime/performance dimensions.
- RC validation should have package and browser proof.

Sources:
- https://developer.playcanvas.com/user-manual/engine/
- https://developer.playcanvas.com/user-manual/optimization/

## Babylon.js

Babylon.js shows how mature web engines grow tooling around the core runtime: Playground, Sandbox, Node Material Editor, Node Render Graph Editor, Viewer, Inspector, Frame Graph, large-world rendering, 3D Tiles and more.

Implication for Components:
- Do not copy the full scope now.
- Stabilize diagnostics and observer proof first.
- Keep tooling as optional helpers.
- Use docs and manifests to prevent feature creep.

Source: https://www.babylonjs.com/

## regl

regl is useful as a conceptual benchmark because it replaces raw WebGL calls with resources and complete commands. A command is a complete representation of the WebGL state needed for a draw call.

Implication for Components:
- `WebGlSceneCommandBatch` should remain an explicit command transport.
- Lifecycle states must distinguish accepted/scheduled/settled/failed/cancelled.
- Less hidden mutable global state means easier proof and fewer simulator bugs.

Source: https://github.com/regl-project/regl
