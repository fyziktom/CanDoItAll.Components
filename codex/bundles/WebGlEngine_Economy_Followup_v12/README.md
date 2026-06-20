# CanDoItAll_WebGlEngine_Economy_Followup_Hardening_Bundle_v12

Prepared: 2026-06-05

## Goal

This follow-up bundle hardens the current WebGL/RunLib + Economy simulation stack after the v11 implementation.

The current codebase has made substantial progress: generic action kinds are cleaner, domain-driver concepts exist, the third `multi-goods-elite` canary exists, and readiness now has an evidence model. However, the stack is not yet fully research-grade because raw provenance can still leak domain identifiers into generic run documents, readiness evidence validates record shape more than real file artifacts, the multi-goods canary still needs stronger exchange/investment semantics, and the SimpleAccounts mutation layer remains a high-risk concentration point.

## Primary repos and branches

- `fyziktom/CanDoItAll.Components`, branch/ref: `webgl-engine`
- `fyziktom/CanDoItAll.Economy`, branch/ref: `main`

## Execution rule

Execute subbundles in order. Do not skip refactor gates. After each subbundle, write proof artifacts under `proof/SBxx` and run the relevant focused tests before moving on.

## Current verdict

Economic simulations are suitable for exploratory analysis and pipeline development. Research-grade conclusions require the new v12 gates:

1. Artifact-backed readiness evidence with real hash/file validation.
2. Opaque generic provenance with domain-side trace maps.
3. Domain-driver manifest/hash embedded in run documents and observer proof.
4. External oracle corpus for all three scenarios.
5. Multi-goods exchange/investment semantic driver and metamorphic tests.
6. Mandatory domain boundary audits across Components generic source and Economy simulation layers.
7. Final red-team closure with no zero-length proof transcripts.

## Subbundle list

- SB01: Current-state and v11 closure audit
- SB02: Evidence resolver and artifact-backed readiness
- SB03: Source provenance opacity policy
- SB04: Domain boundary audit CI v2
- SB05: Domain driver manifest hardening
- SB06: Economy bridge driver refactor
- SB07: DirectedFlowVisual semantics boundary
- SB08: Simple simulation mutation split
- SB09: Exchange/investment semantic driver
- SB10: Multi-goods-elite research canary closure
- SB11: External oracle corpus v3
- SB12: Metamorphic economics tests
- SB13: Design matrix binding registry v4
- SB14: Browser observer real-state proof v2
- SB15: Noise firewall and diagnostic classification
- SB16: Performance/comparability budgets v2
- SB17: Non-economy generic driver canary
- SB18: Docs and operator workflow
- SB19: Final cross-repo red-team closure
