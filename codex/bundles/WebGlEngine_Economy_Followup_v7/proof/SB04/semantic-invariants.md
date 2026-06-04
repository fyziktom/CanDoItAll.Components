# Semantic invariants — SB04

- This subbundle must preserve generic/domain boundaries.
- Proof must include at least one negative/failing-first check where applicable.
- Artifacts must not be empty placeholders.
- Components remains domain-neutral; Economy owns experiment readiness semantics.
- `status` must distinguish `headless-valid`, `oracle-valid`, `browser-observer-valid`, and `research-ready`; missing browser proof cannot be hidden inside economic correctness.
- `researchReady` must be false unless headless economic correctness, oracle correctness, browser observer proof, and a zero-warning budget all pass.
- Runtime, UI, projection, oracle, and browser-observer bands must be independently reported from scenario, simulation, metrics, and performance bands.
- Warning budgets and allowlists must be machine-readable and must use unique warning messages for counts.
- A warning-only condition may be exploratory, but it may not be research-ready.
