# SB16 — Performance/comparability budgets v2

## Goal

Add budgets for multi-goods-elite headless materialization, metrics, oracle validation, artifact hashing, and browser observer proof. Mark not-comparable on budget hard failures.

## Source references

- `E-READINESS`
- `E-MULTIGOODS`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `performance-budget-report.json`
- `not-comparable-negative-test.txt`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
