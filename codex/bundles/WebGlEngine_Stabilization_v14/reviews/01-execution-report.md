# WebGlEngine Stabilization v14 Execution Report

## Summary

Status: Completed on 2026-06-05.

The v14 bundle was executed in `CanDoItAll.Components` only. It hardens package scope, freezes public C# and JS surfaces, makes runtime idle policy explicit, tightens generic driver and provenance behavior, extends domain-boundary gates, proves package-mode samples, validates runtime resource and command-stage behavior, and captures real browser proof for `/run-playback`.

## Implementation Highlights

| Area | Result |
|---|---|
| Package scope | Global `IsPackable` now defaults to false; intended package projects opt in explicitly; sandbox and sample projects opt out. |
| Sample package mode | WebGlRunLib generic sample now supports project mode and package mode with a validation target. |
| C# API freeze | WebGlLib and WebGlRunLib approval snapshots updated and passing. |
| JS API freeze | `window.CanDoItAll.webglScene` has an approval JSON manifest with method/result shape coverage. |
| WebGlSceneView facade | Lifecycle and command-result helpers moved to partial code-behind files while preserving the component facade. |
| Runtime idle | Added `semanticOnly`, `visualStrict`, and `allowFinalRenderDrain` modes across C#, JS, and diagnostics. |
| Domain driver | Pass-through driver only accepts approved generic action kinds; unknown action kinds map to `Wait`. |
| Provenance | Opaque provenance and trace-map refs are allowed without generic interpretation. |
| Domain audit | Source, public API, package, docs/tools/workflows, and active bundle artifact profiles pass. |
| Browser proof | `/run-playback` complete generic timeline passes strict visual idle, observer proof, visible canvas pixels, and screenshot capture. |
| Docs | Added `docs/webgl/components-webgl-engine-rc-freeze.md`. |

## Validation Highlights

| Command | Result | Transcript |
|---|---|---|
| `dotnet build CanDoItAll.Components.slnx /p:UseSharedCompilation=false` | Passed, 0 warnings/errors | bundle://proof/SB16/transcripts/dotnet-build-final.txt |
| `dotnet test WebGlLib.Tests` | Passed, 65 tests | bundle://proof/SB16/transcripts/dotnet-test-webgllib-final.txt |
| `dotnet test WebGlRunLib.Tests` | Passed, 83 tests | bundle://proof/SB16/transcripts/dotnet-test-webglrunlib-final.txt |
| `dotnet pack CanDoItAll.Components.slnx` | Passed; package projects emitted; sandbox pack warnings expected | bundle://proof/SB16/transcripts/dotnet-pack-final.txt |
| `npm run webgllib:test-runtime-idle-policy` | Passed | bundle://proof/SB06/transcripts/runtime-idle-policy-final.txt |
| `npm run webgllib:test-resource-ownership` | Passed | bundle://proof/SB12/transcripts/resource-ownership-final.txt |
| `npm run webgllib:audit-command-batch-parity` | Passed | bundle://proof/SB13/transcripts/command-batch-parity-final.txt |
| `npm run webgllib:audit-motion-queue` | Passed | bundle://proof/SB13/transcripts/motion-queue-final.txt |
| `npm run webgllib:audit-stage-runner` | Passed | bundle://proof/SB13/transcripts/stage-runner-final.txt |
| Domain hard/soft audits | Passed | bundle://proof/SB16/transcripts/domain-boundary-generic-source-hard-gate.txt and peers |
| Anti-stub audit | Passed for owned WebGlLib/WebGlRunLib source | bundle://proof/SB16/transcripts/anti-stub-audit-owned-final.txt |

## Browser Validation Analytics

| Route | Viewport | Actions | Assertions | Screenshot | Result |
|---|---:|---|---|---|---|
| `/run-playback` | 1920x1080 | Open route, step frames 1-3, capture snapshot, wait `visualStrict` idle, sample canvas pixels | Route loaded, host found, canvas sized, strict visual idle, diagnostics captured, proof snapshot captured, observer panel captured, visible pixels, no page errors | bundle://proof/SB14/screenshots/run-playback-1920x1080.png | Passed |

Browser runtime report: bundle://proof/SB14/browser-observer-proof.json. The final report has `pass=true`, `strictVisualIdle=true`, and no page errors. The managed Playwright Chromium install failed earlier because of a certificate-chain error, so the browser proof used installed system Chrome at `C:/Program Files/Google/Chrome/Application/chrome.exe`.

## Subbundle Gate Results

| Subbundle | Status | Manifest | Gate result |
|---|---|---|---|
| SB01 | Completed | bundle://proof/SB01/manifest.md | Pass |
| SB02 | Completed | bundle://proof/SB02/manifest.md | Pass |
| SB03 | Completed | bundle://proof/SB03/manifest.md | Pass |
| SB04 | Completed | bundle://proof/SB04/manifest.md | Pass |
| SB05 | Completed | bundle://proof/SB05/manifest.md | Pass |
| SB06 | Completed | bundle://proof/SB06/manifest.md | Pass |
| SB07 | Completed | bundle://proof/SB07/manifest.md | Pass |
| SB08 | Completed | bundle://proof/SB08/manifest.md | Pass; post-SB08 QA gates retained |
| SB09 | Completed | bundle://proof/SB09/manifest.md | Pass |
| SB10 | Completed | bundle://proof/SB10/manifest.md | Pass |
| SB11 | Completed | bundle://proof/SB11/manifest.md | Pass |
| SB12 | Completed | bundle://proof/SB12/manifest.md | Pass |
| SB13 | Completed | bundle://proof/SB13/manifest.md | Pass |
| SB14 | Completed | bundle://proof/SB14/manifest.md | Pass; real browser proof captured |
| SB15 | Completed | bundle://proof/SB15/manifest.md | Pass |
| SB16 | Completed | bundle://proof/SB16/manifest.md | Pass; RC signoff captured |

## Raw Note Closure

| Raw note / requirement | Status | Proof |
|---|---|---|
| Keep work Components-only | Solved | bundle://proof/SB16/transcripts/domain-boundary-generic-source-hard-gate.txt; no changed path targets CanDoItAll.Economy |
| Freeze generic APIs | Solved | API approval snapshots and tests in SB03/SB04/SB16 |
| Prove package scope and samples | Solved | SB02 tests and SB11 package-mode transcripts |
| Make runtime idle strictness explicit | Solved | SB06 policy tests and SB14 strict browser proof |
| Preserve generic driver boundary | Solved | SB08/SB09 tests and SB10 domain hard gates |
| Prove browser observer flow | Solved | SB14 JSON report and screenshot |
| Produce final RC signoff | Solved | bundle://proof/SB16/components-rc-freeze-manifest.md |

## Residual Risks

No required v14 item remains open. Future generic-engine changes should treat the frozen approval files as compatibility gates and rerun the release checks listed in repo://docs/webgl/components-webgl-engine-rc-freeze.md.
