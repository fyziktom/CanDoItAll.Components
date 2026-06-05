# SB01 closure matrix

Captured: 2026-06-04

## Gate decision

SB01 is complete as an audit phase. v8 has non-empty proof artifacts and many implemented gates, but the current source audit confirms the v9 follow-up is still necessary. The remaining gaps are recorded as owning subbundles instead of being treated as residual prose.

## v8 requirement closure

| v8 requirement | v8 owning subbundles | Current result | Evidence | v9 owner |
| --- | --- | --- | --- | --- |
| R01: baseline/proof integrity and pause/stop/runtime observer stability | SB01, SB02, SB03, SB15, SB16 | Partially solved | v8 proof files are non-empty; current `RunPlayback.StopPlaybackAsync` stops runtime before task drain, but cancels before the first runtime stop and only uses `waitForIdle:true` rather than the v9-required best-effort immediate stop. | SB02, SB03 |
| R02: idle timeout semantics must fail proof paths instead of becoming warnings | SB03, SB04, SB15, SB16 | Partially solved | `applyCommandBatchAndWait` hard-fails required idle proof, but `collectRuntimeIdleBlockers` still treats `render-loop:scheduled` as a blocker without semantic/visual/final-render-drain separation. | SB03, SB04 |
| R03: readiness and observer proof must be artifact backed | SB04, SB05, SB16 | Partially solved | Economy readiness requires valid evidence for oracle/browser observer bands, but RunPlayback diagnostics still call `WebGlRunObserverProof.Compare(runDocument, runDocument, ...)`. | SB05, SB08 |
| R04: design matrix factors must affect effective scenario inputs or be rejected | SB07, SB14, SB16 | Partially solved | Current bindings cover event magnitude, resource quantity, scenario metadata, behavior profile, and seed; v9 requires broader factor materialization for the third scenario. | SB13, SB16 |
| R05: golden oracle and metamorphic proof must protect economic semantics | SB08, SB09, SB16 | Partially solved | v8 external corpus exists and transcripts are non-empty; v9 requires expansion for the exchange/investment/elite scenario. | SB09, SB14 |
| R06: diagnostics must be classified or fail loudly | SB06, SB16 | Done for v8 scope | Economy readiness code has strict bands and warning budget behavior; no current v9 blocker found for the v8 diagnostic classification surface. | SB17 recheck |
| R07: store resolution policy must avoid implicit research-mode noise | SB10, SB09, SB16 | Done for v8 scope | v8 transcript files are non-empty and v9 analysis does not reopen store resolution as a primary blocker. | SB17 recheck |
| R08: metric/invariant evaluator must avoid silent fallback | SB11, SB09, SB16 | Done for v8 scope | v8 metric/invariant proof transcript is non-empty; v9 extends semantic coverage through metamorphic/conservation tests. | SB14, SB17 |
| R09: behavior profile lockfile and manifest metadata must be comparable | SB12, SB13, SB14 | Done for v8 scope | v8 behavior profile, headless manifest, and design comparison transcripts are non-empty. | SB15, SB16 |
| R10: headless artifacts must be reproducible and diffable | SB13, SB14, SB16 | Done for v8 scope | Current headless runner writes artifact validation and manifest data; v9 hardens CLI and manifest classification further. | SB15 |
| R11: browser evidence must remain observer evidence, not economic truth | SB04, SB15, SB16 | Partially solved | v8 browser evidence distinguishes browser observer proof, but self-comparison in RunPlayback means the observer snapshot is not fully independent yet. | SB05, SB12 |
| R12: browser performance proof must be comparable only as browser performance | SB13, SB14, SB15 | Done for v8 scope | v8 final red-team says browser performance has no headless validity impact. | SB16, SB17 recheck |
| R13: final cross-repo red-team closure | SB01, SB16 | Done for v8 closure, reopened by v9 findings | v8 final red-team closed with non-blocking warnings; v9 analysis identifies new hardening work and a missing structurally different scenario. | SB17 |

## Current source assertions

| Assertion | Source evidence | Closure impact |
| --- | --- | --- |
| Pause/stop ordering needs an immediate best-effort JS stop before cancellation and task drain. | `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` lines 427-487 show cancellation before `StopRuntimeActivityAsync(... waitForIdle:true)` and final late drain. | SB02 remains required. |
| Observer proof is self-referential. | `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` lines 79-85 call `WebGlRunObserverProof.Compare(runDocument, runDocument, BuildObserverSnapshot())`. | SB05 remains required. |
| Runtime idle does not distinguish semantic idle from final visual render drain. | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js` lines 44-81 include `render-loop:scheduled` as a blocker without two-probe or final-drain semantics. | SB03 remains required. |
| Command batch lifecycle is mostly present. | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/26-webgl-scene-command-batch.js` lines 63-101 and 152-169 surface settled/scheduled behavior and idle blockers. | SB04 should harden default usage/proof. |
| Generic Components boundary still contains domain-example terms in production. | `repo://src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs` lines 200-214 list economy/market/account/buyer/seller/price terms. | SB06 remains required. |
| Readiness has artifact-backed evidence checks. | `repo://src/CanDoItAll.Economy.SimulationSandbox/EconomyExperimentReadiness.cs` lines 463-490 require valid evidence when an explicit band is exercised, and lines 537-546 require valid oracle/browser evidence for `ResearchReady`. | v8 SB05 is materially done; v9 will harden artifact contracts. |
| Design factors are real but narrow. | `repo://src/CanDoItAll.Economy.SimulationSandbox/EconomyExperimentDesignHarness.cs` lines 511-543 support only scheduled-event magnitude, resource quantity, scenario metadata, behavior profile, and seed. | SB13 remains required. |
| Third exchange/investment/elite scenario is not present as a research input pack. | Repo search found investment services/tests and existing farmer/shared-well fixtures, but no dedicated multi-goods exchange/investment/elite scenario pack. | SB10-SB12 remain required. |

## Closure

SB01 closes the audit requirement. It does not close the behavior gaps; those are explicitly carried by SB02-SB17.
