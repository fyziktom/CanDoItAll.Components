# SB02 — Components JS runtime split audit

## Goal
Keep WebGL JS clean, small, and maintainable without TypeScript.

## Checks
- Runtime JS file hard threshold: 320 lines.
- Warning threshold: 220 lines.
- Public façade threshold: 180 lines.
- No domain words in generic JS: economy, ledger, account, water, well, farmer, land, parcel.
- No unsafe DOM patterns.
- No circular imports.

## Required hardening
- If any runtime JS file exceeds warning threshold, either split it or document why it is temporarily acceptable.
- Keep `audit-scene-runtime.cjs` as a tool, but create a follow-up to split it if it grows further.
- Preserve large-screen-only policy.

## Validation
- Run WebGL runtime audit.
- Include audit output in proof.
