# CanDoItAll WebGL/Economy Follow-up Hardening Bundle v9

Prepared: 2026-06-04 15:14:17Z

## Purpose

This bundle follows the completed v8 work and focuses on turning the current WebGL/Economy simulation stack from an engineering-demo/exploratory system into a safer research-grade experiment stack.

It specifically checks:

- whether the previous fixes were actually completed,
- whether pause/playback/runtime idle is reliable,
- whether browser observer proof is real and not self-referential,
- whether readiness claims are artifact-backed,
- whether generic Components code leaks Economy-specific concepts,
- whether a third structurally different scenario can expose genericity gaps.

## Key conclusion

The simulator can be used now for exploratory work and strict headless runs, but research-grade claims should require the new v9 gates: artifact-backed readiness, real browser observer proof, external golden oracle corpus, strict no-fallback semantics and at least one third scenario that is not structurally similar to shared-well/farmer-land.

## Subbundles

- `SB01` — Current-state and v8 closure audit
- `SB02` — Immediate Pause/Stop ordering fix
- `SB03` — Settled runtime idle semantics
- `SB04` — ApplyCommandBatch lifecycle contract
- `SB05` — Browser observer proof must use real browser state
- `SB06` — Generic Components domain leakage audit
- `SB07` — Economy-to-Components mapping boundary
- `SB08` — Readiness evidence contract v3
- `SB09` — External golden oracle corpus
- `SB10` — Third scenario: multi-goods exchange/investment/elite formation
- `SB11` — Generic event/action extensions for exchange and investment
- `SB12` — Visualization genericity for third scenario
- `SB13` — Design matrix factor materialization v2
- `SB14` — Metamorphic and conservation tests
- `SB15` — Headless experiment CLI and manifest hardening
- `SB16` — Performance and comparability budgets for third scenario
- `SB17` — Final cross-repo red-team closure


## Execution rule

Codex must run the subbundles in order. After each phase, it must perform a refactor/QA stop before continuing. Empty proof transcripts are not valid evidence.

## Files

- `analysis/01-current-state-after-v8.md`
- `analysis/02-critical-findings.md`
- `architecture/01-target-architecture.md`
- `architecture/02-third-scenario-exchange-investment-elite.md`
- `plan/01-phase-plan.md`
- `subbundles/*/README.md`
- `CanDoItAll_WebGlEngine_Economy_Followup_v9_Checklists.xlsx`
