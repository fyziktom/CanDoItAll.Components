# Proof manifest - SB16

Status: completed

## Scope

SB16 adds performance and comparability proof for the third Economy scenario and cross-repo WebGlRun playback budget coverage. Production budget semantics already classify headless overages as hard comparability failures and browser/visual overages as warnings; SB16 adds focused proof that those rules hold for `multi-goods-elite`.

## Changed Files

Changed-file hashes:

- `bundle://proof/SB16/transcripts/changed-file-hashes.txt`

Economy tests:

- `repo://C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyPerformanceProbeTests.cs`

Components proof path:

- Existing `WebGlRunPerformanceBudgetTests` executed with SB16 metrics output.

## Measurement Matrix

| Required signal | Proof |
| --- | --- |
| Materialization | `performance-budget-report.json` includes `headless-load-materialize` for `multi-goods-elite`. |
| Projection | `performance-budget-report.json` includes visual/WebGL projection with object/link/stage counts. |
| Metrics | `performance-budget-report.json` includes metric and invariant evaluation. |
| Snapshot | `performance-budget-report.json` includes snapshot build and snapshot serialization measurements. |
| Browser settle | `performance-budget-report.json` includes `browser-batch-settle` as a browser-category observer budget; `webglrun-large-playback-budget-metrics.json` proves large playback/frame apply budgets on Components. |
| Large-run stress | `large-run-stress-proof.json` covers 100 actors, 500 resources, 500 stores, 1000 events, 500 visual actions, 1000 links, and 1000 symbols. |

## Proof Artifacts

- Third-scenario budget report: `bundle://proof/SB16/performance-budget-report.json`
- Large-run stress proof: `bundle://proof/SB16/large-run-stress-proof.json`
- Components WebGlRun budget metrics: `bundle://proof/SB16/webglrun-large-playback-budget-metrics.json`
- Third-scenario focused test: `bundle://proof/SB16/transcripts/multi-goods-performance-budget-test.txt`
- Economy large-run stress test: `bundle://proof/SB16/transcripts/large-run-stress-test.txt`
- Components WebGlRun budget test: `bundle://proof/SB16/transcripts/webglrun-large-playback-budget-test.txt`
- Source assertions: `bundle://proof/SB16/transcripts/source-assertions.txt`
- Anti-stub audit: `bundle://proof/SB16/transcripts/anti-stub-audit.txt`
- Changed-file hashes: `bundle://proof/SB16/transcripts/changed-file-hashes.txt`
- Bundle validator transcript: `bundle://proof/SB16/transcripts/bundle-validator.txt`
- Semantic invariants: `bundle://proof/SB16/semantic-invariants.md`

## Closure

SB16 passes. The third-scenario report records all required measurements under the large profile with zero hard budget failures and `comparable=true`. Its negative comparability section proves a headless overage produces `not-comparable`, while a browser-only overage remains a performance warning. The large-run stress and Components playback budget tests both pass.
