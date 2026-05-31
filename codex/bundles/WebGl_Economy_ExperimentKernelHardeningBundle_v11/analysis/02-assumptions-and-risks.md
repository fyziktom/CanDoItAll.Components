# Assumptions and risks

## Critical Path Risks

- Components SB02/SB03 are prerequisites for any proof that same-object motion sequences are visually ordered.
- Economy SB08/SB09 are prerequisites for later generic simulation proof because weak input validation or water-specific abstractions would invalidate downstream readiness claims.
- Economy SB12/SB15 are prerequisites for shared-well and farmer-land readiness because transition semantics and invariant evaluation must be behavior-backed, not table-backed.

## Validation Risks

- This bundle spans two repos but forbids cross-repo references; proof must cite both repos without wiring them together.
- Some proof is command-line and source-level rather than browser-visible because the bundle explicitly forbids building the shared-well UI demo in this wave.
- Runtime randomness scanning must be scoped to simulation transition/input-pack code, because other Economy subsystems intentionally use clocks and generated IDs outside this bundle.

## Reopen Triggers

- Reopen SB02/SB03 if stage execution still enqueues competing active motions for the same object.
- Reopen SB08 if an invalid input pack can pass with duplicate kinds, missing required documents, unsafe relative paths, bad content hashes, or stale pack hashes.
- Reopen SB09 if public generic parameter contracts still contain water/well-specific properties.
- Reopen SB12/SB15 if readiness tests pass only from hardcoded runtime state rather than explicit input documents.
- Reopen SB18 if any Components-to-Economy or Economy-to-Components project reference is introduced.
