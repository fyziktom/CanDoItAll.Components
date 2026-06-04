# Semantic invariants — SB06

- This subbundle must preserve generic/domain boundaries.
- Proof must include at least one negative/failing-first check where applicable.
- Artifacts must not be empty placeholders.
- Research-ready economic results must never depend on incidental `StoreId` ordering.
- Multiple matching source/target/shared/effect stores must fail in research strict mode unless a policy disambiguates them.
- Every accepted or rejected flow must carry store-resolution metadata explaining source/target policy, selected store id, reason, and candidate count.
- Capacity rejection, stock rejection, and zero accepted transfer conditions must be machine-readable diagnostics.
- Rejected flows must preserve the same resolution metadata as accepted flows.
- Components remains domain-neutral; store-selection semantics live in Economy simulation code.
