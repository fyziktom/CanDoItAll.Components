# SB17: Docs and external consumer guide

## Objective

Prepare documentation so future Economy and manufacturing simulators can consume the generic engine without touching Components internals.

## Scope

quickstarts, API freeze policy, package install, domain driver authoring, production-line canary walkthrough

## Constraints

No Economy-specific docs except as external consumer examples in boundary doc.

## Required validation

docs link check

## Required proof artifacts

consumer-docs-report.md

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
