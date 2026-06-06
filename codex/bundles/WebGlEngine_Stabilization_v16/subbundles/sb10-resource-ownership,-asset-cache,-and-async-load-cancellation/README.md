# SB10 — Resource ownership, asset cache, and async load cancellation

## Goal

Stabilize asset lifecycle before using large scenes or production-line layouts.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Add stress proof for repeated import/dispose with GLB templates and primitive fallback.
- Verify cloned materials do not dispose shared textures.
- Verify late asset loads after dispose are ignored and diagnosed.
- Track pending disposal promises and require idle to include asset-cache pending disposal when relevant.

## Required proof

- `proof/SB10/manifest.md`
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
