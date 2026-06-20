# SB05 — Domain driver manifest hardening

## Goal

Require driver manifest id/version/hash/action mappings in WebGlRunDocument metadata and browser observer proof. Reject unsupported driver mappings.

## Source references

- `C-DOMAIN-DRIVER`
- `E-BRIDGE-DRIVER`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `driver-manifest-validation-tests.txt`
- `driver-hash-in-run-document.json`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
