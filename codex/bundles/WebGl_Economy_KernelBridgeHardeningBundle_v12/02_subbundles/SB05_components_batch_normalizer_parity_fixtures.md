# SB05 - Components C# / JS Batch Normalizer Parity Fixtures

## Problem

The C# and JS normalizers are duplicated. They must produce equivalent stage, coalescing, and duplicate-motion decisions.

## Goal

Create JSON fixtures consumed by both C# tests and JS audit.

## Fixture cases

- flat independent patches can coalesce
- ordered patches cannot coalesce
- patch + motion + pose change cannot coalesce
- duplicate motions are dropped in coalesce-independent mode
- duplicate motions are preserved in preserve-order mode
- duplicate motions are preserved inside sequential stages
- wait stage survives normalization
- metadata flags `preserveOrder`, `requiresOrderedSemantics`, `dependsOnIntermediateState` are respected

## Proof

- C# test snapshot
- JS audit output
- Compare normalized summary JSON fields:
  - command counts
  - stage count
  - coalesced patch count
  - dropped duplicate motion count
  - preserved ordered duplicate motion count
