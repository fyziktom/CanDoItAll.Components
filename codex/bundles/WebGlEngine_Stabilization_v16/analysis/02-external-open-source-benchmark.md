# External open-source WebGL benchmark notes

This bundle uses external engines only as design references. Do not replace the current engine with them.

## Three.js lessons

Three.js exposes a broad generic 3D vocabulary: geometry, materials, cameras, loaders, Object3D, InstancedMesh, LOD, WebGLRenderer/WebGPURenderer, controls, helpers, and scene graph primitives.

Lessons for Components:

- Keep `WebGlLib` generic and object/scene/asset oriented.
- Avoid domain concepts in core objects.
- Add release-candidate diagnostics for repeated object patterns and future instancing/LOD readiness.
- Keep controls generic: select, focus, drag, transform, camera, and pointer events.

## PlayCanvas lessons

PlayCanvas positions itself as an open-source WebGL/WebGPU engine and platform for games, AR/VR, configurators, education, and other web 3D experiences.

Lessons for Components:

- Asset lifecycle and project/runtime separation matter.
- Domains should be built through scripts/components/drivers, not baked into the renderer.
- Package-mode consumption and static asset delivery must be part of release proof.

## Babylon.js lessons

Babylon.js has an extensive rendering ecosystem: editor, viewer, inspector, frame graph, WebXR, Gaussian splats, large-world rendering, tooling, and debugging.

Lessons for Components:

- Do not try to clone Babylon.js now.
- Borrow the discipline: diagnostics, inspector/profiler-lite, release notes, viewer/sample proof, asset pipeline proof.
- Keep extensibility stable before adding advanced features.

## regl lessons

regl is a functional WebGL abstraction that treats resources and draw commands explicitly and emphasizes correctness, performance, minimalism, and stability.

Lessons for Components:

- Command batches must have explicit lifecycle states.
- Runtime state should be inspectable.
- Proof should distinguish accepted/scheduled/settled/failed/cancelled.
- API stability matters more than adding accidental features.
