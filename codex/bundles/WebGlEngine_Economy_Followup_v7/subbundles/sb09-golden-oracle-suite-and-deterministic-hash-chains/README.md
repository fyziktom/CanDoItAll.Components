# SB09 — Golden oracle suite and deterministic hash chains

## Repository scope

Economy

## Goal

Prove core economic semantics with small scenarios where every expected frame/flow/metric is known.

## Tasks

- Add oracle scenarios: single transfer, insufficient stock, capacity rejection, shared resource depletion, trade+fee, relationship update, rule violation/enforcement.
- For each oracle, define expected stores, flows, issues, relationships, metrics, invariants, and frame hashes.
- Create deterministic frame hash chain over all frames.
- Add failure diff report for oracle mismatches.
- Ensure oracles are independent of WebGL.

## Acceptance criteria

- All golden oracles pass in research strict mode.
- A deliberately broken transfer produces an understandable oracle diff.
- Hash chain is stable across repeated runs.

## Required proof artifacts

- `proof/SB09/transcripts/golden-oracle-tests.txt`
- `proof/SB09/artifacts/oracle-diff-sample.json`

## Gate

No scenario classification above exploratory without oracle coverage or explicit no-oracle label.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.

## Closure notes

Status: completed

- Added golden oracle coverage for single transfer, insufficient stock, capacity rejection, shared resource depletion, trade plus fee, relationship update, and rule violation/enforcement.
- Added production handling for `rule.enforcement.apply` so enforcement events produce deterministic frame issues.
- Added deterministic frame hash-chain comparison across repeated materialization and an artifact-backed broken-transfer oracle diff.
- Added readiness `oracleCoverageLabel` gating: valid oracle proof labels reports `golden-oracle`; explicit `no-oracle` is required for no-oracle headless classification above exploratory.
- Required proof is captured in `proof/SB09/transcripts/golden-oracle-tests.txt` and `proof/SB09/artifacts/oracle-diff-sample.json`.
