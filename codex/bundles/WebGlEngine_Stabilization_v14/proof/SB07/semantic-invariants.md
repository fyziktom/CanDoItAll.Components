# SB07 Semantic Invariants: Command batch and stage lifecycle contract

## SB07-RC

Raw note owned: stabilize and freeze the generic Components WebGL engine without adding consuming-app semantics.

Expected behavior: Command batch wait paths pass explicit idle policy and stage lifecycle audits continue to prove queue, barrier, and settled-state behavior.

Disallowed shallow implementation: a change that only updates incomplete proof markers, snapshots, or diagnostics text without enforcing the runtime/source contract.

Failing-first or closed-gap proof: bundle://proof/SB07/transcripts/failing-first-or-closed-gap.txt

Passing proof: bundle://proof/SB13/transcripts/stage-runner-final.txt

Changed source files and hashes: bundle://proof/SB16/changed-file-hashes.txt

Production assertions: repo://src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js

Adversarial negative case: Stage-runner audit includes barrier, cancellation, and scheduler edge cases.

Semantic positive case: Stage-runner and command-batch audits prove command stages settle and diagnostics remain coherent.

Downstream dependency check: final SB16 build, tests, package proof, browser proof, and domain-boundary audits passed.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB07-RC behavior | repo://src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js | bundle://proof/SB13/transcripts/stage-runner-final.txt | bundle://proof/SB07/transcripts/implementation-validation.txt | bundle://proof/SB07/transcripts/failing-first-or-closed-gap.txt |

## Closure

Status: Solved. This invariant is backed by existing transcripts and the final bundle closure report.


