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
