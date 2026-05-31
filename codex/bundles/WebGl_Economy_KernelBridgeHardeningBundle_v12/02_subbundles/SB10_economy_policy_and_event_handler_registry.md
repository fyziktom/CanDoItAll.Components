# SB10 - Economy Policy and Event Handler Registry

## Problem

`SimpleSimulationStateTransitionEngine` has a handler dictionary, but handlers are still internal lambdas/static methods inside one large class. It is better than a switch but not yet plugin-friendly.

## Goal

Create a pluggable event handler model.

## Proposed contracts

```csharp
public interface ISimulationEventHandler
{
    string EventKind { get; }
    bool CanHandle(SimulationEvent simulationEvent);
    SimulationEventApplyResult Apply(SimpleSimulationState state, SimulationEvent simulationEvent);
}

public interface ISimulationEventHandlerRegistry
{
    IReadOnlyList<ISimulationEventHandler> Resolve(string eventKind);
}
```

## Required behavior

- Unknown event kind produces warning diagnostic, not silent ignore.
- Handlers can add:
  - store changes
  - relationship changes
  - issues
  - artifacts
  - flows
  - derived events
- Handler ordering is deterministic.
- Domain-specific policies are registered through handlers, not hardcoded in the engine core.

## Tests

- shared resource use handled through handler
- farmer land transfer handled through handler
- unknown event returns diagnostic
- handler order stable
