# SB17 — Non-economy generic driver canary

## Goal

Add a small non-economy driver/canary, e.g. neutral workflow/logistics movement, to prove WebGlRunLib and DirectedFlowVisual remain domain-neutral.

## Source references

- `C-DOMAIN-DRIVER`
- `C-WGRUN-ACTIONKINDS`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `non-economy-driver-proof.txt`
- `generic-boundary-report.txt`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
