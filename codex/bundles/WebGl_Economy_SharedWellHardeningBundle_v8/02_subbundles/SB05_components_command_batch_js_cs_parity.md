# SB05 — Components: JS/C# command batch parity

## Problem
C# and JS both normalize batches. Drift is dangerous.

## Required work
- Define a shared set of JSON fixtures for command batches.
- C# test serializes normalized batch.
- JS audit script normalizes the same fixture and compares meaningful fields.
- Cover:
  - coalesced patch;
  - duplicate motion;
  - ordered stages;
  - no coalescing across stages.

## Bottleneck
Interop call count must stay low, but correctness of ordered actions wins over aggressive coalescing.
