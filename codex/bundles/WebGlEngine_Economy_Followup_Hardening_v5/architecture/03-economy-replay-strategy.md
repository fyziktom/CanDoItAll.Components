# Economy replay strategy

Economy needs two browser-apply modes:

- **Incremental forward apply:** apply only new contiguous frames when moving from N to N+1/N+k.
- **Full deterministic replay:** reset to initial scene and replay frames up to target when seeking backward, jumping, loading a new scenario, or after runtime uncertainty.

The UI must expose diagnostics:

- replayMode: incremental|full
- appliedFrameIndexes
- resetApplied
- frameReplayCount
- lastStableFrameIndex
