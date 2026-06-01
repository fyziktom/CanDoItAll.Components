# SB13 - Large-screen browser smoke plan

Prepare, but do not overbuild, a browser smoke path:

- load one `WebGlRunDocument`,
- render initial scene,
- play one frame's stage sequence,
- wait for object motion barrier,
- export runtime diagnostics,
- compare expected completed stage ids.

Large-screen only: 1440x900 or larger.
No mobile/tablet/small-screen optimization.

## Status

Completed.

## Goal

Prepare the first large-screen-only browser smoke criteria without implementing a polished UI demo.

## Prerequisites

- SB12 readiness report must identify fields/actions needed for browser playback.

## Owned Requirements

- R13 Large-Screen Browser Smoke Plan.

## Dependency Impact

Guides later UI/browser proof while keeping this bundle out of mobile and final-demo scope.

## Validation Depth

Checklist/report proof, plus optional browser proof only if a suitable existing route is available without overbuilding.

## Proof Required

- Large-screen smoke plan artifact.
- Execution report browser analytics row stating whether browser proof was run or intentionally deferred.
- Proof manifest.

## Progression Gate

Pass only when the smoke path is scoped to 1440x900 or larger and no mobile/tablet/small-screen optimization or final UI demo was introduced.

Gate result: Passed. `bundle://proof/SB13/large-screen-smoke-plan.md` scopes the smoke path to `1440x900` or larger, uses the generated `shared-well` WebGL run document as the candidate input, lists expected frame-1 stage ids and diagnostics, records browser proof as intentionally deferred because the existing route does not load generated artifacts or compare completed stage ids, and introduces no mobile/tablet/small-screen optimization or final UI demo.
