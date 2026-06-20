# SB06 Semantic Invariants: Runtime idle policy modes

## SB06-RC

Raw note owned: stabilize and freeze the generic Components WebGL engine without adding consuming-app semantics.

Expected behavior: C# and JavaScript runtime idle contracts now expose semanticOnly, visualStrict, and allowFinalRenderDrain policy modes with diagnostics.

Disallowed shallow implementation: a change that only updates incomplete proof markers, snapshots, or diagnostics text without enforcing the runtime/source contract.

Failing-first or closed-gap proof: bundle://proof/SB06/transcripts/failing-first-or-closed-gap.txt

Passing proof: bundle://proof/SB06/transcripts/runtime-idle-policy-final.txt

Changed source files and hashes: bundle://proof/SB16/changed-file-hashes.txt

Production assertions: repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeIdleOptions.cs; repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeIdleResult.cs; repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/41-webgl-scene-runtime-idle-state.js

Adversarial negative case: Runtime idle policy test proves visualStrict does not silently accept render-loop:scheduled.

Semantic positive case: Runtime idle policy Node test proves visualStrict rejects scheduled render blockers while allowFinalRenderDrain can drain final scheduled render.

Downstream dependency check: final SB16 build, tests, package proof, browser proof, and domain-boundary audits passed.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB06-RC behavior | repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeIdleOptions.cs; repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeIdleResult.cs; repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/41-webgl-scene-runtime-idle-state.js | bundle://proof/SB06/transcripts/runtime-idle-policy-final.txt | bundle://proof/SB06/transcripts/implementation-validation.txt | bundle://proof/SB06/transcripts/failing-first-or-closed-gap.txt |

## Closure

Status: Solved. This invariant is backed by existing transcripts and the final bundle closure report.


