# SB03 — Components-only guard and no-Economy enforcement

## Goal

Make it impossible for this bundle to modify Economy or domain code accidentally.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Add or update bundle validation to scan changed-file lists and fail if any path outside Components appears.
- Update README/proof manifests with explicit scope lock.
- Add a source assertion that Components public packages have no Economy references.

## Required proof

- `proof/SB03/manifest.md`
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
