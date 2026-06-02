# Phase plan

## Subbundle Dependency Map

```mermaid
flowchart TD
    SB01[SB01 Current-state and proof hygiene audit]
    SB02[SB02 Runtime scenario catalog and fixture decoupling]
    SB03[SB03 WebGlRun frame command preservation policy]
    SB04[SB04 Revision and runtime option reset policy]
    SB05[SB05 Patch transaction mode semantics]
    SB06[SB06 Generic/domain provenance validation boundary]
    SB07[SB07 Dynamic object reference validation policy]
    SB08[SB08 Resource ownership async race hardening]
    SB09[SB09 Package and WebGlLib-only consumption proof]
    SB10[SB10 Economy simulation docs and public surface audit]
    SB11[SB11 Browser UI and Node route proof]
    SB12[SB12 Final cross-repo red-team closure]

    SB01 --> SB02
    SB01 --> SB03
    SB01 --> SB04
    SB03 --> SB05
    SB04 --> SB05
    SB03 --> SB06
    SB06 --> SB07
    SB02 --> SB11
    SB05 --> SB11
    SB08 --> SB11
    SB09 --> SB11
    SB07 --> SB10
    SB10 --> SB12
    SB11 --> SB12
```

## Critical Subbundles

- SB01, SB02, SB03, SB04, SB05, SB06, SB08, SB09, SB11, SB12 are critical.
- SB07 is critical if the implementation chooses dynamic object support; otherwise it must produce an explicit static-scene policy.

## Phase Gates

| Gate | Before continuing | Required proof |
| --- | --- | --- |
| G01 | After SB01 | Proof inventory, source hash baseline, manifest quality audit. |
| G02 | After SB02 | Node/browser sandbox loads without any runtime `tests/` path. |
| G03 | After SB03 | Mixed direct+staged frame commands cannot be silently lost. |
| G04 | After SB04 | Revision and runtime option reset policy is documented, tested, and stable. |
| G05 | After SB05 | Strict and permissive patch modes are both proven. |
| G06 | After SB06/SB07 | Domain provenance and object-reference policy are explicit and enforced. |
| G07 | After SB08/SB09 | Resource and package proof passes with isolated cache and browser runtime stress. |
| G08 | After SB11 | Browser UI proof passes large screen, narrow width, Node route, and package/deployment-like scenario source. |
| G09 | After SB12 | Final validators, proof manifests, red-team review, and docs are consistent. |

## Execution rule

Codex must execute one subbundle at a time. After each critical subbundle, run the subbundle validator/gate, update proof manifests, and perform a short refactoring review before moving on.
