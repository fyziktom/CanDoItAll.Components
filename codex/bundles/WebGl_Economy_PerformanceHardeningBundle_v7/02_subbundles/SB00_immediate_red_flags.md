# SB00 - Immediate red flags

These are the issues to check before doing broader implementation work.

## Components

1. Verify `disposeAssetCache(state)` is called from scene lifecycle dispose.
   - The asset cache helper exists.
   - Missing disposal would leak GLB template resources.
2. Verify command batching preserves ordered action semantics.
   - Duplicate motions per object must not be dropped in sequential actions.
3. Verify C# and JS batch coalescing rules are behaviorally equivalent.
   - Add shared golden scenario tests.
4. Verify sandbox playback uses reusable WebGlRunLib controller/action planner and not page-local orchestration.
5. Verify no WebGL task introduces small/medium screen optimization.

## Economy

1. Verify whether scenario definitions were actually added.
   - If not, add `SimulationScenarioDefinition` and loader before any more hardcoded examples.
2. Verify simple scenario flows do not contain dangling store ids.
3. Verify event/flow timestamps are not all pinned to scenario start.
4. Verify visual actions are temporal intentions, not static frame nodes only.
5. Verify Ledger delta is a true delta, not a whole-frame projection.
