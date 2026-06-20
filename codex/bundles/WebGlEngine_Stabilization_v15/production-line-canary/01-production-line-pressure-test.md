
# Production-line canary pressure test

The future manufacturing-line simulator should not require Economy concepts in Components. It should use generic scene/run primitives.

## Generic visual model

- Scene objects: `station`, `buffer`, `operator`, `work-item`, `inspection-point`.
- Links: `conveyor`, `route`, `dependency`, `inspection-flow`.
- Status symbols: `running`, `blocked`, `starved`, `maintenance`, `quality-alert`, `queue-length`.
- Motions: WIP token moves from station to station using `MoveToObject` and `DirectedFlowVisual`.
- Stages: load, process, move, inspect, block/unblock.
- Interactions: select station, drag layout object, focus station, toggle layer visibility, inspect diagnostics.

## What it must prove

- Generic engine can visualize a domain with stations, queues, WIP, alarms, and utilization without adding manufacturing terms to generic source.
- Domain driver can map manufacturing action vocabulary to `WebGlRunActionKinds` without generic code knowing manufacturing semantics.
- Browser observer proof can compare expected run document, browser-loaded document, exported scene, runtime diagnostics and final object positions.

## Not in scope

- No actual manufacturing simulation engine in Components.
- No production-line domain package in this bundle unless it is a tiny test-only canary fixture.
- No Economy code changes.
