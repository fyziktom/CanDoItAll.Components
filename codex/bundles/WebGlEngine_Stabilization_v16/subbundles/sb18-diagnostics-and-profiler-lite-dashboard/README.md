# SB18 — Diagnostics and profiler-lite dashboard

## Goal

Expose enough diagnostics for future Economy and production-line operators without domain semantics.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Add generic profiler snapshot contract: frame time, render count, object count, link count, active/queued motions, stage queues, asset cache, patch classifications.
- Add optional sample dashboard in WebGlSandbox.
- Keep metrics generic and do not add domain statistics.

## Required proof

- `proof/SB18/manifest.md`
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
