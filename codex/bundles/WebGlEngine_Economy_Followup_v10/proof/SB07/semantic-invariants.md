# Semantic invariants SB07

## Invariants

- SB07-I01: Browser observer proof must use browser-exported document/hash and object positions.
- SB07-I02: Missing browser-exported positions must fail even when idle and stage completion look valid.
- SB07-I03: Observer proof remains observer evidence only and must not mutate source truth.

## Semantic Adequacy Gate

- Shallow-pass trap: filling browser positions from expected run-document positions.
- Adversarial negative proof: `bundle://proof/SB07/source-scan-no-browser-position-fallback.txt` and `bundle://proof/SB05/components-webglrun-phase-b-test.txt`.
- Semantic positive proof: `bundle://proof/SB07/playwright-runtime-state-assertions.txt` shows `observer-valid`, matching document hashes, two browser final object positions, runtime idle, and zero active/queued work.
- Anti-stub audit: `bundle://proof/SB07/anti-stub-scan.txt`.
- Source hashes: `bundle://proof/SB07/phase-b-source-hashes.txt`.

## Production Behavior Artifact Matrix

| Invariant | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| SB07-I01 | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js` | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs`; `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunObserverProof.cs` | Browser proof snapshot exports `objectPositions`; C# observer proof validates them against expected final positions. | `bundle://proof/SB07/source-scan-no-browser-position-fallback.txt` |
| SB07-I02 | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` | RunPlayback diagnostics and observer proof | `browserObjectPositionsCaptured` is true only when browser snapshot positions exist. | `bundle://proof/SB07/playwright-runtime-state-assertions.txt` |
