# Proof Manifest

Subbundle: `SB11`
Status: `Completed`
Prepared by: Codex
Completed at UTC: `2026-06-02T05:28:03Z`

## Changed File Hashes

| Repo | Path | Before SHA-256 | After SHA-256 | Why changed |
| --- | --- | --- | --- | --- |
| CanDoItAll.Economy | `tests/CanDoItAll.Economy.Tests/SimulationPreparationTests.cs` | `214aa060d52e18c9c7fd95e652a173dfcac0caefdebf68cd0650872a8e0c282a` | `429530369f6c2555cba67f9649732965c9cc14dae25146ff4819b925882daf8a` | Extended the large generic shared-resource proof from visual-frame mapping into strict WebGlRun projection, validation, deterministic replay fingerprinting, and count/timing artifact export. |

Hash transcript: `proof/SB11/transcripts/sb11-file-hashes.txt`.

## Commands

| Command | Working directory | Transcript path | Result |
| --- | --- | --- | --- |
| `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter EconomyPerformanceProof_CompilesMaterializesAndMapsLargeGenericSharedResourceScenario` with `ECONOMY_PERFORMANCE_PROOF_PATH=proof\SB11\artifacts\large-generic-webglrun-proof.json` | `C:\repositories\CanDoItAll.Economy` | `proof/SB11/transcripts/failing-first-large-generic-webglrun-determinism.txt` | Failed first: raw serialized run documents differed only by `SavedAtUtc`, proving the replay check needed a deterministic structural fingerprint. |
| Same focused command after fingerprint fix | `C:\repositories\CanDoItAll.Economy` | `proof/SB11/transcripts/passing-large-generic-webglrun-proof.txt` | Passed: 1 test; artifact reports 51 frames, 15,000 visual actions, 15,000 WebGlRun stages, 10,000 motions, and matched replay fingerprint. |
| `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter "SimulationSandboxWorkflow|EconomyPerformanceProbe|EconomyWebGlSnapshotVisualStateBuilder"` | `C:\repositories\CanDoItAll.Economy` | `proof/SB11/transcripts/passing-sb11-focused-tests.txt` | Passed: 12 tests. Existing `ncalc`/DI prune warnings preserved. |
| `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter ValidatorRejectsUnsupportedActionKindByDefaultButPermissiveDiagnosticsStillWork` | `C:\repositories\CanDoItAll.Economy` | `proof/SB11/transcripts/passing-unsupported-action-negative-proof.txt` | Passed: unsupported scenario-specific action does not silently map to wait in strict mode. |
| `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter RealScenarioReadinessReporter_AnswersRequiredQuestionsWithArtifactCitations` | `C:\repositories\CanDoItAll.Economy` | `proof/SB11/transcripts/passing-real-scenario-readiness-probe.txt` | Passed: shared-well and farmer-land fixtures run headlessly, strict validation passes, and browser playback gaps are explicit. |
| Source assertion scan | `C:\repositories\CanDoItAll.Economy` | `proof/SB11/transcripts/sb11-source-assertions.txt` | Passed. |
| Browser-host gap scan | `C:\repositories\CanDoItAll.Economy` | `proof/SB11/transcripts/browser-host-gap-scan.txt` | Passed: generated browser route/actions are explicitly listed as missing by readiness proof. |
| Anti-stub, refactor, and Components domain-boundary scan | `C:\repositories\CanDoItAll.Economy` | `proof/SB11/transcripts/sb11-anti-stub-and-boundary-scan.txt` | Passed: no stub markers, no new concrete scenario-name switches in SB11 diff, no Economy references in Components WebGl source/tests. |
| `python scripts\validate_bundle.py --stage execution --profile initiative` | `C:\repositories\CanDoItAll.Components\codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1` | `proof/SB11/transcripts/bundle-validate-execution.txt` | Passed: `Bundle validation passed for stage=execution, profile=initiative, subbundles=14`. |
| `git status` and SB11 diff stat capture | `C:\repositories\CanDoItAll.Economy`, `C:\repositories\CanDoItAll.Components` | `proof/SB11/transcripts/economy-git-diff-check.txt`, `proof/SB11/transcripts/components-git-diff-check.txt` | Passed as evidence capture; existing dirty bundle/worktree state preserved. |

## Source Assertions

| Assertion | Source path | Line/range or search term | Verified by |
| --- | --- | --- | --- |
| Large proof projects generic visual frames into WebGlRun documents. | `tests/CanDoItAll.Economy.Tests/SimulationPreparationTests.cs` | `EconomyPerformanceProof_CompilesMaterializesAndMapsLargeGenericSharedResourceScenario`, `EconomyWebGlRunProjector` | `proof/SB11/transcripts/sb11-source-assertions.txt` |
| Large proof uses a generic visual mapping, not a scenario-name switch. | `tests/CanDoItAll.Economy.Tests/SimulationPreparationTests.cs` | `BuildLargeGenericVisualMapping` | `proof/SB11/transcripts/sb11-source-assertions.txt`, `proof/SB11/transcripts/sb11-anti-stub-and-boundary-scan.txt` |
| Deterministic replay is checked with a structural fingerprint. | `tests/CanDoItAll.Economy.Tests/SimulationPreparationTests.cs` | `BuildWebGlRunReplayFingerprint`, `deterministicReplayMatched` | `proof/SB11/transcripts/sb11-source-assertions.txt` |
| Strict validation rejects unsupported actions instead of silently waiting. | `tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs` | `ValidatorRejectsUnsupportedActionKindByDefaultButPermissiveDiagnosticsStillWork` | `proof/SB11/transcripts/passing-unsupported-action-negative-proof.txt` |
| Built-in examples remain explicit examples over generic definitions. | `src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleScenarioDefinitionMaterializer.cs` | `SimpleScenarioFactoryMaterializerHandler`, `SimpleStateTransitionMaterializerHandler` | `proof/SB11/scenario-inventory.md`, `proof/SB11/transcripts/sb11-source-assertions.txt` |
| Experiment fixtures carry visual mapping data outside bridge code. | `tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/*/visual.mapping.json` | `visual-mapping/v1`, `actionMappings` | `proof/SB11/scenario-inventory.md`, `proof/SB11/transcripts/sb11-source-assertions.txt` |

## Semantic Adequacy Gate

| Item | Required evidence | Status | Artifact |
| --- | --- | --- | --- |
| Shallow-pass trap named | A test could stop at visual-frame counts and never prove WebGlRun projection, strict validation, replay determinism, or unsupported-action failure. | Passed | `proof/SB11/semantic-invariants.md` |
| Adversarial negative proof | Unsupported scenario-specific action fails under strict mapping unless permissive diagnostics are explicit. | Passed | `proof/SB11/transcripts/passing-unsupported-action-negative-proof.txt` |
| Semantic positive proof | Large generic shared-resource scenario compiles, materializes, maps to visual frames, projects to WebGlRun stages/motions, validates, and replays deterministically. | Passed | `proof/SB11/transcripts/passing-large-generic-webglrun-proof.txt`, `proof/SB11/artifacts/large-generic-webglrun-proof.json` |
| Anti-stub audit | No TODO/stub/placeholder markers in touched paths; no new concrete scenario-name switches; Components remains domain-clean. | Passed | `proof/SB11/transcripts/sb11-anti-stub-and-boundary-scan.txt` |
| Raw-note closure | Economy as first generic consumer and generic simulator proof closed for SB11 scope. | Passed | `reviews/01-execution-report.md`, `traceability/01-requirement-traceability.md` |
| Downstream smoke | Required focused suite plus readiness probe pass on final code. | Passed | `proof/SB11/transcripts/passing-sb11-focused-tests.txt`, `proof/SB11/transcripts/passing-real-scenario-readiness-probe.txt` |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `large-generic-webglrun-proof.json` | `SimulationPreparationTests.EconomyPerformanceProof_CompilesMaterializesAndMapsLargeGenericSharedResourceScenario` | Bundle proof, SB12/SB13 downstream scale gates | Written when `ECONOMY_PERFORMANCE_PROOF_PATH` is set; records scenario sizes, counts, timings, input hash, run id, and replay fingerprint hash. | Failing-first transcript shows raw timestamp-based document comparison is not deterministic enough. |
| Large generic visual mapping | `BuildLargeGenericVisualMapping` in the test proof | `EconomyWebGlRunProjector`, `EconomyWebGlRunValidator` | Supplies category, action, pose, symbol, and anchor mappings without checking concrete example names. | Unsupported action test proves missing/unknown actions fail under strict policy. |
| Real scenario readiness report | `EconomyRealScenarioReadinessReporter` | SB11 proof, SB13 browser planning | Generated in temp output by focused readiness test; cites shared-well/farmer-land artifacts and missing browser route/actions. | Browser route/action gap remains explicit instead of being treated as a successful browser smoke. |

## Browser Validation Analytics

| Route | Viewport | Actions | Screenshot/log artifacts | Result |
| --- | --- | --- | --- | --- |
| N/A | N/A | SB11 changed command-level Economy proof/test coverage only. No Economy browser host route for generated WebGlRun playback was available; readiness proof explicitly lists the missing generated-route actions. | Command-level proof: `proof/SB11/transcripts/passing-large-generic-webglrun-proof.txt`; route gap: `proof/SB11/transcripts/browser-host-gap-scan.txt`; readiness proof: `proof/SB11/transcripts/passing-real-scenario-readiness-probe.txt`. | Passed / browser proof not applicable for SB11. |

## Refactor Gate Result

- Touched files reviewed: `tests/CanDoItAll.Economy.Tests/SimulationPreparationTests.cs`.
- Duplicates removed: deterministic replay comparison is isolated in `BuildWebGlRunReplayFingerprint`; mapping setup is isolated in `BuildLargeGenericVisualMapping`.
- Layering checked: SB11 added no Components changes and no Economy references to Components WebGl source/tests.
- Fixture-specific code removed: none introduced; the SB11 diff contains no concrete example scenario names or new switch statements.
- Docs/tests updated: scenario inventory, manifest, semantic invariants, refactor gate, execution report and traceability updated.
- Remaining refactor risk: SimpleAccounts still keeps two named example factory shims for legacy/current examples, while generic scheduled-event materialization is handled by `SimpleStateTransitionMaterializerHandler`; no new bridge hard-coding was added.
