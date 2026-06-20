# SB02 — Evidence resolver and artifact-backed readiness

## Goal

Make EconomyExperimentEvidenceValidator verify real artifact existence, byte size, SHA-256, and schema version through an artifact root/resolver.

## Source references

- `E-READINESS`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `evidence-resolver-tests.txt`
- `readiness-evidence-negative-tests.txt`
- `artifact-hash-proof.json`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
