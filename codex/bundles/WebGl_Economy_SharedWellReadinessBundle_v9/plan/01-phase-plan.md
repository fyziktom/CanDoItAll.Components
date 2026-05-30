# Phase Plan

## Dependency Map

```mermaid
flowchart TD
    SB01["SB01 inventory and branch guard"] --> SB02["SB02 ordered action stages"]
    SB02 --> SB03["SB03 batch parity and performance"]
    SB03 --> SB04["SB04 link index bottleneck"]
    SB02 --> SB05["SB05 target anchor resolver"]
    SB05 --> SB06["SB06 visual state catalog"]
    SB01 --> SB07["SB07 definition normalizer"]
    SB07 --> SB08["SB08 event normalizer"]
    SB08 --> SB09["SB09 behavior expansion"]
    SB09 --> SB10["SB10 state transition engine"]
    SB10 --> SB11["SB11 distance inventory trade"]
    SB11 --> SB12["SB12 visual ordering and bindings"]
    SB06 --> SB13["SB13 future bridge design only"]
    SB12 --> SB13
    SB04 --> SB14["SB14 cross repo performance proofs"]
    SB13 --> SB14
    SB14 --> SB15["SB15 refactoring gate and closure"]
```

## Critical Foundations

- SB02, SB03, SB04, SB05 and SB06 are Components foundations for ordered generic WebGL run execution.
- SB07, SB08, SB09, SB10, SB11 and SB12 are Economy foundations for canonical scenario, event, frame and visual action readiness.
- SB14 is the cross-repo proof gate.
- SB15 is the final refactoring and scope closure gate.

## Phase Gates

- Do not start a dependent subbundle until its prerequisites are completed or explicitly blocked.
- Do not connect Economy projects to Components or WebGL projects in this wave.
- Do not add economy, water, well, ledger, tax or business-specific concepts to Components WebGlLib or WebGlRunLib.
- Use 1440x900 or larger for any WebGL/browser proof. Small, medium, mobile and tablet proof is out of scope.
- Every completed subbundle must have a proof manifest under `proof/SBxx/manifest.md`.
