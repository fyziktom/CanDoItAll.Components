# SB11 — Economy ledger-backed simulation adapter preparation

## Goal

Prepare a separate adapter for ledger-backed simulations without polluting shared abstractions.

## Project

Create:

`src/CanDoItAll.Economy.Simulation.Ledger`

References:
- `Simulation.Abstractions`
- `Economy.Ledger`
- `Economy.BusinessObjects`
- `Economy.Sdk`

## Contracts/classes

Add stubs and adapters:

- `LedgerSimulationBackend`
- `LedgerScenarioForkDescriptor`
- `LedgerSnapshotSimulationSource`
- `LedgerProjectionFrameProjector`
- `LedgerBusinessObjectEvidenceMapper`
- `LedgerSimulationIssueMapper`

## Rules

Do not reference SimpleAccounts.

Do not move ledger scenario logic into Abstractions.

Do not change existing ledger transaction validation.

## Validation

- dependency scan passes;
- adapter compiles;
- tests can build a frame from fake/minimal ledger projection data.
