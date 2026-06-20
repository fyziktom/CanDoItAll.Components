# External WebGL engine benchmark and lessons

## Three.js

Three.js exposes a broad generic rendering surface: cameras, geometries, materials, loaders, controls, objects such as `InstancedMesh` and `LOD`, WebGL/WebGPU renderers, and many primitives. Its `InstancedMesh` docs frame instancing as draw-call reduction for many objects with the same geometry/material and different transforms, with explicit update/disposal responsibilities. The relevant lesson is that generic rendering engines stay generic by exposing reusable building blocks, not domain concepts.

Implication for Components:
- Keep WebGlLib as renderer/scene substrate.
- Keep WebGlRunLib as generic run/playback/action layer.
- Add repeated-object/instancing readiness as a generic concept.
- Do not add Economy or production-line semantics to core engine.

Sources:
- https://threejs.org/docs/
- https://threejs.org/docs/api/en/objects/InstancedMesh.html
- https://threejs.org/docs/#api/en/objects/LOD

## PlayCanvas

PlayCanvas describes its Engine as a JavaScript runtime framework, MIT-licensed, published to npm and shipped with TypeScript declarations and examples. Its batching guidance treats draw-call reduction as an optimization with material and static/dynamic constraints. It also emphasizes optimization goals: load time, stable frame rate, CPU/GPU load and memory usage.

Implication for Components:
- Package-mode proof matters.
- Samples must be credible.
- Runtime diagnostics must include load/runtime/performance dimensions.
- RC validation should have package and browser proof.

Sources:
- https://developer.playcanvas.com/user-manual/engine/
- https://developer.playcanvas.com/user-manual/optimization/
- https://developer.playcanvas.com/user-manual/graphics/advanced-rendering/batching/

## Babylon.js

Babylon.js shows how mature web engines grow tooling around the core runtime: Playground, Sandbox, Node Material Editor, Node Render Graph Editor, Viewer, Inspector, Frame Graph, large-world rendering, 3D Tiles and more. Its LOD documentation reinforces that detail switching is renderer behavior based on distance or coverage, not a domain-specific simulator contract.

Implication for Components:
- Do not copy the full scope now.
- Stabilize diagnostics and observer proof first.
- Keep tooling as optional helpers.
- Use docs and manifests to prevent feature creep.

Sources:
- https://www.babylonjs.com/
- https://doc.babylonjs.com/features/featuresDeepDive/mesh/LOD

## regl

regl is useful as a conceptual benchmark because it replaces raw WebGL calls with resources and complete commands. A command is a complete representation of the WebGL state needed for a draw call.

Implication for Components:
- `WebGlSceneCommandBatch` should remain an explicit command transport.
- Lifecycle states must distinguish accepted/scheduled/settled/failed/cancelled.
- Less hidden mutable global state means easier proof and fewer simulator bugs.

Source: https://github.com/regl-project/regl
