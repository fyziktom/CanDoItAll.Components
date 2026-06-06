# CHECKPOINT-B: Review/refactor checkpoint B

## Mandatory checkpoint

This checkpoint is a hard stop. Codex must summarize completed work, failing/passing proofs, API changes, package changes, and unresolved risks before continuing. Do not proceed until the checkpoint report is complete.

## Objective

Senior review after runtime/asset changes.

## Scope

confirm WebGlSceneView refactor did not alter public API, JS surface or package content unexpectedly

## Constraints

Stop if idle/command/resource proof is flaky.

## Required validation

all freeze tests + browser proof

## Required proof artifacts

checkpoint-b-report.md

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
