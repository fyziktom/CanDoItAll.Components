# SB08 - Economy inventory and boundary gate

Repository: `CanDoItAll.Economy`

## Goal

Audit the new Simulation.* projects before adding more scenario complexity.

## Required checks

- Current branch, no new branch.
- Project reference graph for:
  - `Simulation.Abstractions`
  - `Simulation.SimpleAccounts`
  - `Simulation.Ledger`
  - `Simulation.Visualization`
- Ensure no `CanDoItAll.Components`, WebGL, Blazor, BaseLib, Charts, or UI dependencies in the new Simulation.* projects.
- Ensure `SimpleAccounts` does not reference Ledger/BusinessObjects/SDK.
- Ensure `Visualization` references only Abstractions.
- Ensure `Ledger` does not reference SimpleAccounts.

## Evidence

Write:

```text
artifacts/webgl-economy-followup-v5/economy/SB08/boundary-report.md
```
