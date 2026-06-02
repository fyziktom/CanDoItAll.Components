# Semantic Invariants - SB02

## Status

Completed.

## Invariants

- Economy sandbox services can be registered outside Node with one public extension.
- Node consumes the same registration path as other hosts.

## Adversarial Negative Proof

Source scan verifies the page no longer needs manual construction for production service wiring; BUnit uses DI registration.

## Semantic Positive Proof

Focused Economy sandbox tests and the Economy release build passed.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `AddEconomySimulationSandbox` | Economy sandbox package | Node, tests, future hosts | Startup DI | Source scan includes catalog/session registrations |
