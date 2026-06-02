# SB07 Async session persistence and portable export/import

## Objective

Implement the next hardening step for the WebGL/Economy engine foundation.

## Scope

- Add async-first session methods for load/project/export/import/snapshot operations.
- Remove `.GetAwaiter().GetResult()` from library persistence logic.
- Export session with scenario id, source kind, scenario pack hash, input pack hash, snapshot ids/hashes, and optional portable pack metadata.
- Keep legacy path fields only for backward compatibility and mark them as legacy.
- Import must verify source/hash before restoring current step and snapshot.

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

- Failing-first sync-over-async detection test.
- Passing async export/import tests.
- Portable export/import test from a different base directory.
- Backward compatibility test for old exports.

## Refactor gate

Before moving to the next subbundle, record:
- changed files;
- test/build/audit commands;
- proof artifact paths;
- open risks;
- whether public API changed and how users migrate.
