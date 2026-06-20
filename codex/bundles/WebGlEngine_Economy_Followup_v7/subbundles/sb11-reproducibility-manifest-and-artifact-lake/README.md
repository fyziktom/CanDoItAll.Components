# SB11 — Reproducibility manifest and artifact lake

## Repository scope

Economy

## Goal

Make every run reproducible and auditable.

## Tasks

- Emit manifest with repo commit hashes, package versions, scenario pack hash, behavior profile hash, policy id, seed, frame hash chain, metric hashes, and artifact hashes.
- Add artifact schema versions.
- Add artifact citation resolver used by readiness report.
- Validate artifact set completeness.
- Add manifest diff tool for comparing runs.

## Acceptance criteria

- Two identical runs produce identical deterministic outputs, except approved volatile fields.
- Manifest diff clearly identifies scenario/model/policy changes.
- Missing artifacts fail readiness.

## Required proof artifacts

- `proof/SB11/transcripts/reproducibility-manifest-tests.txt`
- `proof/SB11/artifacts/manifest-diff-sample.json`

## Gate

Artifacts must be sufficient for later audit without rerunning UI.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.

## Closure notes

- Added typed `economy-headless-run/v2` manifests with repo commits, package versions, scenario/catalog pack hashes, behavior profile hash, policy id, seed, frame hash chain, metric hashes, artifact hashes, artifact citations, artifact set hash, and deterministic manifest hash.
- Added schema-versioned headless artifacts for event stream, frames, frame hashes, metrics/invariants, warnings, and run summary.
- Added artifact-set validation and readiness integration so missing required artifacts mark readiness failed with `artifact-set-incomplete`.
- Added manifest diff tooling that categorizes scenario, model-output, policy, metric, artifact, repository, and package changes.
- Required proof is captured in `proof/SB11/transcripts/reproducibility-manifest-tests.txt` and `proof/SB11/artifacts/manifest-diff-sample.json`.
