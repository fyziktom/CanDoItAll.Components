# SB09 Proof Manifest

Subbundle: `SB09-webglrunlib-runtime-integration`
Status: `Completed`
Prepared by: Codex
Completed at UTC: `2026-06-02T05:05:00Z`

## Changed File Hashes

| Repo | Path | Before SHA-256 | After SHA-256 | Why changed |
| --- | --- | --- | --- | --- |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` | `d79fef3c22e11cadbaad51947bb39b0ac0b81c7fda51a07969baabda950e9dcf` | `2135d356c35909e12fbb3036645628ed0c408cf29705f57e528a3117fc75988e` | Carried WebGlLib batch diagnostics into `WebGlRunRuntimeSnapshot.Diagnostics`. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs` | `de9c242b7fd6bf9cf0fb4efcceac3196b44c1a7effa04150c4aa1de0260e2c2b` | `b22b7219c7396df100cf8c6700971c99e28a4d19a6a1cab652d6676a42e3dd27` | Added failing-first/passing proof that a large frame is one browser batch with batch diagnostics. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor` | `4a9a65e6d96b93e1a26675c5472431b1b99287ccff012a8dba8c1bc4fccc796c` | `536194b8f50bc49e6c57bcc7afc5c218d87f44669153f4d1258fdcabfaf73d6a` | Added deterministic cancel/reset/batch-frame controls, batch metrics, and diagnostics JSON. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` | `5246bd4d77f2acc0a9f5ca33cb3bb9b41eb8746d29cd8cffa47201d90e2054f4` | `eb6e8a23f68db9eeaa19c2230723c3b4fd028460a7d836b568b78aa548348078` | Reworked route to use `WebGlRunDocumentRunner` plus `WebGlRunBrowserApplyAdapter` and a 24-actor generic batch proof frame. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/wwwroot/css/sandbox-village.css` | `a9480c4f8bb26bf29dab34b141fbf70ab18ee98a907fc2a25b3f694c44b65e52` | `5502d269296f75d01c1333aa4598295cf205d10811f6e234318f2331357a6bcf` | Added scoped JSON inspector styling. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/README.md` | `cb5cb215185417d418c4175f3555b751c7af023525860f5f9ccf89b0c70bb60d` | `893b3299046985d039d51122394d90f2bfb1a9e2504a7ddc4bf33834867d8b45` | Documented the browser playback adapter chain and sandbox proof route. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/docs/webgl/run-layer-boundary.md` | `ee2bb7c714a0e55721fbff92099d56c13405378d32636a83dfdd7a85cdd1d31e` | `d87e2feea3cfe2a1296f7780136632fc587f5431daa737ea62d0b51fad92c695` | Documented the allowed runner/adapter path and WebGlLib public API boundary. |

## Commands

| Command | Working directory | Transcript path | Result |
| --- | --- | --- | --- |
| `dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --filter Adapter_reports_large_frame_batch_diagnostics_without_per_object_apply_loop` | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/failing-first-batch-diagnostics.txt` | Failed for the right reason before implementation: `batchCommandCount` was missing from the run snapshot diagnostics. |
| `dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --filter Adapter_reports_large_frame_batch_diagnostics_without_per_object_apply_loop` | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/passing-batch-diagnostics-adapter-test.txt` | Passed. |
| `dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj` | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/passing-webglrunlib-tests.txt` | Passed: 32 tests. |
| `dotnet build src\CanDoItAll.Components.WebGlSandbox\CanDoItAll.Components.WebGlSandbox.csproj` | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/passing-webglsandbox-build.txt` | Passed with 0 warnings and 0 errors. |
| `npm run webglrunlib:audit-boundary` | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/passing-webglrunlib-boundary-audit.txt` | Passed; WebGlRunLib references WebGlLib only and has no forbidden domain terms. |
| `npm run webgllib:audit-boundary` | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/passing-webgllib-boundary-audit.txt` | Passed; WebGlLib remains independent of WebGlRunLib and Economy. |
| `dotnet build CanDoItAll.Components.slnx` | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/components-solution-build.txt` | Passed with 0 warnings and 0 errors. |
| `dotnet test CanDoItAll.Components.slnx --no-build` | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/components-solution-test-no-build.txt` | Passed: WebGlLib 44 tests, WebGlRunLib 32 tests. |
| Source assertion scan | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/sb09-source-assertions.txt` | Passed; runner, adapter, diagnostics, route, docs, and audit script references located. |
| Anti-stub and boundary scan | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/sb09-anti-stub-and-boundary-scan.txt` | Passed; no stub markers and WebGlRunLib boundary audit clean. |
| Filtered `git diff --check` | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/git-diff-check.txt` | Passed; no non-line-ending diff-check findings. |
| Placeholder/template scan | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/sb09-placeholder-scan.txt` | Passed; no SB09 placeholder/template leftovers found. |
| `python scripts\validate_bundle.py --stage execution --profile initiative` | `bundle://` | `bundle://proof/SB09/transcripts/bundle-validate-execution.txt` | Passed; bundle remains execution-valid after SB09 closure updates. |
| Browser proof `/run-playback` batch frame | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/browser-run-playback-batch-proof.json` | Passed; 24-stage/24-motion generic frame applied as `run-frame:4` with `interopCallsAvoided=23`. |
| Browser proof `/run-playback` mobile layout | `repo://CanDoItAll.Components` | `bundle://proof/SB09/transcripts/browser-run-playback-mobile-proof.json` | Passed; controls and diagnostics remained present at 390x844. |

## Source Assertions

| Assertion | Source path | Line/range or search term | Verified by |
| --- | --- | --- | --- |
| Browser adapter exports WebGlLib batch diagnostics into run snapshots. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` | `batchCommandCount`, `interopCallsAvoided` | `bundle://proof/SB09/transcripts/sb09-source-assertions.txt` |
| Run playback route uses `WebGlRunDocumentRunner` and `WebGlRunBrowserApplyAdapter`. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` | `WebGlRunDocumentRunner`, `WebGlRunBrowserApplyAdapter`, `WebGlSceneViewBrowserRuntime` | `bundle://proof/SB09/transcripts/sb09-source-assertions.txt` |
| Generic batch proof frame contains 24 non-domain actors. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` | `BatchActorCount`, `BatchProofFrameIndex` | `bundle://proof/SB09/transcripts/browser-run-playback-batch-proof.json` |
| Route exposes diagnostics JSON for browser proof. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor` | `webgl-run-diagnostics-json` | `bundle://proof/SB09/transcripts/browser-run-playback-batch-proof.json` |
| Boundary docs name the allowed adapter path and WebGlLib independence. | `repo://CanDoItAll.Components/docs/webgl/run-layer-boundary.md` | `WebGlRunDocumentRunner`, `WebGlRunBrowserApplyAdapter`, `WebGlSceneViewBrowserRuntime` | `bundle://proof/SB09/transcripts/sb09-source-assertions.txt` |

## Semantic Adequacy Gate

| Item | Required evidence | Status | Artifact |
| --- | --- | --- | --- |
| Shallow-pass trap named | A weak pass could add route buttons and call `ApplyCommandBatchAsync` directly while never proving the runner/adapter path or large-frame batching diagnostics. | Passed | `bundle://proof/SB09/semantic-invariants.md` |
| Adversarial negative proof | Failing-first adapter test proved the run snapshot previously dropped batch diagnostics. | Passed | `bundle://proof/SB09/transcripts/failing-first-batch-diagnostics.txt` |
| Semantic positive proof | Focused adapter test, full WebGlRunLib tests, browser route proof, and solution tests pass. | Passed | `bundle://proof/SB09/transcripts/passing-batch-diagnostics-adapter-test.txt`, `bundle://proof/SB09/transcripts/browser-run-playback-batch-proof.json`, `bundle://proof/SB09/transcripts/components-solution-test-no-build.txt` |
| Anti-stub audit | Stub/TODO scan clean and package boundary audits clean. | Passed | `bundle://proof/SB09/transcripts/sb09-anti-stub-and-boundary-scan.txt`, `bundle://proof/SB09/transcripts/passing-webgllib-boundary-audit.txt` |
| Raw-note closure | REQ-010, REQ-011, and REQ-015 SB09 slices mapped in traceability and execution report. | Passed | `bundle://traceability/01-requirement-traceability.md`, `bundle://reviews/01-execution-report.md` |
| Downstream smoke | Components solution build and test pass after browser runtime integration. | Passed | `bundle://proof/SB09/transcripts/components-solution-build.txt`, `bundle://proof/SB09/transcripts/components-solution-test-no-build.txt` |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Run browser apply result | `WebGlRunBrowserApplyAdapter` | WebGlSandbox `/run-playback` and future domain bridge hosts | Created for each frame application from a `WebGlRunFrameApplyResult`, WebGlLib command-batch result, diagnostics, and runtime snapshot. | `bundle://proof/SB09/transcripts/failing-first-batch-diagnostics.txt` |
| Run runtime snapshot diagnostics | `WebGlRunBrowserApplyAdapter.BuildDiagnostics` | Browser proof JSON and future QA/performance gates | Copies WebGlLib runtime diagnostics such as batch command count, stage count, normalization counts, and interop calls avoided into a stable run snapshot dictionary. | `bundle://proof/SB09/transcripts/failing-first-batch-diagnostics.txt` |
| Generic run playback route | `WebGlSandbox` `/run-playback` | Browser validation and downstream Economy integration proof | Loads a generic run document, applies frames through runner/adapter, and exposes deterministic controls plus diagnostics JSON. | `bundle://proof/SB09/transcripts/browser-run-playback-batch-proof.json` |

## Browser Validation Analytics

| Route | Viewport | Actions | Screenshot/log artifacts | Result |
| --- | --- | --- | --- | --- |
| `http://localhost:5298/run-playback` | 1440x1000 | Navigated to route, clicked `Batch frame`, captured full-page screenshot and named browser snapshot. | `bundle://proof/SB09/browser/sb09-run-playback-batch-frame.png`, `bundle://proof/SB09/browser/sb09-run-playback-batch-frame-snapshot.md`, `bundle://proof/SB09/browser/sb09-run-playback-console.log`, `bundle://proof/SB09/transcripts/browser-run-playback-batch-proof.json` | Passed; diagnostics showed frame 4, 24 stages, 24 motions, `run-frame:4`, and `interopCallsAvoided=23`. |
| `http://localhost:5298/run-playback` | 390x844 | Resized after batch-frame proof, captured mobile screenshot and deep snapshot. | `bundle://proof/SB09/browser/sb09-run-playback-mobile.png`, `bundle://proof/SB09/browser/sb09-run-playback-mobile-deep-snapshot.md`, `bundle://proof/SB09/transcripts/browser-run-playback-mobile-proof.json` | Passed; controls and diagnostics remained present with stacked layout. |

## Refactor Gate Result

- Touched files reviewed: yes; adapter, route markup/code-behind, CSS, docs, and tests were reread and source-scanned.
- Duplicates removed: route frame application now flows through a single browser adapter path instead of direct page-level command-batch calls.
- Layering checked: WebGlLib remains independent; WebGlRunLib calls only WebGlLib public scene APIs through the adapter/runtime abstraction.
- Fixture-specific code removed: batch proof route uses generic actor/marker vocabulary and no domain terms.
- Docs/tests updated: WebGlRunLib README, boundary doc, adapter tests, browser proof route, and proof transcripts updated.
- Remaining refactor risk: medium-low; SB10 must prove the first Economy bridge maps into these generic contracts without adding domain semantics to Components.
