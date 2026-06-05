# SB12 — Metamorphic economics tests

## Goal

Add monotonic and conservation tests: increasing investment changes dependency/concentration; increasing fee changes liquidity; reciprocal exchange conserves goods/credit as declared.

## Source references

- `E-MULTIGOODS`
- `E-MULTIGOODS-INVARIANTS`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `metamorphic-test-results.txt`
- `conservation-test-results.txt`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
