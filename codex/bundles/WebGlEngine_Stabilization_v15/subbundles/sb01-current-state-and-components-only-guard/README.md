# SB01: Current-state and Components-only guard

## Objective

Audit what Codex changed, verify only Components changed for this wave, capture branch/commit hashes, and fail if Economy files are touched by v15 execution.

## Scope

repo audit, changed-file manifest, proof hygiene inventory, explicit no-economy guard

## Constraints

No implementation changes except bundle/proof scripts.

## Required validation

none

## Required proof artifacts

changed-file-hashes, source refs, proof inventory

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
