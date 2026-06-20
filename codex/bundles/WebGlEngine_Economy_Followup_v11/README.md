# CanDoItAll_WebGlEngine_Economy_Followup_Hardening_Bundle_v11

Prepared: 2026-06-05T14:42:05Z

## Purpose

This follow-up bundle hardens the current WebGL/Economy simulation stack after the previous Codex pass. It focuses on whether CanDoItAll can safely investigate economic simulations without simulator/runtime bugs contaminating experimental conclusions.

## Current verdict

The system is now usable for exploratory simulation work, scenario debugging, headless runs and visualization development. It is not yet safe to treat every output as research-grade economic evidence unless the run passes the strict evidence pipeline in this bundle.

Required research-grade path:

```text
scenario pack -> ResearchStrict load -> backend frames/deltas -> metrics/invariants -> external oracle corpus -> reproducibility manifest -> readiness report -> optional browser observer proof
```

Browser/WebGL remains observer evidence only. It must never mutate, reinterpret or validate economic truth by itself.

## Major findings

| Id | Severity | Area | Finding |
| --- | --- | --- | --- |
| F01 | P0 | Components genericity | Generic WebGlRunActionKinds now uses DirectedFlowVisual, fixing prior ResourceTransferVisual leak, but the generic/domain boundary still needs a broader scan over all public constants, docs, tests and generated artifacts. |
| F02 | P0 | Domain drivers | Economy bridge now maps ResourceTransferVisual to DirectedFlowVisual through a mapping driver, which is the right direction; follow-up should formalize a reusable domain driver contract and prove Economy remains a driver, not a generic dependency. |
| F03 | P0 | Boundary audit | EconomyWebGlMappingBoundary owns forbidden domain term list now, but it is still a static Economy-specific list; this is acceptable as driver-owned guardrail, but needs config/CI validation and extension to new multi-goods terms such as capital, investor, elite, exchange, equity, credit and claim. |
| F04 | P0 | Pause/runtime | RunPlayback now uses WebGlRunPlaybackStopCoordinator with immediate stop before cancel/final drain. This likely fixes the user-observed Pause symptom, but must be locked with real browser proof and stale callback checks. |
| F05 | P0 | Third scenario | multi-goods-elite exists and is meaningfully different, but needs strict canary proof that all new event kinds are semantically handled and not just treated as generic transfers or aliases. |
| F06 | P0 | Research readiness | Readiness should not be caller-asserted. Any browser/runtime/oracle/UI exercise flag must be derived from artifact hashes, live diagnostics, manifest citations and validator results. |
| F07 | P1 | Simulation maintainability | SimpleSimulationStateTransitionEngine.Mutations remains a large, high-risk semantic hub. Store resolution, transfer/rejection, and relationship/effect handlers should be split to reduce accidental coupling and noise. |


## Phase gates

- Gate A after SB01-SB05: generic/domain boundary must be proven before adding scenario features.
- Gate B after SB06-SB10: third scenario and runtime/browser proof must be stable before readiness claims.
- Gate C after SB11-SB16: simulation semantics, oracles and performance must be hardened before final closure.
- Gate D at SB18: cross-repo red-team closure; no empty proof artifacts, no uncited claims, no research-ready status without evidence.

## Execution rules for Codex

- Work subbundle by subbundle in order.
- After each subbundle, run focused tests and update `proof/SBxx/manifest.md`.
- Every claim must cite a source file, test output, browser proof artifact or generated manifest.
- Do not mark research-ready unless the readiness report is evidence-backed and machine-verifiable.
- Do not add domain concepts into Components generic libraries. Use domain drivers.
