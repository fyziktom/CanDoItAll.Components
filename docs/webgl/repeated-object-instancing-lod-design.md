# Repeated Object Instancing and LOD Design Checkpoint

This checkpoint records the generic design before Components adds any heavy repeated-object renderer.

## Decision

Do not implement a new instancing backend in this release candidate.

The current engine should continue to use primitive/model fallback rendering, command batching, compact lifecycle keys, and runtime budgets. A future instancing backend should be added only after diagnostics show that repeated identical objects are draw-call or memory bottlenecks in at least two generic canaries.

## External Lessons

- Three.js `InstancedMesh` reduces draw calls for many objects that share geometry and material while varying transforms. It also requires explicit instance-matrix/color updates, bounds recomputation after transforms, and disposal of GPU resources.
- Three.js `LOD` and Babylon.js LOD both point to distance or screen-coverage based detail changes as renderer internals, not domain concepts.
- PlayCanvas batching emphasizes shared-material rules and distinguishes static and dynamic batching. That maps to a backend optimization decision, not a public simulator vocabulary.
- regl is a lower-level reminder to keep command state explicit and validated rather than hidden in mutable global state.

References:
- https://threejs.org/docs/api/en/objects/InstancedMesh.html
- https://threejs.org/docs/#api/en/objects/LOD
- https://developer.playcanvas.com/user-manual/graphics/advanced-rendering/batching/
- https://doc.babylonjs.com/features/featuresDeepDive/mesh/LOD
- https://github.com/regl-project/regl

## Backend-Neutral Adapter Shape

A future adapter should stay internal until it proves value:

```text
Scene objects -> repeated-object classifier -> render group plan -> backend renderer
```

The classifier may group scene objects only by generic render facts:

- asset id or primitive kind
- material/tint compatibility
- symbol and label requirements
- selectable, draggable, or individually hoverable state
- transform update frequency
- visibility and layer membership

It must not group by station, machine, work order, account, market, or any other domain meaning.

## Fallback Rules

Use the existing renderer when any of these are true:

- objects need different geometry, material, or per-object asset variants
- objects require independently pickable meshes and the backend cannot preserve object-id hit testing
- object count is below the measured break-even threshold
- the repeated group contains model diagnostics errors
- the backend cannot preserve command-result proof, object-position export, or runtime idle diagnostics

## Diagnostics Required Before Implementation

Before adding an instancing backend, collect:

- object count, visible object count, repeated-object group count
- mesh, material, and transparent-material counts
- estimated triangle, vertex, and loaded-asset byte counts
- frame time average and peak
- command batch size and interop calls avoided
- transform update count per repeated group
- hit-test fidelity for selection and hover
- browser observer proof showing exported object positions still match expected positions

## Public API Position

No public API widening is approved by this checkpoint. If later proof justifies the backend, prefer internal options and diagnostics first. Public controls should appear only after a separate API change request shows why runtime budgets and asset hints are insufficient.
