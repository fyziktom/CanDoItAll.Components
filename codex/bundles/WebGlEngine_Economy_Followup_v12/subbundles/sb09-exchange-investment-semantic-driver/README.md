# SB09 — Exchange/investment semantic driver

## Goal

Add explicit semantics for multi-leg exchange, investment/contribution, claim issuance, return/repayment, and obligation relationship updates without baking them into generic simulation abstractions.

## Source references

- `E-MULTIGOODS`
- `E-BRIDGE-DRIVER`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `exchange-investment-driver-tests.txt`
- `semantic-driver-boundary.md`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
