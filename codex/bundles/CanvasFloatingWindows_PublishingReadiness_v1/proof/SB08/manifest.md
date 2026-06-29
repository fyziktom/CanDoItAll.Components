# SB08 Proof Manifest

## Status

- Result: Passed.
- Date: 2026-06-29.
- Scope: Canvas, Canvas benchmark, and Overlays sandbox route matrix without WebGL coverage.

## Source Changes

- `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/CanvasBenchmark.razor`
- `repo://src/CanDoItAll.Components.Sandbox/CanvasBenchmarkSamples.cs`
- `repo://src/CanDoItAll.Components.Sandbox/wwwroot/js/canvasBenchmarkPage.js`
- `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB08/verify-sandbox-matrix.cjs`

## Command Proof

- Build: `bundle://proof/SB08/transcripts/dotnet-build-sandbox.txt`
- Verifier syntax: `bundle://proof/SB08/transcripts/node-check-sandbox-matrix.txt`
- Benchmark ES module syntax: `bundle://proof/SB08/transcripts/node-check-canvas-benchmark-page.txt`
- Browser matrix: `bundle://proof/SB08/transcripts/playwright-sandbox-matrix.txt`
- Console log: `bundle://proof/SB08/console-log.txt`
- Matrix JSON: `bundle://proof/SB08/matrix-results.json`
- Source assertions: `bundle://proof/SB08/transcripts/source-assertions-sandbox-matrix.txt`
- Anti-stub audit: `bundle://proof/SB08/transcripts/anti-stub-audit.txt`
- Screenshot inventory: `bundle://proof/SB08/transcripts/screenshot-inventory.txt`
- Changed-file hashes: `bundle://proof/SB08/transcripts/changed-file-hashes.txt`
- Semantic adequacy: `bundle://proof/SB08/transcripts/semantic-adequacy.txt`
- Representative SHA-256: `7247EA3BEF7A3CF55DA7D75AF8BAC07C108DB6C06224415018358A0BDDFB50A6`
- Failing-first: N/A process proof exemption; SB08 reopened defects during matrix execution and records the repaired verifier/browser proof rather than a separate failing command transcript.
- Passing transcript: `bundle://proof/SB08/transcripts/playwright-sandbox-matrix.txt`

## Browser Evidence

- Routes: `route groups/canvas`, `route groups/canvas/benchmark`, `route groups/overlays`.
- Viewports: 1920x1080, 1366x900, 1024x768, 390x844.
- Actions: 20 recorded matrix actions.
- Screenshots: 20 files listed in `bundle://proof/SB08/transcripts/screenshot-inventory.txt`.
- Console result: 0 warnings, 0 errors, 0 page errors.

## Reopened And Resolved During SB08

- Benchmark preview: fixed collapsed/misleading shipped workbench preview and stale DOM-SVG language before accepting proof.
- Overlay lifecycle verifier: added bounded-geometry wait after show/remount so proof asserts the real safe-top/container invariant.

## Gate Decision

- SB09 may proceed.
- SB10 may use SB08 as completed browser analytics evidence.

