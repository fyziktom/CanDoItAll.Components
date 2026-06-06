# SB02: External WebGL engine benchmark matrix

## Objective

Turn Three.js/Babylon/PlayCanvas/regl findings into a requirements matrix for our generic engine without copying their APIs.

## Scope

scene graph, asset lifecycle, ECS/canary, command model, profiling/optimization, package stability

## Constraints

No feature implementation yet; only architecture comparison and gap classification.

## Required validation

architect review required

## Required proof artifacts

external-gap-matrix.md

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
