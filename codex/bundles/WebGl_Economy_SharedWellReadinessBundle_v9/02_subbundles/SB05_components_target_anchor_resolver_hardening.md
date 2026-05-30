# SB05 - Components: target/anchor resolver hardening

## Problem
Target resolution must work for objects, locations, places, resource nodes, store nodes and explicit positions.

## Tasks
- Add/finish a reusable target resolver in WebGlRunLib.
- Resolver inputs:
  - scene object id
  - target object id
  - anchor key
  - explicit position
  - object anchors
  - fallback anchor policy
- Resolver outputs:
  - resolved position
  - target object id
  - anchor kind
  - diagnostics/warnings
- Add common anchor keys:
  `home`, `use`, `admin`, `work`, `trade`, `front`, `center`, `top`, `base`.

## Tests
- Actor home anchor.
- Well use anchor.
- Council admin anchor.
- Missing target returns a failed diagnostic, not an exception.
