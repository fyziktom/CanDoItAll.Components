# SB13 - Shared well scenario as definition and materializer

Repository: `CanDoItAll.Economy`

## Goal

Make the shared-well community scenario generic and loadable.

## Scenario definition must include

Actors:
- household-north
- household-south
- free-rider
- well-keeper
- rule-council

Locations:
- household-north-home
- household-south-home
- free-rider-home
- shared-well
- council-place
- well-keeper-workplace

Resources:
- water
- labor
- compliance
- trust
- reserve-fund
- well-health

Scheduled events:
- fair water draw;
- contribution;
- missed contribution;
- repair;
- enforcement.

## Materializer

Simple account backend should materialize:
- frames;
- deltas;
- events;
- stores;
- issues;
- relationships.

## Visualization

Economy visual mapper should produce:
- stable visual nodes with location hints;
- action stream:
  - actor moves to well;
  - actor returns home;
  - rule council action;
  - repair/admin action.
