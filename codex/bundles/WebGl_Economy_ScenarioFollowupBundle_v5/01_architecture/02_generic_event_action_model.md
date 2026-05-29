# Generic event/action model

## Problem

A simulator event like "person goes to the well" must eventually become a visual action:

1. person starts at home;
2. person moves smoothly to the well;
3. optional symbol appears above the person;
4. optional pose/asset changes;
5. resource transfer is shown;
6. person returns home.

This must not be hardcoded for wells. The generic engine must understand only abstract actions and object IDs.

## Components-side generic action vocabulary

Add to `WebGlRunLib`:

```text
WebGlRunEvent
  EventId
  EventKind
  SubjectObjectId
  TargetObjectId
  StartsAtSeconds
  DurationSeconds
  Metadata

WebGlRunAction
  ActionId
  ActionKind
  SubjectObjectId
  TargetObjectId
  StartsAtSeconds
  DurationSeconds
  Parameters
  Metadata
```

Suggested generic action kinds:

```text
move-to-object
move-to-position
return-to-anchor
set-asset
set-pose
show-symbol
hide-symbol
pulse-link
resource-transfer-visual
wait
apply-scene-patch
```

These are not economy concepts. They are generic visual playback actions.

## Economy-side event vocabulary

Add to `Simulation.Abstractions`:

```text
SimulationEvent
  EventId
  EventKind
  ActorIds
  ResourceIds
  SourceId
  TargetId
  OccurredAtUtc
  Duration
  Magnitude
  Effects
  Metadata
```

Suggested generic economy event kinds:

```text
resource-use
resource-transfer
travel
work
administration
rule-check
rule-violation
rule-enforcement
relationship-change
production
consumption
trade
loan
repayment
maintenance
```

These are simulation semantics, not WebGL commands.

## Economy visualization action vocabulary

Add to `Simulation.Visualization`:

```text
EconomyVisualAction
  ActionId
  ActionKind
  SubjectNodeId
  TargetNodeId
  StartsAtSeconds
  DurationSeconds
  SymbolCategory
  PoseKey
  Intensity
  Metadata
```

Suggested visual action kinds:

```text
move-to-target
return-to-home
change-pose
show-status-symbol
hide-status-symbol
show-resource-flow
pulse-relationship
```

This still does not reference WebGL types. A later bridge maps these to WebGlRun actions.
