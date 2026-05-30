# SB05 — Components: batching parity and performance

## Problem

C# `WebGlSceneCommandBatchNormalizer` and JS `normalizeBatch` are separate implementations.

## Tasks

1. Create parity fixtures:
   - input batch JSON
   - expected normalized batch JSON
   - expected metrics
2. Add C# tests against fixtures.
3. Add JS/node tests or audit script against same fixtures.
4. Add performance fixture with 1000 patches/motions.
5. Add safety fixture: staged repeated motions must not be dropped.

## Done criteria

- C# and JS normalizers agree for canonical fixtures.
- Performance budget documented.
