# Phase plan

## Subbundle Dependency Map

```mermaid
flowchart TD
    SB01["SB01 inventory and workflow gate"] --> SB02["SB02 motion queue semantics"]
    SB02 --> SB03["SB03 stage-aware playback executor"]
    SB03 --> SB04["SB04 C#/JS batch normalizer parity"]
    SB04 --> SB05["SB05 WebGlRunLib refactor inventory"]
    SB05 --> SB06["SB06 provenance and input-pack bridge contracts"]
    SB06 --> SB07["SB07 Components performance gates"]
    SB01 --> SB08["SB08 Economy input pack validator"]
    SB08 --> SB09["SB09 remove domain-specific abstraction fields"]
    SB09 --> SB10["SB10 canonical scenario cleanup"]
    SB10 --> SB11["SB11 event taxonomy and handler registry"]
    SB11 --> SB12["SB12 transition engine hardening"]
    SB12 --> SB13["SB13 placement topology and distance cost"]
    SB13 --> SB14["SB14 policy/rule expansion"]
    SB14 --> SB15["SB15 metrics and invariant evaluator"]
    SB15 --> SB16["SB16 shared-well readiness probe"]
    SB15 --> SB17["SB17 farmer-land generalization probe"]
    SB07 --> SB18["SB18 cross-repo bridge design only"]
    SB16 --> SB18
    SB17 --> SB18
    SB18 --> SB19["SB19 performance and bottleneck proof"]
    SB19 --> SB20["SB20 workflow closure audit"]
```

## Critical Subbundles

All subbundles are marked critical because they either protect domain-neutral WebGL ordering, deterministic experiment input discipline, generic Economy simulation semantics, or final proof integrity. SB02, SB03, SB08, SB09, SB12, SB15, SB16, SB17, and SB20 are critical foundations with semantic positive and adversarial negative proof requirements.

## Phase Gates

- SB01 must record current branches, dirty files, and the repaired bundle readiness gate before feature code changes.
- Components subbundles SB02-SB07 must not introduce Economy references or non-desktop WebGL work.
- Economy subbundles SB08-SB17 must not introduce Components/WebGL references and must keep simulation input-driven.
- SB18 is design-only; no direct cross-repo project references may be added.
- SB19 must include performance proof for batch normalization and deterministic simulation surfaces.
- SB20 must close raw findings one by one as Solved, Partially solved, or Not solved and rerun completed-stage validation.
