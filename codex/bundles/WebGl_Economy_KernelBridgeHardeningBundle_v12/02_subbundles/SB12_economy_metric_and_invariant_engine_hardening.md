# SB12 - Economy Metric and Invariant Engine Hardening

## Goal

Make experiment interpretation explicit.

## Required metrics

Generic metric evaluator should support:

- total quantity by resource
- top owner share by resource
- HHI / concentration index by resource
- rule violation count by category
- average access cost by actor group
- stock depletion rate
- relationship/trust/conflict aggregate
- admin burden time/cost
- transfer/trade volume

## Required invariants

- total conserved resource within tolerance
- no actor exceeds max resource share
- no store below zero unless allowed
- every frame references known actors/resources/stores
- every event references known participants
- every visual action maps to a known node or has explicit fallback

## Tests

Use:
- shared-well
- farmer-land
- synthetic ambiguous actor/resource ID case
