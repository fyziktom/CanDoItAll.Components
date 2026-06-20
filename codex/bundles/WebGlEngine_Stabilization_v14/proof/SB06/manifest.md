# SB06 Proof Manifest: Runtime idle policy modes

Status: Completed
Invariant contract: bundle://proof/SB06/semantic-invariants.md
Changed-file hashes: bundle://proof/SB16/changed-file-hashes.txt

## Owned Requirements

- Components-only implementation scope.
- Runtime idle policy modes.
- Preserve generic WebGlLib/WebGlRunLib boundaries and public API approval discipline.

## Proof Artifacts

| Evidence | Path |
|---|---|
| Failing-first or closed-gap | bundle://proof/SB06/transcripts/failing-first-or-closed-gap.txt |
| Implementation validation | bundle://proof/SB06/transcripts/implementation-validation.txt |
| Domain-boundary audit | bundle://proof/SB06/transcripts/domain-boundary-audit.txt |
| Changed files | bundle://proof/SB06/transcripts/changed-files.txt |
| Source assertions | bundle://proof/SB06/transcripts/source-assertions.txt |
| Anti-stub audit | bundle://proof/SB06/transcripts/anti-stub-audit.txt |
| Primary passing proof | bundle://proof/SB06/transcripts/runtime-idle-policy-final.txt |

## Source-Level Assertions

repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeIdleOptions.cs; repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeIdleResult.cs; repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/41-webgl-scene-runtime-idle-state.js

## Semantic Adequacy Gate

- Invariant id: SB06-RC
- Shallow-pass trap: accepting file existence, approval snapshot churn, or panel counters without proving the runtime/source behavior.
- Adversarial negative proof: Runtime idle policy test proves visualStrict does not silently accept render-loop:scheduled.
- Semantic positive proof: Runtime idle policy Node test proves visualStrict rejects scheduled render blockers while allowFinalRenderDrain can drain final scheduled render.
- Anti-stub audit: bundle://proof/SB16/transcripts/anti-stub-audit-owned-final.txt
- Raw-note closure: Solved for SB06; no scope words were narrowed and no consuming-app semantics were added to Components.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB06-RC behavior | repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeIdleOptions.cs; repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeIdleResult.cs; repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/41-webgl-scene-runtime-idle-state.js | bundle://proof/SB06/transcripts/runtime-idle-policy-final.txt | bundle://proof/SB06/transcripts/implementation-validation.txt | bundle://proof/SB06/transcripts/failing-first-or-closed-gap.txt |

## Closure Decision

Pass. Required proof artifacts are present, source references are portable, domain-boundary gates pass, and final validation is cited from the transcripts above.


