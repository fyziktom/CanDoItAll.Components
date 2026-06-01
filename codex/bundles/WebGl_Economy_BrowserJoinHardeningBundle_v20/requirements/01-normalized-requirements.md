# Normalized Requirements

| Requirement | Summary | Hard constraints |
|---|---|---|
| R01 | Preserve cross-repo branch and boundary discipline. | No new branch; Components has no Economy references; Economy owns joined simulation + visualization. |
| R02 | Add a generic Components browser apply adapter for `WebGlRunFrameApplyResult`. | No Economy terms or references; typed result includes counts and runtime diagnostics. |
| R03 | Harden Components stage barriers and diagnostics. | Cover active/object motion, render-idle, event/manual-step, unknown policy behavior, bounded journal order. |
| R04 | Export a bounded generic runtime snapshot for browser state. | Current frame, stage, motion, journal, barrier, warning, and error state; no Economy terms. |
| R05 | Add a desktop-only Economy sandbox page skeleton. | Large-screen only; load fixture; apply one frame; pause, step, seek, snapshot, analyze. |
| R06 | Harden real scenario artifact runner outputs and readiness language. | Canonical artifacts stable; no canonical `DateTimeOffset.UtcNow`; browser readiness wording must not overclaim. |
| R07 | Complete strict visual mappings for at least one browser-smoke probe. | Strict profile disallows fallback/no-op where expected; fallback remains explicit diagnostic mode. |
| R08 | Wire durable session and snapshot persistence hooks. | Export/import validates experiment path, input hash when present, step, and snapshot hash. |
| R09 | Add reusable snapshot analysis facets. | Generic facets; no hard-coded water/well/farmer/land logic in reusable analyzers. |
| R10 | Keep backend registry ledger-ready without mixing domains. | Missing backend errors are clear; descriptor-only ledger readiness test does not implement full ledger UI. |
| R11 | Produce desktop browser smoke proof artifacts. | 1440x900+ only; JSON artifacts and screenshot when Playwright/browser proof is available. |
| R12 | Add performance and scalability gates. | Moderate desktop scenario counts with timings, sizes, and warning thresholds. |
| R13 | Audit domain leakage and line-size/refactoring gates. | Generic layers avoid example terms; large files have split plan or pass thresholds. |
| R14 | Complete final validation and closure. | Required builds/tests/audits pass or blockers are explicit; transcripts non-empty; readiness answer explicit. |

## Assumptions And Risks

## Critical Path Risks

- The Economy browser page depends on the Components browser apply adapter and existing WebGL scene assets being available from the consuming host.
- Full browser proof may require an Economy host route that can serve the page and WebGL static assets.
- Existing Economy solution warnings may be unrelated to this bundle; the warning budget must separate known legacy warnings from new bridge/sandbox warnings.

## Validation Risks

- Headless scenario success is not browser-runtime success.
- Tests that only assert non-empty artifacts are supporting proof, not semantic proof for critical subbundles.
- Browser screenshot proof must inspect the open desktop page and not only save a file.

## Reopen Triggers

- Any Components source begins referencing Economy.
- Any readiness report says browser execution is ready before browser runtime apply proof exists.
- Strict visual mapping proof still uses fallback object or no-op pose/symbol fallback.
- Session import accepts mismatched input pack or snapshot hash.
- Browser smoke cannot load/apply a frame on a large-screen viewport.
