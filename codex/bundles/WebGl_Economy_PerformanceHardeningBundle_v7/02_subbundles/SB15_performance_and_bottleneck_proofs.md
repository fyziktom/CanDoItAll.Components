# SB15 - Performance and bottleneck proofs

## Components proof

Large-screen desktop only.

Required browser/runtime proof:
- 100 agents
- 25 resource/building objects
- 200 links
- one command batch with 100 move actions
- one sequence action with multiple waypoints that must not be coalesced
- render scheduler idles after motions complete
- asset cache hit/miss counters visible
- link update count visible

## Economy proof

Required unit tests:
- shared well scenario definition loads from JSON
- event stream materializes same hashes across two runs
- visual action mapper emits move-to-target, return-to-anchor, show-symbol, change-pose
- simple accounts frame/delta validation catches dangling flow store ids
- ledger adapter delta is minimal

## Cross-repo proof

- Components build/test passes.
- Economy build/test passes.
- No forbidden references.
- No small/medium WebGL optimization tasks added.
