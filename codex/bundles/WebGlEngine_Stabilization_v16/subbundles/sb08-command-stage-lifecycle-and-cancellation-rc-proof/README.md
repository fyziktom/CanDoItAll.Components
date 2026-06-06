# SB08 — Command/stage lifecycle and cancellation RC proof

## Goal

Prove accepted/scheduled/settled/failed/cancelled lifecycle states cannot be confused.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Add tests for `ApplyCommandBatchAsync` returning scheduled state when stage/motion work remains.
- Add tests for `ApplyCommandBatchAndWaitAsync` hard-failing on idle timeout when required.
- Add cancellation test for stage runner and motion queues.
- Add proof snapshot after cancellation showing no queued/active stages and no active/queued motions.

## Required proof

- `proof/SB08/manifest.md`
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
