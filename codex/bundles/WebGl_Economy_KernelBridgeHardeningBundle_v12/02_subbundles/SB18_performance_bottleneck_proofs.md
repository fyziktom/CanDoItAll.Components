# SB18 - Performance Bottleneck Proofs

## Components bottlenecks

- repeated motion updates on many objects
- link geometry rebuilds when many connected objects move
- symbol animation keeping render loop alive
- GLB model cloning and material normalization
- command batch child result size returned through interop

## Economy bottlenecks

- repeated normalization per step
- hash canonicalization of large frames
- frame/delta materialization storing all frames in memory
- store lookups when event volume grows
- metric/invariant evaluation over many stores/resources

## Required benchmark probes

- 100 actors, 500 events, 1000 stores
- 100 staged WebGL actions
- 1000 command batch items
- 1000-frame materialization smoke test if feasible
- memory allocation notes

## Constraints

No mobile optimization. Desktop/large-screen proof only.
