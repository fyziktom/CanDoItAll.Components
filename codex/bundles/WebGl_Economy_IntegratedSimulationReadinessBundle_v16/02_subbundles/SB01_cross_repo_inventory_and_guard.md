# SB01 - Cross-repo inventory and guard

## Goal
Reconfirm the current branches, current projects, changed files, and architecture boundaries before implementation.

## Required actions

1. Do not create a new branch.
2. Record current branch for both repos.
3. Run or update boundary audits:
   - Components: WebGL runtime audit.
   - Economy: simulation boundary audit.
4. Verify no Components project references Economy.
5. Verify only `Economy.Simulation.WebGlBridge` references Components WebGL projects.
6. Verify `Economy.SimulationSandbox` is in Economy, not Components.

## Proof

- `proof/SB01/manifest.md`
- branch names
- changed-file hashes
- boundary audit transcript
