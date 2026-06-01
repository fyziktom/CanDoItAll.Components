# SB06 Semantic Invariants

| Invariant ID | Assertion |
|---|---|
| SB06-decomposition | WebGlBridge remains decomposed into focused projectors, validators, and contracts under the line-count ceiling. |
| SB06-boundary | Joined Economy/WebGL projection remains in Economy and does not move simulation knowledge into Components. |

## Shallow-pass trap

A shallow pass could only count files while adding bridge behavior to Components. The boundary audit and project-reference tests guard the architectural edge.

## Adversarial negative proof

`economy-boundary-audit.txt` would fail if forbidden simulation boundary references were introduced.

## Semantic positive proof

`economy-bridge-line-counts.txt` shows all bridge files below 300 lines, and `economy-boundary-audit.txt` passes.

## Anti-stub audit

Bridge behavior remains real source decomposition, not a documentation-only claim.

