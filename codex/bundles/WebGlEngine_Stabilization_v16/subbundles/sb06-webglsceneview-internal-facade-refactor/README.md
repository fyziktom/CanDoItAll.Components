# SB06 — WebGlSceneView internal facade refactor

## Goal

Reduce internal risk in `WebGlSceneView.razor` while preserving public API.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Split implementation into partial/helper classes: lifecycle, callbacks, import/export, commands, runtime idle/stop, serialization/keying.
- Keep the Razor markup and public methods unchanged unless approval baseline is deliberately updated.
- Add tests/scan verifying public API snapshot unchanged.
- Keep comments in source code in English.

## Required proof

- `proof/SB06/manifest.md`
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
