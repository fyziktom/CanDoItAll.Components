# Economic simulation readiness assessment

## Can we already run and study economic simulations?

Yes, for exploratory and engineering use:

- scenario authoring
- headless pipeline debugging
- visual mapping debugging
- comparing generated frames and metrics
- testing run manifests and reproducibility
- validating UI/browser observer flows

No, not yet as an automatic research-grade claim pipeline.

A run should be considered research-grade only when the following are all true:

1. The scenario pack is closed and hash-validated.
2. ResearchStrict policy is active.
3. No unclassified diagnostics remain.
4. Headless backend frames/deltas are deterministic and manifest-backed.
5. Metrics and invariants come from registered evaluators.
6. External golden oracle coverage exists for the scenario class or the run is explicitly marked
   `no-oracle` and cannot claim research-ready.
7. Evidence records are resolved against real artifact bytes and schema versions.
8. Browser observer proof is optional and strictly separated from economic correctness.
9. Performance/comparability budgets pass or the result is marked `not-comparable`.
10. The scenario result survives metamorphic and conservation checks.

## What can still inject simulator noise?

- implicit store resolution or ambiguous fallback
- warning severity downgrades
- action-driver mapping fallback to wait/no-op
- unverified evidence records
- browser observer mismatch hidden by fallback proof
- scenario factor materialization that mutates only labels, not semantic inputs
- mutation-layer bugs in transfer/rejection/provenance
- over-broad domain leakage allowlists
- public API changes in Components that alter projection/replay semantics
- canary scenario metrics that measure transfer volume but not true investment feedback

## Stabilization direction

The next wave should intentionally reduce generic Components churn. After the Components freeze gate,
Economy work can continue rapidly using domain drivers, semantic drivers, scenarios, oracles, and UI
improvements without reopening WebGlLib/WebGlRunLib for every economic need.
