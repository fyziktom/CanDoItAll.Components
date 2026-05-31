# SB10 — Transition engine handler registry and mutation safety

## Problem
The simple transition engine has improved, but event handling can still grow into a backend-specific monolith.

## Tasks
- Introduce `ISimulationEventHandler` and registry.
- Split handlers by generic capability:
  - resource transfer
  - resource stock change
  - relationship update
  - rule issue
  - tax/fee event
  - generic effects
- Keep SimpleAccounts backend as one implementation of generic handlers.
- Add mutation guard:
  - no negative stores unless explicitly allowed
  - capacity overflow reports diagnostics
  - missing actor/store/resource is an error or warning, not silent no-op.

## Tests
- unknown event kind produces diagnostic.
- missing store produces diagnostic.
- negative stock blocked unless allowed.
