# Experiment input discipline inspired by Vernon L. Smith

Economic experiments must have controlled, interpretable, reproducible induced values and initial conditions.

Required rules:

- Every treatment must have explicit input documents.
- Every random generator must be a pre-run artifact generator.
- Generator request, seed, algorithm version, and output hash must be recorded.
- Runtime simulation must consume only input JSONs, never hidden randomness.
- Initial actor positions, endowments, preferences, constraints, capacities, institution rules, market demand, and expected invariants must be inspectable before the run starts.
- The run output must cite the exact input pack hash and per-document hashes.
- Two runs with the same input pack must produce byte-stable hashes for scenario, event stream, frames/deltas, metrics, and invariant results.

This applies to both shared-well and farmer-land probes.
