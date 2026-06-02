# Semantic Invariants - SB09

## Status

Completed.

## Invariants

- Operator UI exposes scenario identity, version, pack hash, validity, and runtime diagnostics.
- Browser-visible controls operate against runtime session services.

## Adversarial Negative Proof

Runtime fixture path scan found no `tests/` or fixture dependency in production UI/scenario routes; the only match is an external W3C URL in sample data.

## Semantic Positive Proof

Browser proof loads the sandbox, applies a frame, steps, backs up, switches to `farmer-land`, and records console with zero errors/warnings.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Sandbox browser diagnostics | Economy UI | Bundle proof | Runtime/browser | Console has 0 errors and 0 warnings |
