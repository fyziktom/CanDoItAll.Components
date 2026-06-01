# SB07 Semantic Invariants

## Invariant ID

SB07-strict-visual-mapping

## Shallow-pass trap

Strict options can exist while fixture projection still succeeds only because fallback objects, no-op poses, or no-op symbols hide missing semantic mappings.

## Adversarial negative proof

`FixtureProbeFailsStrictlyWhenRequiredSmokeMappingIsRemoved` removes the required `risk` symbol from the shared-well fixture mapping and proves strict projection emits an error-level `missing-symbol-mapping`.

## Semantic positive proof

`FixtureProbeProjectsWithStrictMappingWithoutFallbacks` proves both committed probe fixtures project with strict fallback-disabled options and no fallback/no-op diagnostics.

## Anti-stub audit

`bundle://proof/SB07/transcripts/anti-stub-audit.txt` confirms no placeholder/stub markers in the SB07 fixture mappings or strict mapping tests.

## Raw-note literal closure

- Admin/write pose: `admin-writing` action and pose mappings.
- Risk/warning symbol: `risk` symbol and rule-violation mapping.
- Rule/fee/tax symbol: `rule` and `tax-fee` symbol mappings.
- Resource transfer visual: `resource-transfer-visual` mappings for transfer/trade/tax-fee events.
- Relationship/conflict pulse: `pulse-link` mapping for relationship conflict.
- Strict profile: fallback flags are disabled in committed mappings and strict projection options.
