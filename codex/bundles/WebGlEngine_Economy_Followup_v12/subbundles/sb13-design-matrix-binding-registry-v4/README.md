# SB13 — Design matrix binding registry v4

## Goal

Replace ad hoc factor binding switch with registered binding drivers. Add bindings for multi-goods investment amount, fee rate, claim issue size, and exchange volume.

## Source references

- `E-MULTIGOODS`
- `E-READINESS`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `factor-binding-registry-tests.txt`
- `effective-source-diff-proof.json`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
