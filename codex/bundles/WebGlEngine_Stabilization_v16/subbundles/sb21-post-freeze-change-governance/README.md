# SB21 — Post-freeze change governance

## Goal

Prevent future Economy-driven generic churn.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Add `docs/webgl/post-freeze-change-governance.md`.
- Define change categories and approval requirements.
- Require domain-change triage: driver first, generic only with proof from at least two domains or one domain + generic canary.

## Required proof

- `proof/SB21/manifest.md`
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
