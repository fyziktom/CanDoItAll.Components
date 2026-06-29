# Requirement Traceability

| Raw Note | Exact Input Wording | Requirements | Owning Subbundles | Planned Proof |
|---|---|---|---|---|
| RAW01 | "Wee did preparation for publishing in part of basic components." | R01,R02,R13 | SB01,SB08,SB10 | Prior bundle pattern cited in analysis, phase plan checkpoints, final red-team closure. |
| RAW02 | "study system of how we improved it (look at last bundles)" | R01,R02,R13 | SB01,SB10 | `StandardComponents_PublishingReadiness_v1` and WebGL v17 pattern review recorded; execution report closes pattern reuse. |
| RAW03 | "prepare new bundle that will focus on preparation/refactor/improvement/hardening and true validation of canvas and floating windows parts." | R02,R05-R12 | SB01-SB09 | Inventory, contract tests, runtime proof, Playwright matrix, package/API/docs proof. |
| RAW04 | "do not do webgl part yet." | R03 | SB01,SB04,SB08,SB09,SB10 | WebGL exclusion source assertions and final raw-note closure. |
| RAW05 | "We must preserve all functionality." | R04,R06-R11 | SB02-SB10 | Failing-first and passing proof for behavior changes, no-regression tests, browser proof, package/API approval. |
| RAW06 | "make it more maintainable, clear and well documented for soon publishing as opensource." | R01,R02,R07,R12,R13 | SB01,SB04,SB09,SB10 | Module map, ownership docs, package metadata, README/version alignment, final transfer checklist. |
| RAW07 | "avoid to use npm or being dependent on it... implementation must be in pure JS." | R07,R12,R14 | SB04,SB06,SB07,SB09,SB10 | Runtime dependency assertions, package assertions, pure-JS source checks, and final red-team proof. |

## Requirement To Subbundle Matrix

| Requirement | SB01 | SB02 | SB03 | SB04 | SB05 | SB06 | SB07 | SB08 | SB09 | SB10 |
|---|---|---|---|---|---|---|---|---|---|---|
| R01 prior workflow pattern | Owns | Supports | Supports | Supports | Supports | Supports | Supports | Supports | Supports | Closes |
| R02 inventory all surfaces | Owns | Supports | Supports | Supports | Supports | Supports | Supports | Supports | Supports | Audits |
| R03 exclude WebGL | Owns | Enforces | Enforces | Enforces | Enforces | Enforces | Enforces | Enforces | Enforces | Closes |
| R04 preserve functionality | Plans | Owns | Owns | Owns | Owns | Owns | Owns | Owns | Owns | Audits |
| R05 window ownership | Maps | Owns | Supports | Supports | Supports | Supports | Owns | Proves | Documents | Closes |
| R06 contract tests | Maps | Supports | Owns | Supports | Supports | Supports | Supports | Verifies | Approves | Closes |
| R07 asset/runtime boundaries | Maps | Supports | Supports | Owns | Verifies | Verifies | Verifies | Proves | Documents | Closes |
| R08 workbench validation | Maps | Supports | Supports | Supports | Owns | Supports | Supports | Proves | Documents | Closes |
| R09 calendar/preview validation | Maps | Supports | Supports | Supports | Supports | Owns | Supports | Proves | Documents | Closes |
| R10 floating-window validation | Maps | Owns | Supports | Supports | Supports | Supports | Owns | Proves | Documents | Closes |
| R11 Playwright matrix | Plans | Supports | Supports | Supports | Feeds | Feeds | Feeds | Owns | Supports | Closes |
| R12 package/API/docs | Maps | Supports | Supports | Supports | Supports | Supports | Supports | Supports | Owns | Closes |
| R13 red-team closure | Plans | Feeds | Feeds | Feeds | Feeds | Feeds | Feeds | Feeds | Feeds | Owns |
| R14 pure-JS runtime and no npm runtime dependency | Maps | Supports | Supports | Owns | Verifies | Owns | Owns | Verifies | Owns | Closes |
