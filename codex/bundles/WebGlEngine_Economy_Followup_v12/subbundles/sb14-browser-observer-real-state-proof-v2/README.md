# SB14 — Browser observer real-state proof v2

## Goal

Require browser proof to export actual loaded run-document hash, scene content hash, runtime idle state, completed stages, final object positions, and driver hash.

## Source references

- `C-DOMAIN-DRIVER`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `browser-observer-proof.json`
- `playwright-transcript.txt`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
