# SB09 — Interaction abstraction for non-simulator controls

## Goal

Prepare generic small controls without embedding simulator logic.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Audit current selection/hover/drag/move callbacks.
- Define generic event contract for object select, hover, drag start/update/end, command completed/failed, and optional object action request.
- Ensure WebGlLib does not interpret domain actions.
- Add production-line-canary interaction proof with generic node/token selection only.

## Required proof

- `proof/SB09/manifest.md`
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
