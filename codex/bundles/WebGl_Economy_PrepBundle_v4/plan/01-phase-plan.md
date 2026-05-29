# Phase Plan

## Execution Order

- SB01: current branch and inventory guard.
- SB02: Components JS command-result hardening.
- SB03: Components scene document hashing and validation.
- SB04: Components render scheduler and resource disposal hardening.
- SB05: Components model diagnostics batch report.
- SB06: Components WebGlRunLib foundation.
- SB07: Components refactoring gate A.
- SB08: Economy inventory and boundary guard.
- SB09: Economy shared simulation abstractions.
- SB10: Economy simple-account backend preparation.
- SB11: Economy ledger-backed simulation adapter preparation.
- SB12: Economy visualization contracts without WebGL dependency.
- SB13: Economy scenario seeds.
- SB14: cross-repo no-coupling validation.
- SB15: refactoring gate B and closure.

## Subbundle Dependency Map

```mermaid
flowchart TD
  SB01["SB01 inventory"] --> SB02["SB02 command results"]
  SB02 --> SB03["SB03 scene document"]
  SB03 --> SB04["SB04 render scheduler"]
  SB04 --> SB05["SB05 model diagnostics"]
  SB05 --> SB06["SB06 WebGlRunLib"]
  SB06 --> SB07["SB07 Components gate A"]
  SB01 --> SB08["SB08 Economy boundary"]
  SB08 --> SB09["SB09 abstractions"]
  SB09 --> SB10["SB10 simple accounts"]
  SB09 --> SB11["SB11 ledger adapter"]
  SB09 --> SB12["SB12 visualization"]
  SB10 --> SB13["SB13 scenarios"]
  SB12 --> SB13
  SB07 --> SB14["SB14 cross-repo validation"]
  SB11 --> SB14
  SB13 --> SB14
  SB14 --> SB15["SB15 closure"]
```

## Critical Subbundles

- SB01 is critical because branch and inventory evidence protect the execution baseline.
- SB14 is critical because it consolidates the cross-repo build, test, screenshot, and no-coupling evidence for subbundles 02 through 13.
- SB15 is critical because it closes the reports, proof manifests, and validator handoff.

## Phase Gates

- Do not start dependent work until listed prerequisites are completed or explicitly reopened.
- Run required builds, tests, audits, dependency scans, and browser screenshots before final closure.
- Reopen the owning subbundle if source evidence contradicts a boundary, render, hash, or deterministic-output claim.
