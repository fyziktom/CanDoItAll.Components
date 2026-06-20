# Phase plan

## Phase 1 — API and boundary stabilization

- SB01
- SB02
- SB03
- SB04
- SB05
- CP-A

## Phase 2 — Runtime internals and lifecycle

- SB06
- SB07
- SB08
- SB09
- SB10
- CP-B

## Phase 3 — Canary and performance

- SB11
- SB12
- SB13
- SB14
- CP-C

## Phase 4 — Release-candidate validation

- SB15
- SB16
- SB17
- SB18
- CP-D
- SB19
- SB20
- SB21
- SB22

## Execution stop rules

Stop and request review if:

- A generic API change is needed for only one domain.
- Domain terms appear in generic source/package hard gates.
- `WebGlSceneView` public API changes without approval baseline and rationale.
- Runtime idle semantics cannot be proven by deterministic tests.
- Package-mode samples fail.
- Browser proof uses expected-only self-comparison.
