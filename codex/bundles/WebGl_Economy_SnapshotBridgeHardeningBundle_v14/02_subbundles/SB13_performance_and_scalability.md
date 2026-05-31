# SB13 - Performance and scalability proof

Goal:
- Protect the bridge and runtime from obvious bottlenecks.

Tasks:
1. Add perf probes for:
   - 100 actors,
   - 300 visual actions,
   - 1000 resource stores,
   - many links.
2. Measure:
   - scenario normalization,
   - transition engine materialization,
   - visual frame mapping,
   - WebGL bridge projection,
   - command batch generation.
3. Avoid O(n^2) lookups in bridge node/action resolution.
4. Report diagnostics as JSON.

Acceptance:
- Performance proof exists before adding richer demos.

## Status

Completed.

## Prerequisites

SB05 bridge projection proof and SB08 snapshot store/export.

## Validation Depth

Add or verify performance probes for 100 actors, 300 visual actions, 1000 resource stores, many links, and JSON timing diagnostics for normalization/materialization/mapping/bridge/batch stages.

## Progression Gate

SB15 may proceed only after performance proof exists and bridge/action resolution avoids obvious O(n^2) lookup paths for node/action resolution.
