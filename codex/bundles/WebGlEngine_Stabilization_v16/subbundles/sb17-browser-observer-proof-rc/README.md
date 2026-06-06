# SB17 — Browser observer proof RC

## Goal

Make browser proof independent of expected-only self-comparison.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Browser proof must load/export a real browser run/scene state and compare hash to expected.
- Proof must include runtime idle result, UI exercised result, object positions, command lifecycle, and no runtime errors.
- Add negative proof for browser-loaded document hash mismatch.

## Required proof

- `proof/SB17/manifest.md`
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
