# SB12 — Large scene and repeated-object readiness

## Goal

Stress the engine shape expected by production-line layouts.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Build proof scenes for 100, 500, and 1000+ repeated generic nodes/tokens/links.
- Measure build time, patch time, command batch time, idle wait, and snapshot size.
- Do not require full instancing implementation unless gap is severe; record explicit instancing/LOD backlog if not implemented.
- Ensure compact lifecycle key works with revision increments and does not miss updates.

## Required proof

- `proof/SB12/manifest.md`
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
