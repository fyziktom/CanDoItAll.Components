# Semantic invariants - SB07

- Components remains domain-neutral; metric and invariant semantics live in Economy.
- Research-grade metric evaluation must not infer fallback kinds or emit default-valued metrics for unknown kinds.
- Research-grade invariant evaluation must not pass unknown or underspecified known invariant kinds.
- Every emitted metric value carries `kind`, `unit`, `precision`, `sourceFrameHash`, and `evaluatorVersion`.
- Oracle-covered metric values are deterministic and exact after descriptor precision rounding.
- Proof includes a failing-first transcript and real evaluator JSON artifact, not screenshots or placeholder files.
