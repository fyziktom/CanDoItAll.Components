# SB01 — Cross-repo inventory and workflow gate

## Repo scope

- `CanDoItAll.Components`
- `CanDoItAll.Economy`

## Tasks

1. Confirm current branch in each repo; do not create a new branch.
2. Record current commit hashes.
3. Confirm `Components` contains `WebGlLib`, `WebGlRunLib`, `WebGlSandbox`, and tests.
4. Confirm `Economy` contains `Simulation.Abstractions`, `Simulation.SimpleAccounts`, `Simulation.Ledger`, `Simulation.Visualization`, and tests.
5. Create/refresh bundle proof manifests using the CanDoItAll bundle workflow style:
   - proof/SBxx/manifest.md
   - proof/SBxx/semantic-invariants.md
   - transcripts/
   - source assertions
   - changed-file hashes
   - anti-stub audit

## Done criteria

- No new branch created.
- Inventory report exists.
- Bundle status and proof paths are synchronized.
