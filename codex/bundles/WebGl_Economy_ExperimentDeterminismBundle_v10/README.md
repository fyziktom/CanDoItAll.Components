# CanDoItAll WebGL + Economy Experiment Determinism Follow-up Bundle v10

## Purpose

This follow-up bundle prepares the next hardening wave after the latest Codex implementation in both repositories:

- `CanDoItAll.Components` branch: current checked-out branch, expected `webgl-engine`.
- `CanDoItAll.Economy` branch: current checked-out branch, expected current active branch / `main`.

The goal is **not** to implement the shared-well demo yet. The shared-well and farmer-land examples are used as stress tests to find missing generic engine, simulation, scenario, and experiment-design capabilities.

## Key direction

The next wave must move from "seeded example code" to **deterministic experiment inputs**:

```text
versioned experiment input files
  -> scenario definition
  -> parameter set
  -> placement / topology file
  -> rule/institution file
  -> run plan
  -> event stream
  -> deterministic simple/ledger simulation backend
  -> visual intention stream
  -> generic WebGL run actions
```

Randomization is allowed only as a separate input-generation step. The generated positions/parameters must be written into versioned JSON inputs before the simulation starts. Runtime simulation must consume explicit inputs, not hidden random state.

## Hard non-goals

- Do not implement a WebGL bridge from Economy to Components in this bundle.
- Do not implement the final shared-well demo UI.
- Do not optimize WebGL for small, medium, mobile, tablet, or phone screens.
- Do not add economy terms into generic WebGL runtime code.
- Do not collapse simple-account simulation and ledger-backed simulation into one implementation.

## Codex branch rule

Codex must work in the currently checked-out branch in each repository. It must not create a new branch.

## Primary artifacts

- `05_spreadsheets/implementation_matrix.xlsx`
- `01_architecture/01_experimental_input_pipeline.md`
- `01_architecture/02_generic_simulation_kernel_boundaries.md`
- `01_architecture/03_shared_well_and_farmer_land_gap_analysis.md`
- `02_subbundles/*.md`
- `08_experiment_examples/shared_well/*.json`
- `08_experiment_examples/farmer_land/*.json`
