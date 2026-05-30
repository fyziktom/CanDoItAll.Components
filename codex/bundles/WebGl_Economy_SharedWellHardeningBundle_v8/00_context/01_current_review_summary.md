# Current review summary

## Components

The Components repo moved in the correct direction:

- `CanDoItAll.Components.WebGlRunLib` exists and is included in the solution.
- Generic run contracts exist: `WebGlRunDocument`, `WebGlRunTimeline`, `WebGlRunFrame`, playback interfaces, frame source/store abstractions.
- Generic action contracts exist: `WebGlRunEvent`, `WebGlRunAction`, `WebGlRunActionTarget`, pose/symbol/action binding definitions.
- Scene command batching exists in both C# and JS.
- JS runtime has a scheduler, indexes, command result helper, asset cache helper, and large-screen policy audit.

However, hardening is still needed before this can safely drive complex scenario playback.

## Economy

The Economy repo also moved in the correct direction:

- Scenario definitions are split into abstractions.
- Scenario validation, serializer, loader, event stream compiler, simple account materializer, ledger adapter, and visualization actions exist.
- Shared-well and entrepreneur examples now have scenario definitions and events.

However, the current implementation still has several risks:

- Some contracts still contain duplicate/alias fields that can drift.
- The shared-well example is closer, but still not a fully generic behavior/rule-driven simulation.
- Economy visual actions are not yet ready for reliable conversion into ordered generic WebGL run actions.
- Several mappers sort by IDs instead of timeline order.
- Performance benchmarks are missing.
