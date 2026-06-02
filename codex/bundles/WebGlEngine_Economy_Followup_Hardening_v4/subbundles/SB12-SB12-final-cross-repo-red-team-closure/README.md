# SB12 Final cross-repo red-team closure

## Objective

Implement the next hardening step for the WebGL/Economy engine foundation.

## Scope

- Run final cross-repo builds, focused tests, package-mode proofs, browser proofs, and boundary audits.
- Run proof-integrity validator over the completed bundle.
- Perform senior QA red-team review against all requirements.
- Update execution report with real command outputs and non-empty proof links.
- Do not mark completed if any required transcript is empty.

## Out of scope

- Do not add domain semantics into Components packages.
- Do not rewrite unrelated systems.
- Do not close the subbundle with screenshots only.
- Do not accept empty required proof artifacts.

## Implementation guidance

- Start with a failing-first test or audit where applicable.
- Make the smallest cohesive refactor that fixes the root cause.
- Add source assertions that prove the intended path is used.
- Keep API compatibility where safe; otherwise document the migration.
- Ensure all source-code comments are in English.

## Required proof

- Final execution report.
- Completed-stage validation.
- Proof-integrity pass.
- Cross-repo build/test/package/browser proof matrix.
- Senior QA sign-off with remaining known limitations.

## Refactor gate

Before moving to the next subbundle, record:
- changed files;
- test/build/audit commands;
- proof artifact paths;
- open risks;
- whether public API changed and how users migrate.
