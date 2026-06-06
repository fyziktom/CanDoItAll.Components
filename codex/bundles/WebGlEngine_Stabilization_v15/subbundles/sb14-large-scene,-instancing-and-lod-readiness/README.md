# SB14: Large scene, instancing and LOD readiness

## Objective

Add large-scene budgets and smoke proofs inspired by Three/PlayCanvas optimization patterns.

## Scope

Scene100/500/1000, compact lifecycle key, instancing opportunities, LOD/fallback quality profile

## Constraints

No premature custom WebGL rewrite; use current Three backend and diagnostics.

## Required validation

performance audit + memory/asset metrics

## Required proof artifacts

large-scene-budget.json

## Detailed tasks

1. Read the referenced source files before modifying code.
2. Write a failing-first or gap proof before implementation when feasible.
3. Make the smallest generic change that satisfies the subbundle.
4. Run focused tests/audits before broader validation.
5. Record changed files and proof output under the corresponding proof directory.
6. Do not hide failures with broad allowlists or docs-only assertions.

## Exit criteria

- Requirements are implemented or explicitly rejected with senior-architect reasoning.
- All required proof artifacts exist and are non-empty.
- No new Economy/domain terms appear in generic source/package hard gates.
- Public API, action-kind and JS API approvals are either unchanged or intentionally updated with rationale.
