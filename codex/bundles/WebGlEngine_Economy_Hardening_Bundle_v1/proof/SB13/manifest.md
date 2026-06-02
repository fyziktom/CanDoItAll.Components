# Proof Manifest

Subbundle: `SB13`
Status: `Completed`
Prepared by: Codex
Completed at UTC: `2026-06-02T07:21:00Z`

## Changed File Hashes

| Repo | Path | Before SHA-256 | After SHA-256 | Why changed |
| --- | --- | --- | --- | --- |
| CanDoItAll.Components | `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/20-webgl-scene-command-results.js` | `worktree already dirty from prior SB03-SB06 runtime changes; SB13 pre-edit hash not separately captured` | `b9aaf1086cf4cad7a1be186c28be3519ab7a5b565d753e93ae9b8aa96aa58411` | SB13 red-team browser proof exposed that a 202-command batch could complete in JS while the full Blazor `OnCommandCompleted` callback payload was too large for the circuit. The event callback now sends a compact `WebGlSceneCommandResult` shape with bounded arrays and total-count metadata; direct JS interop results remain rich. |
| CanDoItAll.Components | `codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/proof/SB13/manifest.md` | template | after-close hash recorded in `proof/SB13/transcripts/sb13-file-hashes.txt` | Completed SB13 proof manifest. |
| CanDoItAll.Components | `codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/proof/SB13/semantic-invariants.md` | template | after-close hash recorded in `proof/SB13/transcripts/sb13-file-hashes.txt` | Recorded SB13 invariants. |
| CanDoItAll.Components | `codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/proof/SB13/refactor-gate.md` | new-file | after-close hash recorded in `proof/SB13/transcripts/sb13-file-hashes.txt` | Recorded mandatory SB13 refactor gate. |
| CanDoItAll.Components | `codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/README.md` | previous execution-progress state | after-close hash recorded in `proof/SB13/transcripts/sb13-file-hashes.txt` | Marked SB13 completed and SB14 next eligible. |
| CanDoItAll.Components | `codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/reviews/01-execution-report.md` | previous execution report through SB12 | after-close hash recorded in `proof/SB13/transcripts/sb13-file-hashes.txt` | Added SB13 gate, browser analytics and closure notes. |
| CanDoItAll.Components | `codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/traceability/01-requirement-traceability.md` | previous traceability through SB12 | after-close hash recorded in `proof/SB13/transcripts/sb13-file-hashes.txt` | Added SB13 REQ-015 evidence. |
| CanDoItAll.Components | `codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/subbundles/SB13-browser-performance-memory-proof/README.md` | prepared/not-started checklist | after-close hash recorded in `proof/SB13/transcripts/sb13-file-hashes.txt` | Marked SB13 acceptance checklist complete. |

Hash transcript: `proof/SB13/transcripts/sb13-file-hashes.txt`.

## Commands

| Command | Working directory | Transcript path | Result |
| --- | --- | --- | --- |
| Start WebGlSandbox on `http://127.0.0.1:5313` | `C:\repositories\CanDoItAll.Components` | `proof/SB13/transcripts/webglsandbox-server.json`, `webglsandbox-server.out.txt`, `webglsandbox-server.err.txt` | Passed; server served `/tycoon-village`, `/run-playback`, and `/performance-proof`. |
| Browser proof for `/tycoon-village` | `C:\repositories\CanDoItAll.Components` | `proof/SB13/transcripts/browser-tycoon-stress-proof.json` | Passed; primitive stress, GLB profile switching, repeated import/export, missing asset, bad patch, dispose/recreate all exercised. |
| Browser proof for `/run-playback` | `C:\repositories\CanDoItAll.Components` | `proof/SB13/transcripts/browser-run-playback-proof.json` | Passed; generic batch frame applied with 24 stages and 24 commands. |
| Browser proof for `/performance-proof` | `C:\repositories\CanDoItAll.Components` | `proof/SB13/transcripts/browser-performance-proof.json` | Passed after callback compaction fix; UI reports 202 commands, 100 coalesced patches, 100 dropped duplicate motions, and no runtime errors. |
| `dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj` | `C:\repositories\CanDoItAll.Components` | `proof/SB13/transcripts/components-webgllib-tests.txt` | Passed; 44 tests. |
| `dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj` | `C:\repositories\CanDoItAll.Components` | `proof/SB13/transcripts/components-webglrunlib-tests.txt` | Passed; 32 tests. |
| Resource ownership JS proof | `C:\repositories\CanDoItAll.Components` | `proof/SB13/transcripts/webgllib-test-resource-ownership.txt` | Passed; shared textures are retained, duplicate resource disposal is deduped, cache template disposal is counted. |
| `npm run webgllib:audit-command-batch-parity` | `C:\repositories\CanDoItAll.Components` | `proof/SB13/transcripts/webgllib-audit-command-batch-parity.txt` | Passed; 5 command-batch fixtures. |
| `npm run webgllib:audit-boundary` | `C:\repositories\CanDoItAll.Components` | `proof/SB13/transcripts/webgllib-audit-boundary.txt` | Passed. |
| `npm run webglrunlib:audit-boundary` | `C:\repositories\CanDoItAll.Components` | `proof/SB13/transcripts/webglrunlib-audit-boundary.txt` | Passed. |
| Start Economy Node on `http://127.0.0.1:5413` | `C:\repositories\CanDoItAll.Economy` | `proof/SB13/transcripts/economy-node-server.json`, `economy-node-server.out.txt`, `economy-node-server.err.txt` | Passed; `/economy/simulation-sandbox` returned and hydrated. |
| Browser proof for `/economy/simulation-sandbox` | `C:\repositories\CanDoItAll.Components` | `proof/SB13/transcripts/browser-economy-simulation-sandbox-proof.json` | Passed; frame 1 applied through `WebGlRunBrowserApplyAdapter` with 3 stages, 2 motions, zero adapter errors/warnings, snapshot and analysis captured. |
| `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter FullyQualifiedName~EconomyPerformanceProbeTests|FullyQualifiedName~EconomyReadinessProbeTests|FullyQualifiedName~EconomyWebGlBridgeTests.SimulationSandboxHostsJoinedProjectionWithoutMovingBridgeToComponents` | `C:\repositories\CanDoItAll.Economy` | `proof/SB13/transcripts/economy-focused-sb13-tests.txt` | Passed; 3 tests. Existing `ncalc` restore compatibility warnings preserved. |
| Source assertion scan | Both repos | `proof/SB13/transcripts/sb13-source-assertions.txt`, `economy-simulation-sandbox-route-scan.txt` | Passed. |
| Anti-stub scan | Both repos | `proof/SB13/transcripts/sb13-anti-stub-and-boundary-scan.txt` | Passed; no first-party matches. |
| `git diff --check` | `C:\repositories\CanDoItAll.Components` | `proof/SB13/transcripts/components-git-diff-check.txt` | Passed after removing SB13 manifest trailing whitespace. |
| `git diff --check` | `C:\repositories\CanDoItAll.Economy` | `proof/SB13/transcripts/economy-git-diff-check.txt` | Passed; line-ending warnings only, no whitespace errors. |
| `python scripts\validate_bundle.py --stage execution --profile initiative` | `C:\repositories\CanDoItAll.Components\codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1` | `proof/SB13/transcripts/bundle-validate-execution.txt` | Passed: `Bundle validation passed for stage=execution, profile=initiative, subbundles=14`. |

## Source Assertions

| Assertion | Source path | Line/range or search term | Verified by |
| --- | --- | --- | --- |
| Runtime callback payload is compacted before invoking Blazor event handlers. | `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/20-webgl-scene-command-results.js` | `compactCommandResultForCallback`, `invokeMethodAsync` | `proof/SB13/transcripts/sb13-source-assertions.txt`, `/performance-proof` browser rerun |
| Runtime diagnostics include performance/resource counters required by SB13 proof. | `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/20-webgl-scene-command-results.js`, runtime diagnostics APIs | `assetCacheMode`, `fullSceneRebuildCount`, `linkGeometryUpdateCount` | `proof/SB13/transcripts/browser-performance-proof.json`, `browser-tycoon-stress-proof.json` |
| Economy host exposes a real browser route for the simulation sandbox. | `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Node\Components\Pages\SimulationSandbox.razor` | `@page "/economy/simulation-sandbox"` | `proof/SB13/transcripts/economy-simulation-sandbox-route-scan.txt` |
| Economy sandbox uses `WebGlSceneView` and applies projected frames through the generic browser adapter. | `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Components\Components\SimulationSandbox\EconomySimulationSandboxPage.razor` | `WebGlSceneView`, `WebGlRunBrowserApplyAdapter`, `ApplyCurrentFrameCoreAsync` | `proof/SB13/transcripts/sb13-source-assertions.txt`, `browser-economy-simulation-sandbox-proof.json` |

## Semantic Adequacy Gate

| Item | Required evidence | Status | Artifact |
| --- | --- | --- | --- |
| Shallow-pass trap named | A proof could pass by rendering only the initial scene, ignoring command-stage queues, swallowing missing assets, or returning a huge JS result while the Blazor callback silently fails. | Passed | `proof/SB13/semantic-invariants.md` |
| Adversarial negative proof | Bad patch fails for missing object/wrong scene, intentional missing GLB is diagnosed, and the red-team batch exposed the oversized callback path before compaction. | Passed | `proof/SB13/transcripts/browser-tycoon-stress-proof.json`, `proof/SB13/transcripts/browser-performance-proof.json` |
| Semantic positive proof | Generic scene, generic run playback, performance command batch, and Economy simulation sandbox all render/apply through the browser runtime with diagnostics and screenshots. | Passed | Browser proof JSON and screenshots under `proof/SB13/` |
| Anti-stub audit | First-party production runtime/bridge/sandbox surfaces contain no TODO/stub/NotImplemented placeholders; boundary audits pass. | Passed | `proof/SB13/transcripts/sb13-anti-stub-and-boundary-scan.txt`, `webgllib-audit-boundary.txt`, `webglrunlib-audit-boundary.txt` |
| Raw-note closure | SB13 closes final browser/performance/memory proof for Components and Economy; final consolidated QA remains SB14. | Passed | `reviews/01-execution-report.md`, `traceability/01-requirement-traceability.md` |
| Downstream smoke | Focused Components tests, WebGlRunLib tests, resource audit, command-batch audit, and Economy probe/bridge tests pass after the callback hardening fix. | Passed | Test/audit transcripts listed above |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Compact command-result callback payload | WebGlLib JS runtime `notifyCommandResult` | Blazor `CommandCompleted` / `CommandFailed` callbacks | Created per command result only for event callbacks; bounded arrays include total-count metadata. Direct JS interop returns still expose the full result. | Red-team `/performance-proof` batch exercises 202 commands without Blazor circuit failure after compaction; bad patch still reports failure details. |
| Runtime diagnostics JSON | WebGlLib browser runtime | Proof tooling, WebGlSceneView diagnostics, Economy sandbox | Captured live from route state; includes rebuild, cache, missing asset, frame timing, command batch, queue and motion counters. | Missing GLB path increments `missingAssetCount`; bad patch fails rather than mutating the scene. |
| Economy simulation sandbox browser proof | Economy Node route and Economy Components page | SB13/SB14 QA, Economy users | Loads `shared-well`, applies frames through `WebGlRunBrowserApplyAdapter`, captures snapshot and analysis. | Route scan and focused bridge test prove the route remains a host/consumer and does not move Economy semantics into Components. |

## Browser Validation Analytics

| Route | Viewport | Actions | Screenshot/log artifacts | Result |
| --- | --- | --- | --- | --- |
| `/tycoon-village` on WebGlSandbox | 1440x980 | Primitive stress, runner motion, 5 import/export iterations, mixed/high GLB profile switching, intentional missing asset, bad patch, dispose/recreate. | `proof/SB13/browser/sb13-tycoon-*.png`, `proof/SB13/transcripts/browser-tycoon-stress-proof.json` | Passed; expected GLTF extension warnings only, missing asset diagnosed, bad patch failed, dispose removed state/canvas and recreate restored 20 objects. |
| `/run-playback` on WebGlSandbox | 1440x980 | Loaded route and applied batch frame. | `proof/SB13/browser/sb13-run-playback-batch-frame.png`, `proof/SB13/transcripts/browser-run-playback-proof.json` | Passed; frame 4, 24 stages/commands, `interopCallsAvoided=23`, no console errors. |
| `/performance-proof` on WebGlSandbox | 1440x980 | Ran 202-command batch and captured runtime diagnostics. | `proof/SB13/browser/sb13-performance-proof-command-batch.png`, `proof/SB13/transcripts/browser-performance-proof.json` | Passed after callback compaction; UI reports 202 commands, 100 coalesced patches, 100 dropped duplicate motions, 1 stage, 100 objects, no runtime errors. |
| `/economy/simulation-sandbox` on Economy Node | 1440x980 | Loaded fixture, applied frame 0, stepped/applied frame 1, captured snapshot, ran analysis. | `proof/SB13/browser/sb13-economy-simulation-sandbox-applied-frame.png`, `proof/SB13/transcripts/browser-economy-simulation-sandbox-proof.json`, `browser-economy-console.log` | Passed; step 1, 3 stages, 13 objects, browser apply `applied`, adapter errors/warnings 0, center 16x16 pixel probe nonblank. |

## Visual QA Review

- `sb13-performance-proof-command-batch.png`: nonblank WebGL scene, status and batch/runtime counters visible, no overlap in the proof panel.
- `sb13-run-playback-batch-frame.png`: batch proof is visible and readable; the existing large-screen layout remains functional.
- `sb13-economy-simulation-sandbox-applied-frame.png`: scene is nonblank, controls fit, diagnostics/runtime panels are readable, and snapshot analysis is visible.
- `sb13-tycoon-high-glb-missing-asset.png` and related tycoon screenshots: GLB profile and missing-asset paths render/fallback as expected without blank canvas.

## Refactor Gate Result

- Touched files reviewed: `20-webgl-scene-command-results.js`, SB13 proof/docs/report files.
- Duplicates removed: no new duplicated result serializer; callback compaction reuses existing `limitWithOverflow` behavior and records totals in metadata.
- Layering checked: Components remains generic; Economy route consumes WebGlLib/WebGlRunLib through existing bridge/adapter. No Economy concepts were added to Components.
- Fixture-specific code removed: none introduced; route proof uses existing `shared-well` sandbox fixture only as a consumer smoke.
- Docs/tests updated: manifest, semantic invariants, refactor gate, execution report, traceability, subbundle README, root bundle progress.
- Remaining refactor risk: callback event consumers now receive bounded affected-id arrays. Full command results remain available from direct JS interop, and the event payload carries total/returned counts for consumers that need to detect truncation.
