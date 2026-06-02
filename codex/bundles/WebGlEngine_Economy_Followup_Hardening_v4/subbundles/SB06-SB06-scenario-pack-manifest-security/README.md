# SB06 Scenario pack manifest and security

## Objective

Implement the next hardening step for the WebGL/Economy engine foundation.

## Scope

- Define scenario pack manifest format for runtime samples.
- Validate required files: experiment, scenario definition, parameters, placement, run plan, visual mapping, expected invariants.
- Add max file size/count limits and safe relative path policy.
- Make file-system catalog validate manifest and compute deterministic pack hash.
- Expose safe diagnostics for missing/invalid scenario packs.

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

- Traversal attack tests.
- Missing-file and invalid-manifest tests.
- Pack hash determinism tests.
- Node runtime content packaging proof.

## Refactor gate

Before moving to the next subbundle, record:
- changed files;
- test/build/audit commands;
- proof artifact paths;
- open risks;
- whether public API changed and how users migrate.
