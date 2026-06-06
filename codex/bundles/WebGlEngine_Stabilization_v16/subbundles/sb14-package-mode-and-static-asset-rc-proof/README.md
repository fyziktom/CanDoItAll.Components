# SB14 — Package-mode and static asset RC proof

## Goal

Prove actual consumer usage with packages, not only project references.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Package WebGlLib and WebGlRunLib with proof suffix.
- Build WebGlLibOnlyViewer against package.
- Build WebGlRunLibGenericSample against package; add package switch if missing.
- Verify static web assets are included and referenced correctly.

## Required proof

- `proof/SB14/manifest.md`
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
