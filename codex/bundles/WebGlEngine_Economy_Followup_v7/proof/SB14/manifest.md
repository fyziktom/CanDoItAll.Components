# Proof Manifest - SB14

Status: completed

Implementation:
- Added generic Components runtime budget profiles `Small()`, `Medium()`, `Large()`, and `Stress()` while keeping Economy semantics out of Components.
- Added Economy performance profiles with named headless, projection, serialization, browser-load, and browser-batch-settle budgets.
- Promoted headless performance budget overages to readiness-affecting `not-comparable` outcomes through a separate `performance-budget` gate.
- Preserved economic failure classification by keeping scenario, simulation, and metric hard-gate failures separate from performance noise.
- Added large-run proof, browser idle/settled timing proof, profile docs, source assertions, anti-stub scans, and changed-file hashes.

Required artifacts:
- `proof/SB14/transcripts/performance-budget-tests.txt`
- `proof/SB14/artifacts/performance-budget-report.json`

Additional proof:
- `proof/SB14/transcripts/performance-budget-tests-failing-first.txt`
- `proof/SB14/transcripts/performance-budget-regression-tests.txt`
- `proof/SB14/artifacts/webglrun-performance-budget-metrics.json`
- `proof/SB14/artifacts/economy-performance-budget-report.json`
- `proof/SB14/browser/performance-budget-browser-proof.cjs`
- `proof/SB14/browser/performance-budget-browser-proof.json`
- `proof/SB14/browser/performance-budget-browser-proof.png`
- `proof/SB14/transcripts/performance-budget-browser-proof.txt`
- `proof/SB14/transcripts/webgl-sandbox-sb14.out.txt`
- `proof/SB14/transcripts/webgl-sandbox-sb14-stop.txt`
- `proof/SB14/transcripts/source-assertion-performance-budget-scan.txt`
- `proof/SB14/transcripts/anti-stub-audit.txt`
- `proof/SB14/transcripts/changed-file-hashes.txt`
- `proof/SB14/transcripts/bundle-validator-prepared-after-sb14.txt`

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| `WebGlRuntimeBudgetProfiles.Small/Medium/Large/Stress` | Components `WebGlRuntimeBudgetProfiles` | WebGL runtime options, diagnostics, large-run tests, browser proof | Provides named generic scene/runtime capacity profiles without carrying Economy terms or economic interpretation | `proof/SB14/transcripts/performance-budget-tests-failing-first.txt` captures the missing-profile compile failure; `proof/SB14/transcripts/source-assertion-performance-budget-scan.txt` confirms domain-neutral source |
| `EconomyExperimentPerformanceProfiles` | Economy SimulationSandbox | Headless runner and performance budget evaluator | Selects named research performance budgets for materialization, projection, metrics, snapshot build, serialization, browser load, and batch settle timing | `proof/SB14/transcripts/performance-budget-tests.txt` covers the profile-backed pass and hard failure classification |
| `performance-budget` readiness gate | Economy readiness reporter | Readiness report consumers, CLI/docs, final SB15 closure | Separates performance comparability from economic model validity; headless over-budget runs become `not-comparable`, not economic failures | `proof/SB14/transcripts/performance-budget-tests.txt` covers hard budget failure without economic failure |
| `performance-budget-report.json` | SB14 headless, generic large-run, and browser proof generation | Bundle closure and SB15 final readiness audit | Aggregates hard failure counts, browser idle/settled timings, top bottlenecks, and pass/fail assertions for large-run performance proof | `proof/SB14/artifacts/performance-budget-report.json` and `proof/SB14/browser/performance-budget-browser-proof.json` cite hard status and browser settled timing |
