# Phase plan

## Refactor gates

- Gate A after SB03: stop/pause and runtime idle semantics must be proven before browser observer work continues.
- Gate B after SB07: research-readiness claims must be artifact-backed before oracle/design work continues.
- Gate C after SB12: semantic economic correctness must be protected by data-driven oracles and metamorphic tests before performance work continues.
- Gate D after SB15: browser observer and performance proof must be stable before final red-team closure.

## Work order

1. Stabilize runtime stop/idle proof.
2. Harden readiness evidence semantics.
3. Remove unclassified diagnostics and design-matrix no-op risk.
4. Externalize oracle fixtures and add metamorphic tests.
5. Harden store resolution, metrics/invariants, behavior profile drift.
6. Harden headless manifest/diff and statistical comparison.
7. Add browser/performance observer gates.
8. Red-team everything across both repos.
