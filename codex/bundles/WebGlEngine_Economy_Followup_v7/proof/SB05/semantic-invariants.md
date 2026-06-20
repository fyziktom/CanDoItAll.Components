# Semantic invariants — SB05

- This subbundle must preserve generic/domain boundaries.
- Proof must include at least one negative/failing-first check where applicable.
- Artifacts must not be empty placeholders.
- Research-claim readiness must use a first-class policy, not implicit caller convention.
- `ResearchStrict` must fail unknown handlers, insufficient stock, missing references, ambiguous stores, unknown metrics, and unknown invariants.
- Research strict mode must use a zero-warning budget unless an explicit allowlist/budget is supplied for non-research exploration.
- Demo/permissive policy must remain runnable but must report `researchReady: false`.
- Headless runner and readiness reports must expose the effective policy id.
- Components remains domain-neutral; all policy semantics live in Economy simulation code.
