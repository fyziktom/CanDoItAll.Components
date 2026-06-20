# SB18 — Docs and operator workflow

## Goal

Document when a simulation is exploratory/headless-valid/oracle-valid/research-ready, and how to run the canonical CLI/test sequence.

## Source references

- `E-READINESS`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `operator-workflow.md`
- `research-readiness-checklist.md`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
