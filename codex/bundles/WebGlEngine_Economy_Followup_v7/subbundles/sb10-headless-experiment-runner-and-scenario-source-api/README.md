# SB10 — Headless experiment runner and scenario-source API

## Repository scope

Economy

## Goal

Make headless runs the canonical way to investigate simulations.

## Tasks

- Add or harden CLI/tool command for running scenario catalog entries and outputting artifacts.
- Prefer scenario-id/catalog source over experimentJsonPath in new APIs.
- Emit readiness report, frame hashes, metrics, invariants, warnings, and reproducibility manifest.
- Support batch run over all scenario packs.
- Make path-based APIs legacy-compatible but not primary.

## Acceptance criteria

- A scenario can be run headlessly by scenario id.
- Artifacts are deterministic and self-contained.
- Output says whether the run is exploratory/headless-valid/research-ready.

## Required proof artifacts

- `proof/SB10/transcripts/headless-runner-tests.txt`
- `proof/SB10/artifacts/headless-run-manifest.json`

## Gate

Headless runner becomes the research source of truth.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.

## Closure notes

- Added catalog-first headless APIs: `RunScenario(IEconomySimulationScenarioCatalog, ...)`, `Run(EconomySimulationScenarioSource, ...)`, and `RunBatch(IEconomySimulationScenarioCatalog, ...)`.
- Retained legacy `experimentJsonPath` runner compatibility with explicit `scenarioSourceKind: legacy-path`.
- Added CLI `economy scenario run` support for `--catalog --scenario`, `--catalog --all`, and legacy `--experiment` modes with readiness status output.
- Added `warnings.json` and `headless-run-manifest.json` output alongside event streams, frames, frame hashes, metrics/invariants, readiness report, and run summary.
- Required proof is captured in `proof/SB10/transcripts/headless-runner-tests.txt` and `proof/SB10/artifacts/headless-run-manifest.json`.
