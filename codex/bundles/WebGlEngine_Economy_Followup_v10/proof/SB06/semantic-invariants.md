# Semantic invariants SB06

## Invariants

- SB06-I01: `applyCommandBatchAndWait` must distinguish scheduled, settled, cancelled, and failed states.
- SB06-I02: Runtime idle blockers must be surfaced to C# snapshots and not hidden behind `Success=true`.
- SB06-I03: Multi-frame playback must stop on the first failed or cancelled frame.

## Semantic Adequacy Gate

- Shallow-pass trap: reporting command success immediately after scheduling stages or motions.
- Adversarial negative proof: `bundle://proof/SB05/components-webglrun-phase-b-test.txt` includes idle-timeout, scheduled-not-settled, and cancellation tests.
- Semantic positive proof: `bundle://proof/SB06/components-webgllib-phase-b-test.txt` plus `bundle://proof/SB07/playwright-runtime-state-assertions.txt`.
- Anti-stub audit: `bundle://proof/SB06/anti-stub-scan.txt`.
- Source hashes: `bundle://proof/SB06/phase-b-source-hashes.txt`.

## Production Behavior Artifact Matrix

| Invariant | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| SB06-I01 | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/26-webgl-scene-command-batch.js` | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` | JS command result carries lifecycle state; C# snapshot records lifecycle and settled flags. | `bundle://proof/SB05/components-webglrun-phase-b-test.txt` |
