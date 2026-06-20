# SB19 — Docs and consumer migration guide

## Goal

Prepare the freeze boundary for downstream consumers.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Document WebGlLib-only usage.
- Document WebGlRunLib usage.
- Document domain-driver usage for Economy and future production-line simulator.
- Document post-freeze change governance.
- Document runtime idle policies and troubleshooting.

## Required proof

- `proof/SB19/manifest.md`
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
