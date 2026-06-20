# Proof manifest SB09

Status: completed

- Objective: Large simulation performance budgets and resource stress.
- Gate: Budget test outputs machine-readable metrics and fails on regression thresholds.
- Result: Passed. A deterministic WebGlRunLib stress harness now emits JSON metrics and asserts elapsed time, average frame-apply time, allocation ceiling, frame/stage/motion counts, recreate/reset count, and batching savings.

## Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Proof |
| --- | --- | --- | --- | --- |
| `webglrun-performance-budget/v1` metrics JSON | `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunPerformanceBudgetTests.cs` | Test runner, bundle proof, regression review | Test generates a generic 500-object, 120-frame, 2-recreate run and writes metrics to `CDA_WEBGLRUN_BUDGET_METRICS_PATH` or test output. | `bundle://proof/SB09/metrics/webglrun-performance-budget-metrics.json`; `bundle://proof/SB09/transcripts/webglrun-performance-budget-tests.txt` |
| Budget assertions | `WebGlRunPerformanceBudgetTests` | xUnit assertions | Each budget has actual, threshold, and pass/fail fields; the test fails when any threshold fails. | `bundle://proof/SB09/metrics/webglrun-performance-budget-metrics.json` |
| Resource/cache diagnostics coverage | WebGlLib runtime diagnostics/proof snapshot tests | Browser runtime diagnostics consumers | Existing GLB cache, dispose, recreate, and runtime-budget counters remain round-tripped and tested. | `bundle://proof/SB09/transcripts/webgllib-resource-diagnostics-tests.txt`; `bundle://proof/SB09/transcripts/resource-cache-dispose-diagnostics-scan.txt` |

No new production signal, state record, or event was added in SB09; this subbundle adds a test harness and proof metrics.

## Source Changes

- `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunPerformanceBudgetTests.cs`

## Proof Artifacts

- Metrics JSON: `bundle://proof/SB09/metrics/webglrun-performance-budget-metrics.json`
- Budget test transcript: `bundle://proof/SB09/transcripts/webglrun-performance-budget-tests.txt`
- Focused WebGlRunLib tests: `bundle://proof/SB09/transcripts/webglrun-focused-performance-resource-tests.txt`
- WebGlLib resource diagnostics tests: `bundle://proof/SB09/transcripts/webgllib-resource-diagnostics-tests.txt`
- Components build: `bundle://proof/SB09/transcripts/components-build-after-performance-budget.txt`
- Source assertion scan: `bundle://proof/SB09/transcripts/source-assertion-performance-budget-scan.txt`
- Resource/cache scan: `bundle://proof/SB09/transcripts/resource-cache-dispose-diagnostics-scan.txt`
- Anti-stub scan: `bundle://proof/SB09/transcripts/anti-stub-performance-budget-scan.txt`
- Domain-boundary scan: `bundle://proof/SB09/transcripts/domain-boundary-performance-budget-scan.txt`
- SB09 diff check: `bundle://proof/SB09/transcripts/components-sb09-diff-check.txt`
- Changed-file hashes: `bundle://proof/SB09/transcripts/changed-file-hashes.txt`
- Proof hygiene inventory: `bundle://proof/SB09/transcripts/proof-hygiene-inventory.txt`
- Prepared-stage bundle validator: `bundle://proof/SB09/transcripts/bundle-validator-after-sb09.txt`

## Validation Commands

- `dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore --filter "FullyQualifiedName~WebGlRunPerformanceBudgetTests" --logger "console;verbosity=normal"`
- `dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore --filter "FullyQualifiedName~WebGlRunDocumentRunnerTests|FullyQualifiedName~WebGlRunBrowserApplyAdapterTests|FullyQualifiedName~WebGlRunPerformanceBudgetTests" --logger "console;verbosity=normal"`
- `dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore --filter "FullyQualifiedName~WebGlRuntimeDiagnosticsTests|FullyQualifiedName~WebGlSceneViewExternalImportLifecycleTests" --logger "console;verbosity=normal"`
- `dotnet build .\CanDoItAll.Components.slnx --no-restore`
- `python scripts\validate_bundle.py --stage prepared --profile initiative`
