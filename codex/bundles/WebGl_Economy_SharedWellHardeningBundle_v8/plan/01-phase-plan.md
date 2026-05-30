# WebGL + Economy Shared-Well Hardening Phase Plan

Hard rules:

- Work in the branch already checked out in each repository.
- Keep Components generic and independent from Economy.
- Keep Economy independent from Components/WebGL.
- Validate WebGL only on desktop/large-screen viewports, 1440x900 or larger.

## Dependency Map

```mermaid
flowchart TD
    SB01["SB01 inventory and branch guard"]
    SB02["SB02 ordered action stages"]
    SB03["SB03 action normalizer"]
    SB04["SB04 target and distance policy"]
    SB05["SB05 JS/C# batch parity"]
    SB06["SB06 runtime performance"]
    SB07["SB07 asset cache lifetime"]
    SB08["SB08 playback controller"]
    SB09["SB09 JS audit and large-screen policy"]
    SB10["SB10 canonical scenario model"]
    SB11["SB11 event taxonomy and order"]
    SB12["SB12 behavior/rule expansion"]
    SB13["SB13 distance/capacity/trade"]
    SB14["SB14 visual action mapper"]
    SB15["SB15 SimpleAccounts materializer"]
    SB16["SB16 ledger adapter"]
    SB17["SB17 bridge design only"]
    SB18["SB18 shared-well readiness proof"]
    SB19["SB19 performance proofs"]
    SB20["SB20 closure"]

    SB01 --> SB02 --> SB03 --> SB04 --> SB05 --> SB06 --> SB07 --> SB08 --> SB09
    SB01 --> SB10 --> SB11 --> SB12 --> SB13 --> SB14 --> SB15 --> SB16
    SB09 --> SB17
    SB14 --> SB17
    SB16 --> SB18
    SB17 --> SB18
    SB18 --> SB19 --> SB20
```

## Critical Foundations

- SB02 is critical for preserving sequential visual meaning across WebGL run frames.
- SB03 is critical for avoiding alias drift in Components action processing.
- SB05 is critical for browser/runtime parity with C# batch normalization.
- SB10 and SB11 are critical for canonical Economy semantics.
- SB12 through SB14 are critical for the shared-resource scenario proof.
- SB18 and SB19 are critical closure proofs.

## Progression Gates

- Components implementation must not start until SB01 inventory confirms the branch and clean starting state.
- Components downstream subbundles must re-check SB02/SB03/SB05 if ordered action proof weakens.
- Economy downstream subbundles must re-check SB10/SB11 if canonicalization or ordering proof weakens.
- SB17 is documentation-only and must not introduce cross-repo references.
- SB18 must include no-coupling proof and explicit shared-well readiness status.
- SB19 must include desktop-only WebGL screenshots or browser artifacts for rendered scene/model proof.

