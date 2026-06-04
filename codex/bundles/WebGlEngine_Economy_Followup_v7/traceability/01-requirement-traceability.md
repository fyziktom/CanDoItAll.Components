# Requirement traceability

| Requirement | Subbundles |
|---|---|
| R01 | SB01,SB02,SB03 |
| R02 | SB02,SB03 |
| R03 | SB04,SB10,SB15 |
| R04 | SB05 |
| R05 | SB06 |
| R06 | SB07 |
| R07 | SB08 |
| R08 | SB09 |
| R09 | SB10,SB13 |
| R10 | SB11 |
| R11 | SB12 |
| R12 | SB13 |
| R13 | SB14 |
| R14 | SB15 |

## Execution notes

- R01/R02 SB03 closure proof is captured in `proof/SB03/transcripts/command-lifecycle-tests.txt` and `proof/SB03/browser/staged-batch-settled-proof.json`.
- R03 SB04 closure proof is captured in `proof/SB04/transcripts/readiness-v2-tests.txt` and `proof/SB04/artifacts/readiness-v2-sample.json`; readiness now exposes v2 hard gates, warning budget, and `researchReady`.
- R04 SB05 closure proof is captured in `proof/SB05/transcripts/research-strict-mode-tests.txt` and `proof/SB05/artifacts/strict-mode-failure-examples.json`; research strictness is now an explicit policy threaded through loader, backend, runner, and readiness.
- R05 SB06 closure proof is captured in `proof/SB06/transcripts/store-resolution-policy-tests.txt` and `proof/SB06/artifacts/flow-resolution-sample.json`; flows now carry explicit store-resolution provenance and full rejection diagnostics.
- R06 SB07 closure proof is captured in `proof/SB07/transcripts/metric-invariant-registry-tests.txt` and `proof/SB07/artifacts/metric-oracle-results.json`; metrics and invariants now use strict registries, schema validation, deterministic rounding, and metric provenance metadata.
- R07 SB08 closure proof is captured in `proof/SB08/transcripts/behavior-profile-tests.txt` and `proof/SB08/artifacts/expanded-event-provenance.json`; behavior expansion profiles now carry version/hash descriptors, event provenance, and frame/readiness/run artifact metadata.
- R08 SB09 closure proof is captured in `proof/SB09/transcripts/golden-oracle-tests.txt` and `proof/SB09/artifacts/oracle-diff-sample.json`; core economic semantics now have golden oracle coverage, stable repeated frame hash chains, and no-oracle readiness classification cannot rise above exploratory without an explicit label.
- R09 SB10 closure proof is captured in `proof/SB10/transcripts/headless-runner-tests.txt` and `proof/SB10/artifacts/headless-run-manifest.json`; headless research runs are now catalog-first, emit statused self-contained artifacts, support batch status/run-hash maps, and keep path-based APIs legacy-compatible.
- R10 SB11 closure proof is captured in `proof/SB11/transcripts/reproducibility-manifest-tests.txt` and `proof/SB11/artifacts/manifest-diff-sample.json`; every headless run now emits a manifest v2 with repo/package/scenario/profile/policy/seed/frame/metric/artifact hashes, artifact citations, schema-versioned artifacts, completeness validation, and categorized manifest diffs.
- R11 SB12 closure proof is captured in `proof/SB12/transcripts/experiment-design-harness-tests.txt` and `proof/SB12/artifacts/design-matrix-summary.json`; experiment design matrices now isolate factors, seeds, repetitions, deterministic repeat stability, stochastic variance metadata, and hidden configuration drift.
- R09/R12 SB13 closure proof is captured in `proof/SB13/transcripts/headless-vs-browser-hash-tests.txt` and `proof/SB13/browser/observer-boundary-proof.json`; browser visualization is now observer-only, compares expected/browser-loaded document hashes, rejects runtime/UI failures as visual claims, and cannot mutate headless economic artifacts.
- R13 SB14 closure proof is captured in `proof/SB14/transcripts/performance-budget-tests.txt` and `proof/SB14/artifacts/performance-budget-report.json`; named small/medium/large/stress performance profiles now convert headless performance overages into `not-comparable` readiness through a separate `performance-budget` gate while browser load and batch-settle timing remain observer/runtime proof.
- R14 SB15 closure proof is captured in `proof/SB15/final-red-team-report.md`, `proof/SB15/transcripts/final-cross-repo-tests.txt`, and `proof/SB15/artifacts/final-readiness-summary.json`; final red-team closure classifies unknown event, ambiguous store, unknown metric, browser non-idle, and broken scenario hash as simulation/config/runtime/comparability failures that block economic conclusions.
