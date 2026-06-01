# SB07 - Economy strict visual mapping completion

## Status

Completed. Closure gate passed.

## Goal

Reduce permissive fallback use for the first real browser smoke.

## Tasks

- Complete fixture visual mappings for:
  - admin/write pose,
  - risk/warning symbol,
  - rule/fee/tax symbol,
  - resource transfer visual,
  - relationship/conflict pulse.
- Add strict mapping profile that disables no-op/fallback where expected.
- Keep fallback modes available for diagnostics, but not for the strict browser smoke.

## Acceptance

- At least one probe runs with no fallback object and no no-op pose/symbol fallbacks.
- Any fallback use is explicit, visible, and treated as a warning/error according to options.

## Prerequisites

- SB06 real scenario export/readiness proof completed.
- Fixture mapping files are located and strict options remain available.

## Owned Requirements

- R07 Strict visual mapping.

## Dependency Impact

Strict mapping is required before SB11 can claim a meaningful smoke proof. Hidden fallback would make the browser smoke misleading.

## Validation Depth

Tests must run at least one probe with strict fallback-disabled options and assert no fallback object, no no-op pose fallback, and no no-op symbol fallback.

## Proof Required

- Strict mapping test transcript.
- Source/artifact assertions for fixture mappings.
- `bundle://proof/SB07/manifest.md`
- `bundle://proof/SB07/semantic-invariants.md`

## Browser Validation Logging

N/A until SB11. Browser smoke must cite strict mapping proof.

## Semantic Adequacy Gate

- Shallow-pass trap: strict option exists but test still passes with fallback assets/no-op mappings.
- Adversarial negative proof: intentionally incomplete mapping fails strict validation.
- Semantic positive proof: selected probe projects with no fallback object/no-op pose/no-op symbol fallbacks.
- Anti-stub audit: no fixture-specific production branch or hidden permissive fallback in strict mode.

## Progression Gate

Pass only when strict projection has no hidden fallback for the selected browser-smoke input.
