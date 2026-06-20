# SB19 — Final cross-repo red-team closure

## Goal

Run all required builds/tests/audits, validate bundle proof, inspect zero-length transcripts, and produce final QA report with reopen list.

## Source references

- `All`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `final-red-team-report.md`
- `completed-validator.txt`
- `sha256.txt`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
