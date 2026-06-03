# SB06 semantic invariants

- The implementation must preserve Components genericity.
- The implementation must not introduce Economy semantics into WebGlLib or WebGlRunLib.
- Runtime proof must demonstrate behavior, not only compilation.
- Required proof artifacts must be non-empty.

## Completion assertions

- Scenario-pack security validation is implemented only in Economy SimulationSandbox catalog code and Economy Node runtime scenario manifest files.
- Runtime manifests now declare `requiredFiles`, `maxFileCount`, and `maxFileBytes`.
- The catalog validates manifest-declared paths with the same safe-relative-path policy used for companion file access.
- The catalog validates experiment input packs in strict mode with document reference, document content hash, and pack hash verification enabled.
- Deterministic pack hashing is proven across different copy orders and between source/runtime output packs.
