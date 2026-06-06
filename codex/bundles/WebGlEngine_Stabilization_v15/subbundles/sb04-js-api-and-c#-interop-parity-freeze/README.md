# SB04: JS API and C# interop parity freeze

## Objective

Approve window.CanDoItAll.webglScene API and prove C# wrappers map to valid JS functions with stable result shapes.

## Scope

JS API manifest, C# wrapper parity, missing-runtime behavior, result shape docs

## Constraints

Do not add new JS API methods without approval manifest update.

## Required validation

node audit + dotnet tests

## Required proof artifacts

js-api-manifest and parity report

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
