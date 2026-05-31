# SB05 - Economy WebGL bridge projection hardening

## Goal
Make the bridge output executable, traceable and strict enough for joined simulation + visualization.

## Required actions

1. Validate every `EconomyVisualNode` maps to a `WebGlSceneObject` unless explicitly marked ignored.
2. Validate every visual action subject and target mapping.
3. Fail unresolved mapping unless fallback is explicitly enabled for that action/category.
4. Ensure global actions are not duplicated into every frame.
5. Ensure every run stage contains either scene patches, motions, or an explicit `wait` marker.
6. Ensure every stage metadata has:
   - source simulation frame id,
   - source event id if available,
   - source visual action id,
   - source input pack hash.

## Acceptance criteria

- Bridge validation detects missing mapping instead of silently routing to diagnostic object.
- Shared-resource and constrained-resource probes both produce valid run documents.
