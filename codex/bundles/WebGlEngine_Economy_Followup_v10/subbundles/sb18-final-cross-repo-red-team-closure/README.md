# SB18 — Final cross-repo red-team closure

## Objective
Independent QA pass over genericity, scenario trust, proof evidence, browser idle, and package boundaries.

## Scope
No feature work except fixes found by QA.

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
Final report says what is safe to claim and what remains exploratory.

## QA red flags

- Passing only by changing expectations instead of behavior.
- Skipped tests, empty transcripts, or screenshots without machine-readable assertions.
- Generic package references to Economy-specific terms except through explicit driver extension points.
- Research readiness status raised without artifact-backed evidence.
