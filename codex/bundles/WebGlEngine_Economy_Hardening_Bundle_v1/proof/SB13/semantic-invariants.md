# SB13 Semantic Invariants

Subbundle: `SB13`

## Invariants

| Invariant ID | Requirement | Expected behavior | Disallowed shallow implementation | Negative proof | Positive proof |
| --- | --- | --- | --- | --- | --- |
| SB13-INV-001 | Browser proof must exercise real WebGlLib scene behavior, not only static markup. | Routes expose a live scene host/canvas, nonblank pixel probe, runtime diagnostics, command callbacks, screenshots and console logs. | Claiming success from prerendered HTML, a screenshot-only check, or a canvas existing while diagnostics stay empty. | Bad patch and missing asset paths in `browser-tycoon-stress-proof.json`; page health checks in `browser-economy-simulation-sandbox-proof.json`. | `browser-tycoon-stress-proof.json`, `browser-run-playback-proof.json`, `browser-performance-proof.json`, `browser-economy-simulation-sandbox-proof.json`. |
| SB13-INV-002 | Large command batches must not break Blazor Server callbacks. | JS may return rich direct interop results, but `OnCommandCompleted` / `OnCommandFailed` event callbacks send a bounded result shape with total-count metadata. | Returning the full 202-command child-result payload to Blazor and leaving the UI stale or circuit-disconnected while the JS runtime appears successful. | `/performance-proof` red-team run exposed the oversized callback path; bad patch still reports errors through the compact shape. | Patched `/performance-proof` rerun reports `Applied 202 commands across 1 stage(s)` with `runtimeErrorCount=0` and no Blazor circuit failure in `browser-performance-proof.json`. |
| SB13-INV-003 | Asset fallback and ownership proof must be diagnostic, not silent. | Missing assets increment missing counters/ids, GLB profile switches update cache counters, and dispose/recreate releases state without losing later renderability. | Swallowing missing assets, leaking cached templates, or treating any fallback as success without counters. | Intentional missing asset id is recorded in `browser-tycoon-stress-proof.json`; resource ownership script validates shared texture retention/disposal. | `webgllib-test-resource-ownership.txt`, `browser-tycoon-stress-proof.json`. |
| SB13-INV-004 | Generic run playback must remain generic and staged. | WebGlRunLib applies staged generic frames through `WebGlSceneView` command batches with queue/barrier diagnostics and no Economy terms in Components. | Encoding Economy/simulation semantics inside Components or flattening stages into an unobservable patch burst. | Boundary audits pass and command-stage diagnostics expose queued/completed stage state. | `browser-run-playback-proof.json`, `webgllib-audit-boundary.txt`, `webglrunlib-audit-boundary.txt`. |
| SB13-INV-005 | Economy browser proof must be a consumer smoke, not a Components-layer leak. | Economy Node hosts `/economy/simulation-sandbox`; Economy Components uses `WebGlRunBrowserApplyAdapter` over `WebGlSceneView`; Components remains domain-neutral. | Moving Economy visual mapping, ledger, market or scenario concepts into WebGlLib/WebGlRunLib to make the route pass. | Route/source scan and bridge dependency test prove the route remains in Economy and the bridge remains the consumer. | `economy-simulation-sandbox-route-scan.txt`, `browser-economy-simulation-sandbox-proof.json`, `economy-focused-sb13-tests.txt`. |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Compact command-result callback payload | `20-webgl-scene-command-results.js` | Blazor event callbacks | Produced only when notifying .NET event handlers; bounded arrays include total and returned counts. | Oversized full callback payload red-team failure was fixed; bad patch still emits a failure result. |
| Browser diagnostics JSON | WebGlLib runtime proof APIs | SB13/SB14 QA | Captured per route after actions; records runtime counters, missing assets, rebuilds, frame timing and command-stage state. | Missing GLB and bad patch prove diagnostics distinguish failures from success. |
| Economy simulation sandbox proof | Economy Node + Economy Components | Cross-repo QA | Live route loads shared-well, applies frames, captures snapshot and analysis. | Focused bridge test and source scan guard against moving consumer behavior into Components. |

## Reopen Triggers

- A browser proof route reports a nonblank screenshot but runtime diagnostics are empty or unavailable.
- A large command batch reintroduces Blazor callback disconnects, stale UI status, or unbounded event payloads.
- Missing asset/fallback behavior stops recording `missingAssetCount` / `missingAssetIds`.
- Components gains Economy, ledger, market, production-line or scenario references.
- Economy sandbox no longer applies frames through `WebGlRunBrowserApplyAdapter` and `WebGlSceneView`.
