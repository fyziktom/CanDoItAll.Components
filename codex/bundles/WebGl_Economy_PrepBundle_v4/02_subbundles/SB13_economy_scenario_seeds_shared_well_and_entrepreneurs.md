# SB13 — Economy scenario seeds: shared well and entrepreneurs

## Goal

Prepare deterministic toy scenarios for future WebGL visual proof.

## Shared well scenario

Actors:
- households;
- well keeper;
- rule council;
- optional free rider.

Resources:
- water;
- labor/maintenance;
- trust;
- rule compliance;
- reserve fund.

Events:
- draw water;
- contribute maintenance;
- missed contribution;
- rule enforcement;
- repair event;
- drought/rain event.

Outputs:
- per-step resource stores;
- conflict score;
- trust score;
- well health;
- issue list;
- visual symbols.

## Small entrepreneur community

Actors:
- baker;
- farmer;
- carpenter;
- customer group;
- local fund.

Resources:
- cash;
- goods;
- labor;
- inventory;
- debt/credit;
- trust.

Events:
- production;
- sale;
- supply purchase;
- small loan;
- delayed payment;
- investment;
- default risk.

## Rules

Seeds must live in SimpleAccounts and Abstractions-compatible fixtures.

Do not use Ledger in simple scenario seeds.

## Validation

- deterministic frame hashes;
- reproducible frame deltas;
- no WebGL coupling.
