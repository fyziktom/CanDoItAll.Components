# SB01 Semantic Invariants

## INV-SB01-001 Current Branches Are Preserved

- Expected behavior: execution stays on the currently checked-out branches in both local clones.
- Disallowed shallow pass: recording branch names without proving current status/log context.
- Evidence: `bundle://proof/SB01/transcripts/components-inventory.txt` and `bundle://proof/SB01/transcripts/economy-inventory.txt`.

## INV-SB01-002 Inventory Baseline Exists

- Expected behavior: source areas named by SB01 are inventoried before implementation edits.
- Disallowed shallow pass: only checking repository root status.
- Evidence: both inventory transcripts include source/test file listings for Components WebGL/WebGlRunLib and Economy Simulation.Abstractions/SimpleAccounts/Visualization/tests/fixtures.

