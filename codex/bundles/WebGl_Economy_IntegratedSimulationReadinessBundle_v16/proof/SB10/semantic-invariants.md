# SB10 Semantic Invariants

| Invariant ID | Assertion |
|---|---|
| SB10-renderer-neutral-mapping | Economy visual mapping remains renderer-neutral until WebGlBridge maps it to WebGlRun contracts. |
| SB10-strict-visual-state | Missing pose and symbol definitions are rejected by default. |

## Shallow-pass trap

A shallow pass could place WebGL asset assumptions into generic visual mapping contracts or always allow fallback.

## Adversarial negative proof

`economy-visual-mapping-boundary-tests.txt` covers strict missing-pose and missing-symbol rejection.

## Semantic positive proof

`economy-visual-mapping-boundary-tests.txt` passes and project reference tests keep WebGlBridge off SimpleAccounts/Ledger dependencies.

## Anti-stub audit

Mapping behavior is tested through real bridge projection, not isolated string matching.

