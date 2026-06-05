# SB08 — Readiness evidence contract v3

Completed. See `proof/SB08/manifest.md` and `reviews/01-execution-report.md`.

## Objective
Make readiness bands artifact-derived: browser/UI/oracle flags computed from evidence bundle, not caller-provided booleans.

## Scope
Economy SimulationSandbox readiness.

## Implementation instructions

1. Read `analysis/01-current-state-after-v9.md` and `analysis/02-weaknesses-and-remediation.md`.
2. Make the smallest coherent set of changes needed for this subbundle.
3. Do not proceed to the next subbundle until all required proof for this subbundle exists and is referenced from the proof manifest.
4. Preserve the boundary: generic Components code must remain domain-neutral; Economy-specific language belongs in Economy driver/packages/scenario files.
5. Update docs/tests only when they prove the behavior and are not a substitute for implementation.

## Acceptance criteria

- Code compiles in the affected repository/repositories.
- Focused tests for this subbundle pass.
- Proof artifacts are non-empty and cited.
- No new domain leakage is introduced.
- Any remaining risk is explicitly documented in the subbundle proof manifest.

## Required proof
ResearchReady cannot be true without artifact citations, hashes, oracle proof, and browser observer proof.

## QA red flags

- Passing only by changing expectations instead of behavior.
- Skipped tests, empty transcripts, or screenshots without machine-readable assertions.
- Generic package references to Economy-specific terms except through explicit driver extension points.
- Research readiness status raised without artifact-backed evidence.
