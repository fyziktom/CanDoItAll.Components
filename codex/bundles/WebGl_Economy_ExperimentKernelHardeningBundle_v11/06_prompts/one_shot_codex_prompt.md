# One-shot Codex prompt

You are working in two already cloned repositories:

- `CanDoItAll.Components`
- `CanDoItAll.Economy`

Do not create a new branch. Work only in the currently checked-out branch in each repository.

Execute `CanDoItAll_WebGl_Economy_ExperimentKernelHardeningBundle_v11` subbundle by subbundle. Do not implement the shared-well UI demo. Use shared-well and farmer-land only as readiness probes.

Hard requirements:

1. Keep WebGL generic and domain-neutral.
2. Keep WebGL desktop/large-screen only; do not optimize for small/medium/mobile/tablet screens.
3. Do not introduce Economy -> Components references in this wave.
4. Do not introduce Components -> Economy references.
5. Replace water-specific generic parameter fields with resource-scoped generic models.
6. Harden experiment input packs: validate hashes, duplicate input kinds, schema versions, relative paths, required inputs, and pack hash recomputation.
7. Ensure runtime simulation consumes explicit JSON inputs and never hidden randomness.
8. Harden ordered stage/action semantics so repeated motions on the same object run sequentially when required.
9. Add parity tests for C# and JS batch normalization.
10. Add shared-well and farmer-land readiness tests based on JSON input packs, not hardcoded runtime state.
11. Record proof after each subbundle in `proof/SBxx/`.
12. Update `reviews/01-execution-report.md` with Solved / Partially solved / Not solved closure rows.

Start with SB01. Stop and repair if a gate fails.
