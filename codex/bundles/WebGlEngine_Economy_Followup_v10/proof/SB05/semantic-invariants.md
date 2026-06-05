# Semantic invariants SB05

## Invariants

- SB05-I01: Pause must stop browser runtime activity before C# playback drain can apply stale callbacks.
- SB05-I02: Post-pause proof must show no active motions, no queued motions, no queued stages, and no idle blockers.
- SB05-I03: Runtime stop generations must suppress stale runtime callbacks.

## Semantic Adequacy Gate

- Shallow-pass trap: toggling `isPlaying=false` while browser motion or command stages continue.
- Adversarial negative proof: `bundle://proof/SB05/components-webglrun-phase-b-test.txt` proves stop-before-cancel ordering and stale callback rejection.
- Semantic positive proof: `bundle://proof/SB07/playwright-runtime-state-assertions.txt` proves the browser route is paused, idle, and has zero active/queued work.
- Anti-stub audit: `bundle://proof/SB05/anti-stub-scan.txt`.
- Source hashes: `bundle://proof/SB05/phase-b-source-hashes.txt`.

## Production Behavior Artifact Matrix

| Invariant | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| SB05-I03 | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/39-webgl-scene-runtime-stop.js` | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunRuntimeStopGenerationPolicy.cs`; `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` | Runtime stop results carry generation metadata; stale completion callbacks are ignored. | `bundle://proof/SB05/components-webglrun-phase-b-test.txt` |
