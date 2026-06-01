# SB09 Semantic Invariants

| Invariant ID | Shallow-pass trap | Adversarial negative proof | Semantic positive proof | Anti-stub audit |
| --- | --- | --- | --- | --- |
| SB09-INV-01 | A validator that only checks nonempty frames could miss skipped broken actions. | Negative tests project unresolved subject/target and missing pose/symbol actions, then assert validation errors. | Positive strict mapping test produces a real stage and valid bridge validation. | Validation inspects document/frame diagnostic metadata emitted by production projection. |
| SB09-INV-02 | Fallback options could silently convert errors into invisible success. | Explicit fallback test asserts a `fallback-object-used` warning. | Options `AllowFallbackObject`, `AllowNoOpPoseFallback`, `AllowNoOpSymbolFallback`, and `TreatUnresolvedMappingAsError` control severity. | Tests verify stage source ids, event ids, input hash, barrier policy, and fallback object ids. |
