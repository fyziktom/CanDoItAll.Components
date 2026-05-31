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

## Status
- Completed.

## Prerequisites
- SB01 branch and boundary baseline is complete.

## Exact Source References
- `repo://tools/webgllib/audit-scene-runtime.cjs`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene`
- `repo://src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj`

## Dependency Impact
- Protects SB03-SB05 from accumulating generic runtime debt or domain leakage.

## Validation Depth
- Runtime audit transcript, forbidden-term scan, and file-size scan are required.

## Acceptance Checklist
- Hard threshold and warning threshold findings are recorded.
- Any over-threshold runtime file is split or explicitly followed up.
- Generic runtime JS remains free of forbidden domain words.

## Proof Required
- `bundle://proof/SB02/manifest.md`
- Runtime audit transcript.
- Source assertion for JS line counts and forbidden terms.

## Browser Validation Logging
- Browser validation is not required unless a rendered route changes.

## Progression Gate
- SB03 may proceed when runtime split/audit proof is recorded and no hard-threshold failures remain.

## Suggested Agent Prompt
- Audit Components WebGL runtime JS for size, imports, unsafe patterns, domain neutrality, and large-screen policy drift.
