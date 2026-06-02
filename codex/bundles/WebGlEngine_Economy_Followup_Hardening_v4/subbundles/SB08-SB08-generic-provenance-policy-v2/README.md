# SB08 Generic provenance policy v2

## Objective

Implement the next hardening step for the WebGL/Economy engine foundation.

## Scope

- Implement typed/allowlisted provenance policy in WebGlRunLib.
- Keep domain values opaque but enforce key/value size, allowed source keys, and required bridge command provenance.
- Update Economy bridge to produce the agreed provenance shape.
- Move Economy-specific provenance interpretation into Economy validators only.
- Update docs and tests.

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

- Generic validator rejects malformed source keys.
- Generic validator accepts valid opaque source provenance.
- Economy bridge validates required source fields.
- Boundary audit proves Components does not interpret Economy semantics.

## Refactor gate

Before moving to the next subbundle, record:
- changed files;
- test/build/audit commands;
- proof artifact paths;
- open risks;
- whether public API changed and how users migrate.
