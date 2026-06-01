# SB02 - Components generic browser apply adapter

## Goal

Provide a generic bridge from `WebGlRunFrameApplyResult` to actual `WebGlSceneView`/JS runtime calls.

## Tasks

- Add a generic adapter in `CanDoItAll.Components.WebGlRunLib` or `WebGlLib`, without Economy references.
- It must accept a `WebGlRunFrameApplyResult`.
- It must apply:
  - scene reset if required,
  - patches,
  - motions,
  - command batches/stages,
  - stage barriers.
- It must return a typed result with:
  - applied frame index,
  - stage count,
  - motion count,
  - patch count,
  - runtime diagnostic snapshot.

## Acceptance

- Unit tests prove the adapter converts a frame apply result into expected runtime calls using a fake runtime interface.
- No Economy references in Components.
- No browser UI work in Components beyond generic primitives.
