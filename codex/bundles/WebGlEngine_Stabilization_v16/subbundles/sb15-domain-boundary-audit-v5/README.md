# SB15 — Domain boundary audit v5

## Goal

Harden domain leakage gates without overbroad allowlists.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Make source/package hard gates strict and separate from docs/bundle soft audit.
- Ensure terms include Economy and production-line vocabulary.
- Add allowlist expiry/owner/reason validation.
- Add negative probe that `machine`, `work-order`, `station`, `conveyor`, `buyer`, `seller`, `credit`, `elite`, and `market` fail when placed in generic source.

## Required proof

- `proof/SB15/manifest.md`
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
