# SB08 — Economy inventory and boundary guard

## Goal

Prepare Economy repo without coupling it to Components.

## Tasks

1. Record current project graph from `CanDoItAll.Economy.slnx`.
2. Add a dependency boundary document:
   - `docs/simulation/architecture-boundaries.md`
3. Add or update a dependency scan script:
   - `scripts/audit-simulation-boundaries.ps1` or `.sh`
4. Boundary scan must fail on:
   - Components/WebGl references in Economy in this wave;
   - `Simulation.Abstractions` referencing Ledger/BusinessObjects/Sdk;
   - `Simulation.SimpleAccounts` referencing Ledger/BusinessObjects/Sdk;
   - `Simulation.Visualization` referencing Components/WebGl.

## Output

`artifacts/economy-simulation-prep-v4/BOUNDARY_AUDIT.md`
