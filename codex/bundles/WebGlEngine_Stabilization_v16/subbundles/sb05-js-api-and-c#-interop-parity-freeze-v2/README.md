# SB05 — JS API and C# interop parity freeze v2

## Goal

Stabilize `window.CanDoItAll.webglScene` API contract beyond method names.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Extend JS API approved manifest to include method name, parameter shape, return shape, missing-runtime result, lifecycle behavior, idle/settled behavior, and failure behavior.
- Add C# parity test that every `WebGlSceneView` public runtime method maps to an approved JS method.
- Add negative probe for unapproved JS method.
- Keep method additions blocked unless approval manifest and rationale are updated.

## Required proof

- `proof/SB05/manifest.md`
- changed-file list for this subbundle
- tests/build/audit transcripts relevant to the subbundle
- semantic invariants file
- zero-byte proof transcript scan
- explicit note if no code was changed

## Done criteria

- Public/generic boundaries remain intact.
- No Economy or domain repository files are changed.
- All new source comments are in English.
- The subbundle can be reviewed independently.
