# One-shot Codex prompt

You are working in two already-cloned repositories:

- CanDoItAll.Components
- CanDoItAll.Economy

Do not create a new branch in either repository. Work in the currently checked-out branch.

Implement the follow-up bundle:

`CanDoItAll_WebGl_Economy_ExperimentDeterminismBundle_v10`

Primary goal:

Prepare the simulation and visualization foundations for reproducible economic experiments without implementing the final shared-well demo.

Critical requirements:

1. Keep generic WebGL code fully domain-neutral.
2. Keep WebGL desktop/large-screen only. Do not optimize for mobile/tablet/small/medium screens.
3. Add deterministic experiment input-pack concepts in Economy.
4. Make random placement/input generation a pre-run step that writes JSON files.
5. Ensure the simulator consumes explicit JSON inputs and hashes them.
6. Add canonical scenario normalization and avoid alias drift.
7. Add typed references and canonical event taxonomy.
8. Add or prepare a generic simple transition engine; do not hardcode only shared-well.
9. Use shared-well and farmer-land as readiness probes, not final demos.
10. Follow CanDoItAll bundle workflow proof rules: proof manifests, semantic invariants, transcripts, source assertions, changed-file hashes.

Use `05_spreadsheets/implementation_matrix.xlsx` as the execution map.
