# SB10 — Economy simple-account backend preparation

## Goal

Prepare a toy/community backend that does not use ledger/UTXO.

## Project

Create:

`src/CanDoItAll.Economy.Simulation.SimpleAccounts`

References:
- `Simulation.Abstractions`
- optionally `Core`.

## Contracts/classes

Add:

- `SimpleAccount`
- `SimpleAccountBalance`
- `SimpleResource`
- `SimpleFlow`
- `SimpleObligation`
- `SimpleRule`
- `SimpleSimulationBackend`
- `SimpleSimulationScenarioFactory`

## Scenario seeds

Add factory seeds for:
- shared well community;
- small entrepreneur community.

## Rules

No ledger, no business object, no SDK references.

## Validation

- tests that simple backend can emit frames and deltas;
- dependency scan proves no ledger references.
