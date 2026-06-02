# Phase plan and refactor gates

## Phase A: Baseline and safety

- SB01 Current-state and proof integrity audit.
- SB02 WebGlRun fail-closed runner semantics.
- SB03 Multi-frame playback and replay API.

Gate A: No browser/runner path may apply commands after frame conversion errors, reset failure, or playback errors.

## Phase B: Scenario/session source model

- SB04 Economy UI deterministic replay.
- SB05 Pathless scenario source contract.
- SB06 Scenario pack manifest and security.
- SB07 Async session persistence and portable export.

Gate B: Runtime UI and Node route must work without test fixture paths and without machine-local absolute path assumptions.

## Phase C: Genericity and lifecycle

- SB08 Generic provenance policy v2.
- SB09 Stage-order parity and dynamic-object lifecycle.
- SB10 WebGlSceneView external import lifecycle.

Gate C: Components remains generic; validation/apply order is identical; external scene import does not leave component lifecycle state stale.

## Phase D: Scale and closure

- SB11 Large simulation performance and resource budgets.
- SB12 Final cross-repo red-team closure.

Gate D: Final proof is non-empty, assertion-backed, and cross-repo.
