
# Open-source WebGL/WebGPU engine comparison

This section is not a proposal to replace our engine. It identifies missing stabilization practices and generic-engine patterns.

## Three.js

Useful lessons:
- Very broad generic object/material/loader/control surface.
- Separate primitives: Object3D, Scene, cameras, loaders, renderers, controls, materials, InstancedMesh/LOD.
- For us: keep WebGlLib as a generic scene/render substrate and avoid domain semantics.

Source: https://threejs.org/docs/

## PlayCanvas

Useful lessons:
- Engine is a standalone open-source runtime with full TypeScript declarations and examples.
- ECS separates entities, components, and systems; systems process component batches.
- Asset lifecycle distinguishes asset registry metadata from loaded runtime resources.
- Optimization docs emphasize load time, stable frame rate, CPU/GPU load, and memory usage.

Sources:
- https://developer.playcanvas.com/user-manual/engine/
- https://developer.playcanvas.com/user-manual/ecs/
- https://developer.playcanvas.com/user-manual/assets/
- https://developer.playcanvas.com/user-manual/optimization/

## regl

Useful lessons:
- Treats WebGL as resources and complete commands, minimizing shared mutable state.
- Emphasizes correctness, performance, stability and semantic versioning.
- For us: command batch lifecycle and result states should be precise and approval-tested.

Source: https://github.com/regl-project/regl

## Babylon.js

Useful lessons:
- Rich engine ecosystem with editor/inspector/viewer, frame graph, large-world rendering, 3D Tiles and advanced material/rendering features.
- For us: do not attempt feature parity now; instead stabilize extensibility points, diagnostics, assets, and package boundaries.

Source: https://www.babylonjs.com/

## Gap summary for CanDoItAll.Components

- Need an explicit asset registry/resource lifecycle contract comparable to PlayCanvas asset/resource separation.
- Need RC-grade public API/JS API/package-content approval comparable to mature engine semver discipline.
- Need production-line canary to test ECS-like generic objects/links/status/flows outside Economy.
- Need profiler-lite diagnostics and large-scene budget proof.
- Need a single release-candidate validation command.
