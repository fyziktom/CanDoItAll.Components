# SB15 Final Cross-Repo Red-Team Report

Status: passed

Final verdict: conditionally research-ready. The stack can support gated headless economic investigation when strict policy, golden-oracle coverage, reproducibility manifests, experiment design metadata, performance budgets, and observer/runtime gates all pass.

What can be claimed:
- Headless Economy runs can be investigated as economic evidence when strict research policy, oracle/readiness, reproducibility, artifact, design, and performance gates pass.
- WebGL/RunPlayback can be used as observer proof for generic browser/runtime behavior, document-hash agreement, pause/idle state, and browser timing.
- Simulator, projection, runtime, performance, and browser failures are now classified separately enough to avoid turning infrastructure noise into economic conclusions.

What cannot be claimed:
- Ungated exploratory/demo runs are not evidence for strong economic conclusions.
- Browser/WebGL playback is not the economic source of truth.
- Over-budget headless runs are `not-comparable`; they must not be interpreted as economic model failures or successes.

## Final Proof

| Proof area | Result | Evidence |
|---|---|---|
| Components focused tests | Passed: 58 WebGlLib tests, 62 WebGlRunLib tests | `proof/SB15/transcripts/final-cross-repo-tests.txt` |
| Economy focused tests | Passed: 32 hardening/probe/CLI tests | `proof/SB15/transcripts/final-cross-repo-tests.txt` |
| Browser pause/idle | Passed: active runtime motion observed before Pause; UI stayed paused; runtime idle diagnostics agreed; no queued stages or motions remained | `proof/SB15/browser/pause-idle-proof.json`, `proof/SB15/transcripts/browser-pause-idle-proof.txt` |
| Browser performance | Passed: browser load 2857.593 ms under 10000 ms; batch settle 2373.374 ms under 5000 ms; runtime idle settled | `proof/SB15/browser/performance-budget-browser-proof.json`, `proof/SB15/transcripts/browser-performance-budget-proof.txt` |
| Performance budget | Passed: SB14 report status `comparable`, hard failures 0, warnings 0 | `proof/SB14/artifacts/performance-budget-report.json` |

## Red-Team Classifications

| Failure mode | Expected classification | Observed classification | Economic conclusion allowed? | Evidence |
|---|---|---|---|---|
| Unknown event | Strict simulation error; exploratory warning | `unknown-event-handler` is an error in strict mode and a warning in exploratory mode | No | `StrictMode_RejectsUnknownEventKindAndExploratoryStillWarns`, `GoldenOracleSuite_NegativeScenariosFailForExpectedReasons`, `TransitionEngine_ReportsUnknownEventKind` |
| Ambiguous store | Store-resolution policy error | `ambiguous-store-resolution` blocks strict transfer; flow metadata explains explicit and zero-accepted paths | No | `StoreResolution_StrictModeFailsAmbiguityAndExactPolicyPasses`, `StoreResolutionPolicy_FlowMetadataExplainsExplicitResolutionAndZeroAcceptedTransfer` |
| Unknown metric | Metric/invariant registry validation error | `unknown-metric-kind`, `unknown-invariant-kind`, and missing metric references reject strict validation | No | `MetricAndInvariantRegistry_RejectsUnknownKindsAndMissingMetricInStrictMode`, `MetricRegistry_StrictModeAddsProvenancePrecisionAndRequiredMetadataErrors` |
| Browser non-idle | Browser observer/runtime failure, not headless economic failure | Final browser proof starts from active runtime motion and proves Pause drains to idle; component tests cover idle timeout as typed runtime failure | No | `proof/SB15/browser/pause-idle-proof.json`, `Adapter_apply_playback_fails_when_runtime_idle_times_out` |
| Broken scenario hash | Manifest/scenario drift, not comparable | Reproducibility diff categorizes `scenarioPackHash` and `runHash` changes instead of treating changed artifacts as equivalent | No | `ReproducibilityManifest_RepeatedCatalogRunsAreStableAndDiffReportsChanges`, `proof/SB11/artifacts/manifest-diff-sample.json` |

## Residual Risks

- The final Economy focused run emitted existing NuGet warnings (`NU1701` for `ncalc`, plus one `NU1510` package-prune warning). They did not fail the focused final tests, but dependency modernization remains a normal follow-up concern.
- Final proof is focused on the WebGL/Economy research-hardening surface. It does not claim every unrelated Economy ledger, persistence, or token test path was re-run.
- Browser proof used installed Chrome through the local Playwright module path because bundled Playwright browsers were not downloaded in this workspace.
