# SB04 — Domain boundary audit CI v2

## Goal

Harden Components domain audit: separate package/source hard gates from docs/history scans, enforce term registry expansion and allowlist expiry, include JS/Razor/JSON/workflow files.

## Source references

- `C-DOMAIN-AUDIT`

## Mandatory implementation constraints

- Preserve all existing functionality.
- Do not weaken generic/domain boundaries.
- Do not replace semantic validation with snapshot-only tests.
- Every proof transcript must be non-empty and must reference the exact command/test/audit run.
- If a proof cannot be produced, reopen this subbundle instead of marking it complete.

## Required proof artifacts

- `domain-audit-source-gate.txt`
- `domain-audit-docs-report.txt`
- `allowlist-review.md`

## Done criteria

- Focused tests for this subbundle pass.
- Relevant repo build passes.
- Domain boundary audit passes where applicable.
- Proof artifacts exist and are non-empty.
- Any remaining limitation is recorded in `reviews/reopen-items.md`.
