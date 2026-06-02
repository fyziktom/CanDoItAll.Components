# Critical Findings And Required Follow-up

| Finding | Severity | Area | Why it matters | Owner subbundle |
|---|---:|---|---|---|
| F01 | High | Proof | Empty or weak proof artifacts can let regressions pass review. | SB01 |
| F02 | High | Economy Components | Sandbox component cannot be safely consumed outside Node without manual DI wiring. | SB02 |
| F03 | High | Scenario packs | Path-centric scenario/session APIs are not portable across Docker/package/enterprise hosts. | SB03, SB04 |
| F04 | Medium | Browser UX | Runtime catalog exists but UI still only loads a default scenario. | SB03, SB09 |
| F05 | High | Session service | Sync-over-async snapshot persistence is unsafe for server/UI/remote stores. | SB05 |
| F06 | Critical | WebGlRunLib | Frame apply can still silently drop direct commands if validation is skipped. | SB06 |
| F07 | Critical | Browser apply | Reset failures should not allow command batch mutation of a stale scene. | SB06 |
| F08 | High | WebGlRunLib/WebGlLib | Scene document runtime options are ignored during browser reset/import. | SB07 |
| F09 | High | Generic/domain boundary | `source.*` can hide domain semantics unless provenance is typed and limited. | SB08 |
| F10 | Medium | Performance | No enforceable resource budget for large simulations. | SB10 |
| F11 | Medium | Package readiness | Static `0.1.0` packages and local-cache proof still risk stale package validation. | SB11 |
| F12 | High | Final validation | Cross-repo browser proof must prove real user flows, not just rendered screenshots. | SB12 |

## Critical path

SB01 gates everything. SB02-SB04 stabilize Economy runtime consumption. SB06-SB08 stabilize generic run safety. SB09-SB12 prove the result with real browser/package/stress validation.

## Reopen triggers

Reopen the previous subbundle immediately if:
- a downstream proof uses direct test fixtures, absolute repo paths, stale package feeds, or manually seeded runtime-only signals;
- a browser proof passes without command/stage diagnostics proving what was applied;
- a frame apply failure is represented only as log text but still mutates the scene;
- a package-mode proof restores from a global cache or private feed before the fresh local feed;
- a scenario can be listed but not loaded through the same public service/extension a consuming host would use.
