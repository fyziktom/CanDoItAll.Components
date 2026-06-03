# Execution report template

Status: superseded by `reviews/01-execution-report.md`

## Subbundle completion table

| Subbundle | Status | Gate result | Proof manifest | Notes |
| --- | --- | --- | --- | --- |
| SB01 | Completed | Pass | `proof/SB01/manifest.md` | Added proof-integrity audit; v2 empty artifacts fail, v4 prepared tree passes; current-state inventory and baseline hashes recorded. |
| SB02 | Completed | Pass | `proof/SB02/manifest.md` | Runner now stops after `FromFrame` errors; adapter exposes `PreApplyValidationFailed`, `ResetFailed`, and `BatchFailed`. |
| SB03 | Completed | Pass | `proof/SB03/manifest.md` | Legacy playback overload rejects multi-frame input; explicit `ApplyPlaybackAsync` applies frames in order and stops on first failed frame. |
| SB04 | Completed | Pass | `proof/SB04/manifest.md` | Economy sandbox now builds explicit replay from initial scene to target frame; Last applies `0,1,2` in route proof. |
| SB05 | Prepared | Pending | `proof/SB05/manifest.md` | |
| SB06 | Prepared | Pending | `proof/SB06/manifest.md` | |
| SB07 | Prepared | Pending | `proof/SB07/manifest.md` | |
| SB08 | Prepared | Pending | `proof/SB08/manifest.md` | |
| SB09 | Prepared | Pending | `proof/SB09/manifest.md` | |
| SB10 | Prepared | Pending | `proof/SB10/manifest.md` | |
| SB11 | Prepared | Pending | `proof/SB11/manifest.md` | |
| SB12 | Prepared | Pending | `proof/SB12/manifest.md` | |

## Final validation matrix

- Components build:
- Components focused tests:
- Components package proof:
- WebGlLib-only sample proof:
- Economy build:
- Economy focused tests:
- Economy package/project mode proof:
- Browser proof:
- Proof integrity validator:
- Boundary audits:
- Remaining known limitations:

## Browser Validation Analytics

| Subbundle | Route/window | Viewport | Evidence | Result |
| --- | --- | --- | --- | --- |
| SB01 | n/a | n/a | Browser behavior not claimed by SB01. | Pass |
| SB02 | n/a | n/a | Fake applier/runtime tests: `bundle://proof/SB02/transcripts/passing-tests.txt`; browser behavior not claimed. | Pass |
| SB03 | n/a | n/a | Fake runtime multi-frame proof: `bundle://proof/SB03/transcripts/passing-tests.txt`; browser behavior not claimed. | Pass |
| SB04 | `/economy/simulation-sandbox` | in-app browser viewport | Runtime diagnostics JSON: `bundle://proof/SB04/browser/runtime-diagnostics.json`; console review: `bundle://proof/SB04/browser/console-review.json`. | Pass |

## Subbundle Gate Results

| Subbundle | Entry gate | Closure gate | Downstream dependency check | Result |
| --- | --- | --- | --- | --- |
| SB01 | Pass: prepared-stage validator passed and SB01 had no prerequisites. | Pass: proof hygiene audit, inventory, source assertions, and hashes are non-empty. | SB02 must use the WebGlRunLib baseline hashes and source assertions before modifying runner/browser apply code. | Pass |
| SB02 | Pass: SB01 completed and WebGlRunLib baseline/source assertions were available. | Pass: failing-first and passing tests, source assertions, boundary audit, anti-stub audit, and full WebGlRunLib tests are non-empty. | SB03 must preserve fail-closed frame conversion and adapter failure reasons while adding multi-frame apply. | Pass |
| SB03 | Pass: SB02 completed and fail-closed branches were proven. | Pass: failing-first and passing multi-frame tests, full WebGlRunLib tests, source assertions, boundary audit, and anti-stub audit are non-empty. | SB04 must use runner or explicit playback apply for Economy UI replay. | Pass |
| SB04 | Pass: SB03 completed explicit multi-frame browser apply semantics. | Pass: failing-first component test, passing component tests, route runtime diagnostics, console review, source assertions, boundary audit, and hashes are non-empty. | SB05 must preserve deterministic UI replay diagnostics while addressing its own scope. | Pass |

## Raw Note Closure

| Raw note / requirement | Owner | Status | Evidence |
| --- | --- | --- | --- |
| R12 proof artifacts must be non-empty and assertion-backed. | SB01, SB12 | Partially solved | SB01 adds `bundle://scripts/audit_proof_integrity.py` and proof under `bundle://proof/SB01/`; SB12 must rerun it after all subbundles complete. |
| R01 Browser/runner paths must be fail-closed for all known frame/apply errors. | SB02, SB03, SB12 | Partially solved | SB02 closes runner `FromFrame` errors and adapter failure reason branches; SB03 still owns multi-frame playback helper semantics. |
| R02 Multi-frame playback must have explicit semantics. | SB03, SB04, SB12 | Partially solved | SB03 closes generic adapter API semantics; SB04 wires deterministic replay into the Economy UI and proves Last applies `0,1,2`; SB12 still owns final cross-bundle closure. |
