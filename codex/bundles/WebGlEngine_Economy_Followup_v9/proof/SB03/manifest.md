# Proof manifest - SB03

Status: completed

## Scope

SB03 separates semantic runtime idle from visual/render-loop idle. Semantic blockers now cover motion, queued motion, command-stage work, barriers, automatic stage work, and pending asset disposal. Visual blockers cover active frames, scheduled render frames, and continuous render mode. A final scheduled render is accepted only after an explicit final-render drain or two consecutive semantic-idle probes.

## Changed files

Changed-file hashes:

- `bundle://proof/SB03/transcripts/changed-file-hashes.txt`

Production files:

- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/41-webgl-scene-runtime-idle-state.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/22-webgl-scene-scheduler.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/25-webgl-scene-diagnostics.js`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeIdleResult.cs`
- `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs`

Test/proof files:

- `repo://tests/CanDoItAll.Components.WebGlLib.Tests/WebGlRuntimeDiagnosticsTests.cs`
- `bundle://proof/SB03/js/runtime-idle-final-render-test.mjs`
- `bundle://proof/SB03/browser/runtime-idle-semantics-proof.mjs`
- `bundle://proof/SB03/semantic-invariants.md`

## Proof artifacts

- JS semantic/final-render proof transcript: `bundle://proof/SB03/transcripts/runtime-idle-final-render-js.txt`
- JS semantic/final-render assertions: `bundle://proof/SB03/js/runtime-idle-final-render-assertions.json`
- WebGlLib regression proof: `bundle://proof/SB03/transcripts/webgllib-tests.txt`
- WebGlRunLib regression proof: `bundle://proof/SB03/transcripts/webglrunlib-tests.txt`
- Sandbox build proof: `bundle://proof/SB03/transcripts/webglsandbox-build.txt`
- Browser proof transcript: `bundle://proof/SB03/transcripts/runtime-idle-semantics-playwright.txt`
- Browser assertions/diagnostics JSON: `bundle://proof/SB03/browser/runtime-idle-semantics-assertions.json`
- Browser screenshot: `bundle://proof/SB03/browser/runtime-idle-semantics-after.png`
- Browser console log: `bundle://proof/SB03/browser/runtime-idle-semantics-console.log`
- Source assertions: `bundle://proof/SB03/transcripts/source-assertions.txt`
- Anti-stub scan: `bundle://proof/SB03/transcripts/anti-stub-scan.txt`
- Bundle validator transcript: `bundle://proof/SB03/transcripts/bundle-validator.txt`
- Sandbox server logs: `bundle://proof/SB03/transcripts/webgl-sandbox-sb03.out.txt`, `bundle://proof/SB03/transcripts/webgl-sandbox-sb03.err.txt`

## Semantic adequacy gate

- Shallow-pass trap: treating `render-loop:scheduled` as equivalent to active semantic work causes false idle timeouts during a final paint and adds browser-observer noise.
- Negative proof: `runtime-idle-final-render-test.mjs` proves active motion remains a semantic blocker and continuous render mode remains a visual blocker, even when final-render drain logic is available.
- Positive proof: the same JS proof shows a scheduled-only final render is not drained on the first semantic-idle probe, is drained on the second consecutive probe, and returns no aggregate blockers.
- Browser proof: `runtime-idle-semantics-proof.mjs` loads `/run-playback`, imports a generic scene, runs a staged batch through `applyCommandBatchAndWait`, applies a final visual patch, waits for runtime idle, and asserts semantic idle, visual idle, finalRenderDrained, and zero active/queued motion/stage blockers.
- Raw-note literal closure: SB03 reduces simulator noise by preventing one final scheduled browser render from being mistaken for unfinished semantic simulation work.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative / proof citation |
| --- | --- | --- | --- | --- |
| Semantic and visual idle split | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/41-webgl-scene-runtime-idle-state.js` | `waitForRuntimeIdle`, `getDiagnostics`, browser/Blazor idle contracts | Semantic blockers and visual blockers are collected separately, then rejoined only for legacy aggregate blockers. | `bundle://proof/SB03/js/runtime-idle-final-render-assertions.json` |
| Two-probe final render drain | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js` | Runtime idle waiters and command-batch settle waits | `waitForRuntimeIdle` tracks consecutive semantic-idle probes and marks a scheduled-only final render drained on the second probe. | `bundle://proof/SB03/transcripts/source-assertions.txt`, `bundle://proof/SB03/js/runtime-idle-final-render-assertions.json` |
| Runtime idle diagnostics fields | JS diagnostics and C# interop contracts | WebGl diagnostics API, RunPlayback diagnostics JSON, tests | `semanticIdle`, `visualIdle`, and `finalRenderDrained` flow through runtime diagnostics and `WebGlRuntimeIdleResult`. | `bundle://proof/SB03/transcripts/webgllib-tests.txt`, `bundle://proof/SB03/browser/runtime-idle-semantics-assertions.json` |
| Browser active/queued idle proof | Browser proof script | Bundle closure | Final browser diagnostics assert zero active motions, queued motions, queued command stages, and barriers after idle wait. | `bundle://proof/SB03/browser/runtime-idle-semantics-assertions.json`, `bundle://proof/SB03/browser/runtime-idle-semantics-after.png` |

## Closure

SB03 passes. JS proof confirms the two-probe final scheduled render rule and negative semantic/visual blockers. Browser proof route: `http://localhost:5298/run-playback`, viewport `1920x1080`, generic scene import plus staged batch plus final render drain, no disallowed console errors, final blockers `[]`, and `semanticIdle`, `visualIdle`, `finalRenderDrained` all `true`.
