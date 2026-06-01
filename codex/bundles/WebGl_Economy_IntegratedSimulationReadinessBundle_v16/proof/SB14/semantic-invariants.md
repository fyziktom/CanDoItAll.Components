# SB14 Semantic Invariants

| Invariant ID | Assertion |
|---|---|
| SB14-performance-threshold | Performance probes remain headless, thresholded, and pass after integrated readiness changes. |
| SB14-large-screen-policy | No mobile/tablet WebGL optimization is introduced to satisfy performance. |

## Shallow-pass trap

A shallow pass could skip performance tests or shift scope into UI tuning outside the bundle.

## Adversarial negative proof

`economy-performance-probe-tests.txt` exercises the performance probe test class directly.

## Semantic positive proof

`economy-performance-probe-tests.txt` passes, and SB15 full suite transcript confirms no broader regression.

## Anti-stub audit

Performance proof comes from the existing test harness, not a static timing note.

