# SB08 semantic invariants

- The implementation must preserve Components genericity.
- The implementation must not introduce Economy semantics into WebGlLib or WebGlRunLib.
- Runtime proof must demonstrate behavior, not only compilation.
- Required proof artifacts must be non-empty.

## Completion assertions

- WebGlRunLib accepts only the v2 source provenance keys and enforces 96-character keys plus 512-character values.
- WebGlRunLib does not reference Economy packages or read source provenance values for Economy-specific behavior.
- Economy bridge run documents use generic source keys for domain traceability instead of old custom run-layer keys such as `source.bridge`, `source.scenarioId`, `source.runId`, and `source.frameHash`.
- Economy-specific command provenance requirements remain enforced by `EconomyWebGlRunValidator`.
