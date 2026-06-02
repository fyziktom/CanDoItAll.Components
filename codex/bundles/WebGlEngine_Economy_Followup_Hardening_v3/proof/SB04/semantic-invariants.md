# Semantic Invariants - SB04

## Status

Completed.

## Invariants

- Public session APIs support scenario ids instead of requiring caller-owned file paths.
- Portable exports bind to scenario id plus pack hash.

## Adversarial Negative Proof

Tests reject tampered pack hashes and bad snapshot/session state during import.

## Semantic Positive Proof

`LoadScenario`, export, moved import, and catalog-backed import all pass in focused tests.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Session export v2 | Session service | Import service, UI workflows | Export/import | Pack hash mismatch rejected |
