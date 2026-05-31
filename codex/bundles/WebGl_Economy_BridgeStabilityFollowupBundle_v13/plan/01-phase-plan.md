# Phase plan

## Dependency map

```mermaid
flowchart TD
    SB01["SB01 inventory and boundary guard"] --> SB02["SB02 Components stage executor and motion queue"]
    SB02 --> SB03["SB03 Components action-plan batch compiler"]
    SB01 --> SB04["SB04 Economy initial scene projector"]
    SB03 --> SB05["SB05 Economy action/stage projection"]
    SB04 --> SB05
    SB01 --> SB06["SB06 bridge dependency strategy"]
    SB04 --> SB07["SB07 visual mapping contracts and loader"]
    SB07 --> SB08["SB08 strict fixture hashes"]
    SB01 --> SB09["SB09 generic leakage audit"]
    SB09 --> SB10["SB10 transition handler registry"]
    SB10 --> SB11["SB11 metrics and invariants"]
    SB05 --> SB12["SB12 bridge traceability"]
    SB12 --> SB13["SB13 simulation sandbox skeleton"]
    SB01 --> SB14["SB14 large-screen-only guard"]
    SB05 --> SB15["SB15 performance and scalability proofs"]
    SB02 --> SB15
    SB01 --> SB16["SB16 refactoring gate and closure"]
    SB15 --> SB16
    SB14 --> SB16
```

## Critical foundations

- SB01 is critical because later work depends on repo branch and dependency boundaries.
- SB02 and SB03 are critical because Economy projection depends on generic staged WebGL playback semantics.
- SB04, SB05, SB07, SB08, and SB12 are critical because they convert simulation/visualization state into actionable WebGL run documents without hardcoded example logic.
- SB09, SB14, and SB16 are critical closure gates for genericity, scope discipline, and proof quality.

## Progression gates

- Do not start Economy bridge projection until Components staged batch semantics have source and test proof.
- Do not close bridge projection while frames contain metadata-only stages or duplicate global actions.
- Do not close strict input pack work until committed shared-well and farmer-land fixtures pass strict hash validation.
- Do not close final bundle while boundary audits, targeted tests, and proof manifests are stale or missing.
