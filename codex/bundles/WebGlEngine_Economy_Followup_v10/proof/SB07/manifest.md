# Proof manifest SB07

Status: completed

Required proof: Observer proof fails on deliberately mismatched document/hash; passes on actual runtime export.

## Artifacts

- WebGlRunLib test transcript: `bundle://proof/SB05/components-webglrun-phase-b-test.txt`
- Browser route snapshot: `bundle://proof/SB07/playwright-run-playback-snapshot.txt`
- Playwright diagnostics JSON: `bundle://proof/SB07/playwright-diagnostics-json.txt`
- Playwright runtime assertions: `bundle://proof/SB07/playwright-runtime-state-assertions.txt`
- Playwright screenshot: `bundle://proof/SB07/run-playback-phase-b.png`
- No-fallback source scan: `bundle://proof/SB07/source-scan-no-browser-position-fallback.txt`
- Source scan: `bundle://proof/SB07/phase-b-source-scan.txt`
- Source hashes: `bundle://proof/SB07/phase-b-source-hashes.txt`
- Anti-stub audit: `bundle://proof/SB07/anti-stub-scan.txt`

## Source Assertions

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` now supplies observer positions only from `latestSnapshot.ObjectPositions`; it no longer falls back to expected positions.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js` exports actual browser runtime object positions.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunObserverProof.cs` compares expected document hash, browser-loaded document hash, runtime idle, completed stages, and browser-exported final positions.
- `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs` proves missing browser-exported positions fail even when document hashes, idle, and completed stages pass.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Browser object positions | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js` | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs`; `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunObserverProof.cs` | Browser exports object positions; RunPlayback passes them into observer proof; observer proof compares them against expected final positions. | `bundle://proof/SB07/source-scan-no-browser-position-fallback.txt`; `bundle://proof/SB05/components-webglrun-phase-b-test.txt` |
| `browserObjectPositionsCaptured` metadata | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` | RunPlayback diagnostics JSON and Playwright assertion | Metadata marks whether positions came from browser proof snapshot, enabling proof consumers to reject fallback-free missing evidence. | `bundle://proof/SB07/playwright-runtime-state-assertions.txt` requires the value to be `True`. |

## Browser Validation Analytics

| Route | Viewport | Actions | Assertions | Result |
|---|---|---|---|---|
| `/run-playback` | 1920x1080 requested; browser-reported proof viewport `1209x1146` | Reset, Play, short wait, Pause, Snapshot | observer valid, hashes match, browser object positions captured, active/queued motions zero, queued stages zero, idle blockers empty | Pass |

## Gate Result

Pass. Browser proof uses exported runtime state and the no-fallback negative test/source scan prevents expected-position self-comparison from passing.
