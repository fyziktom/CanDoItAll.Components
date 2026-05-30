# SB13 - Economy ledger adapter hardening

## Goal

Keep ledger-backed simulation isolated and improve projection correctness.

## Required changes

- Keep `Simulation.Ledger` as the only project that references Ledger/BO/SDK.
- Add real delta diffing for ledger frames:
  - changed balances only
  - added/resolved issues
  - added artifacts
- Add validation for snapshot sequence ordering.
- Add explicit `LedgerScenarioForkDescriptor` validation.
- Add tests for two snapshots producing a minimal delta.

## Performance bottleneck

Current ledger delta can return all target stores. This is acceptable as a first adapter, but it will be expensive for large ledgers. Add minimal diff helpers now.
