# SB18 — Performance and bottleneck proofs

## Components bottlenecks

- command batch coalescing
- repeated motion updates
- link synchronization for moving objects
- asset cache disposal
- symbol/overlay updates
- diagnostics/proof snapshot size

## Economy bottlenecks

- scenario validation on large input packs
- event stream expansion
- transition engine over many actors/steps
- ledger snapshot diffing
- visual action mapping

## Tasks

1. Add synthetic tests:
   - 100 actors x 50 steps
   - 1000 events
   - 1000 visual actions
   - 1000 command batch items
2. Track durations and command counts.
3. Record thresholds as warnings, not premature micro-optimizations.

## Done criteria

- Bottleneck report exists.
- No small/medium/mobile WebGL work.
