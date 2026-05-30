# SB01 - Cross-repo inventory and branch guard

## Scope
Both repos.

## Tasks
- Confirm current branch in each repo and write it to proof output.
- Do not create or switch to a new branch.
- Record solution/project inventory.
- Record changed-file counts and any large files.

## Must validate
- Components solution includes WebGlLib, WebGlRunLib, WebGlSandbox and tests.
- Economy solution includes Simulation.Abstractions, SimpleAccounts, Ledger, Visualization and tests.
- No new Components/WebGL references appear in Economy Simulation.* projects.
