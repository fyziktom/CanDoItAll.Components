# SB16 — Domain driver contract RC

## Goal

Freeze domain-driver compatibility for Economy and future production-line consumers.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Approval-test driver manifest schema.
- Add sample generic pass-through driver proof.
- Add test-only production-line driver proof that maps domain actions to generic action kinds without leaking domain terms into generic contracts.
- Define semver rules for driver manifest changes.

## Required proof

- `proof/SB16/manifest.md`
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
