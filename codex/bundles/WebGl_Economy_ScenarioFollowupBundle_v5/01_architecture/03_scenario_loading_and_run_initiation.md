# Scenario definition, loading, and run initiation

## Required future flow

```text
Load scenario definition
  -> validate definition
  -> choose backend
  -> create run identity
  -> materialize frame 0
  -> start stepping or automatic materialization
  -> emit frames/deltas/events
  -> map to visual frames/actions
```

## Scenario document

Add a backend-neutral `SimulationScenarioDefinition` in `Simulation.Abstractions`.

Minimum fields:

```text
ScenarioId
DisplayName
ScenarioKind
BaseCurrency
StartsAtUtc
StepCount
StepDuration
Seed
Actors
Resources
Locations
InitialStores
Relationships
Rules
ScheduledEvents
BackendHints
Metadata
```

## Locations and anchors

The shared-well case requires positions to matter, but position must not be WebGL-specific.

Add:

```text
SimulationLocation
  LocationId
  Kind
  DisplayName
  X
  Y
  Z
  Metadata

SimulationActorHome
  ActorId
  LocationId
```

This allows the visualization mapper to know that a citizen has a home and the well has a target location.

## Scheduled events

Examples:

```text
Day 1: household-north uses well.
Day 2: free-rider uses well without contributing labor.
Day 3: rule council applies enforcement.
Day 4: repair event uses reserve fund.
```

The backend can convert scheduled events into stores/flows/issues.

## Initiating a run

Add interfaces:

```text
ISimulationScenarioDefinitionStore
ISimulationScenarioDefinitionValidator
ISimulationRunFactory
ISimulationRunFrameProvider
ISimulationEventMaterializer
```

Do not wire this into the existing full `Economy.Simulator` UI yet. Keep it as preparation layer with tests.
