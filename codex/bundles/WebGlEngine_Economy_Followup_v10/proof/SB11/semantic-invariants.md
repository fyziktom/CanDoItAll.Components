# Semantic invariants SB11

- The subbundle must not weaken generic/domain boundaries.
- The subbundle must not convert economic errors into visualization/runtime errors.
- The subbundle must not mark proof as complete without non-empty evidence artifacts.
- Oracle expectations must be externalized in JSON and evaluated by code, not hardcoded only as asserts.
- Oracle diffs must identify the failed path such as `stores.*`, `flows.byReason.*`, `issues.byCategory.*`, `metrics.*`, or `frameHashChain.expected`.
