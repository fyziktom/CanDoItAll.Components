# SB11 — External oracle corpus v3

## Goal

Move/extend golden oracles into external JSON corpus covering shared-well, farmer-land, and multi-goods-elite with expected stores, flows, relationships, metrics, diagnostics, and hash chains.

## Source references

- `E-MULTIGOODS-INVARIANTS`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `oracle-corpus.json`
- `oracle-run-proof.txt`
- `negative-oracle-diff-proof.txt`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
