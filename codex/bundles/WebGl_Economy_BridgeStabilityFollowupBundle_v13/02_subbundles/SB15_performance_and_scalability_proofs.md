# SB15 — Performance and scalability proofs

## Performance risks
- action duplication across frames
- per-action interop calls
- per-frame full scene rebuilds
- unbounded command result payloads
- non-indexed node/store/resource lookup
- unbounded motion queues
- GLB asset template disposal/caching
- metrics/invariant evaluation over all frames without indexing

## Tasks
- Add performance probes for 500 actors / 1000 visual actions / 2000 stores where feasible.
- Add metrics:
  - batch command count before/after normalization
  - interop calls avoided
  - projection duration
  - input pack validation duration
  - transition duration
  - bridge projection duration

## Tests
- bridge creates single run document without per-action interop.
- no duplicated global actions per frame.
