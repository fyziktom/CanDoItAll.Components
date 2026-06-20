# SB10: Domain boundary audit v4

## Objective

Make domain leakage CI practical for long-lived generic packages.

## Scope

hard source gate, public API gate, package-content gate, soft docs audit, allowlist expiry

## Constraints

Hard gates must not inherit active bundle allowlists.

## Required validation

node domain audit all profiles

## Required proof artifacts

domain-boundary-report.md

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
