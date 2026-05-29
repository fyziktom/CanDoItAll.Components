# SB01 Semantic Invariants

- Invariant ID: `SB01-inventory-baseline`
- Source raw note: Work in the currently checked-out branch and do not create a branch.
- Expected behavior: Both repositories have branch/status/project inventories before implementation starts.
- Disallowed shallow implementation: A written assertion without branch/status command evidence.
- Failing-first test: N/A process-only no production behavior; no code-path failure fixture applies.
- Passing test: `git status --short` and inventory file checks recorded in `bundle://proof/SB01/transcripts/inventory-validation.md`.
- Changed source files: `repo://artifacts/webgl-engine-prep-v4/01_INVENTORY.md`
- Production assertions: Inventory evidence is concrete and dated; branch creation did not occur.
- Red-team negative case: The transcript would expose branch creation or dirty unrelated work.
- Downstream dependency check: SB02-SB15 proceeded only after SB01 inventory evidence was available.
