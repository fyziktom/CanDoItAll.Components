# Semantic invariants SB08

- The subbundle must not weaken generic/domain boundaries.
- The subbundle must not convert economic errors into visualization/runtime errors.
- The subbundle must not mark proof as complete without non-empty evidence artifacts.
- Runtime/UI/oracle exercised flags are computed from valid evidence records; caller booleans only record requested exercise.
- `research-ready` requires headless validity, oracle evidence, browser observer evidence, valid evidence hashes/schema refs, and a zero warning budget.
