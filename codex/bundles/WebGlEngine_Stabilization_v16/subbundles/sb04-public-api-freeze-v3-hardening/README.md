# SB04 — Public API freeze v3 hardening

## Goal

Strengthen public API approval tests so they can become an RC gate.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Normalize public API snapshots to reduce line-number noise where possible.
- Separate WebGlLib and WebGlRunLib API snapshots.
- Add explicit approved-break workflow: any API drift must include `API_CHANGE_RATIONALE.md` in proof.
- Approval tests must fail on unapproved added/removed/renamed public members.

## Required proof

- `proof/SB04/manifest.md`
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
