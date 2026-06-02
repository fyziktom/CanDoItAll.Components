# Phase Plan

## Subbundle Dependency Map

```mermaid
flowchart TD
    SB01[SB01 Current-state and proof integrity audit]
    SB02[SB02 Service registration and scenario catalog contracts]
    SB03[SB03 Scenario selection and manifested runtime packs]
    SB04[SB04 Pathless session API and portable export]
    SB05[SB05 Async session persistence and store boundaries]
    SB06[SB06 WebGlRun apply safety and reset fail-fast]
    SB07[SB07 Runtime options document import policy]
    SB08[SB08 Provenance schema and domain boundary hardening]
    SB09[SB09 Browser UI diagnostics and operator UX]
    SB10[SB10 Asset cache and large simulation performance budget]
    SB11[SB11 Package versioning and consumer proof]
    SB12[SB12 Final cross-repo red-team closure]

    SB01 --> SB02
    SB01 --> SB06
    SB01 --> SB08
    SB02 --> SB03
    SB03 --> SB04
    SB04 --> SB05
    SB06 --> SB07
    SB08 --> SB09
    SB03 --> SB09
    SB07 --> SB09
    SB10 --> SB12
    SB11 --> SB12
    SB05 --> SB12
    SB09 --> SB12
```

## Critical Subbundles

| Subbundle | Critical? | Why |
|---|---|---|
| SB01 | Yes | Prevents proof-theater and establishes true current state. |
| SB02 | Yes | Determines whether the component can be consumed outside Node. |
| SB03 | Yes | Defines runtime scenario-pack contract. |
| SB04 | Yes | Prevents absolute-path session lock-in. |
| SB06 | Yes | Prevents silent WebGlRun command loss and stale-scene mutation. |
| SB07 | Yes | Preserves scene document runtime semantics. |
| SB08 | Yes | Prevents domain leakage into generic Components packages. |
| SB12 | Yes | Final red-team gate. |

## Phase Gates

- After SB01, stop and update the bundle if current-state proof contradicts this plan.
- After SB03, stop and review scenario-pack API before pathless session work.
- After SB06, stop and run a browser proof that intentionally triggers reset failure and mixed-frame input.
- After SB08, stop and run both Components boundary audits and Economy provenance bridge tests.
- SB12 cannot start until all prior proof manifests are complete and non-empty.
