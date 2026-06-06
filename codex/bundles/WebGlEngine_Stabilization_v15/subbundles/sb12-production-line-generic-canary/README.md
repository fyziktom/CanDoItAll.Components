# SB12: Production-line generic canary

## Objective

Add a non-Economy canary proving engine can visualize manufacturing-line style simulations generically.

## Scope

station/buffer/conveyor/WIP object model as generic scene/run documents, status symbols, directed flow motions, queue overlays

## Constraints

Use neutral canary package/tests; no Economy repo; no domain terms in generic production source.

## Required validation

generic sample/test only

## Required proof artifacts

production-line-canary-proof.md

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
